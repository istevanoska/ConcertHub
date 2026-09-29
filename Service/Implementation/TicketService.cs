using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TicketService : ITicketService
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<ConcertApplicationUser> _userRepository;
    private readonly IConcertService _concertService;
    private readonly ITicketCategoryService _ticketCategoryService;
    private readonly IEmailService _emailService;
    private readonly IQrCodeService _qrCodeService;
    private readonly ILogger<TicketService> _logger;

    public TicketService(
        IRepository<Ticket> ticketRepository,
        IRepository<ConcertApplicationUser> userRepository,
        IConcertService concertService,
        ITicketCategoryService ticketCategoryService,
        IEmailService emailService,
        IQrCodeService qrCodeService,
        ILogger<TicketService> logger)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _concertService = concertService;
        _ticketCategoryService = ticketCategoryService;
        _emailService = emailService;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task<Ticket> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Ticket with id {id} not found");
        return result;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        return await _ticketRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id,
            include: x => x.Include(t => t.User)
                .Include(t => t.Concert)
                .Include(t => t.TicketCategory));
    }

    public async Task<List<Ticket>> GetAllAsync()
    {
        return await _ticketRepository.GetAllAsync(
            selector: x => x,
            include: x => x.Include(t => t.User)
                .Include(t => t.Concert)
                .Include(t => t.TicketCategory));
    }

    public async Task<Ticket> CreateAsync(TicketDto dto)
    {
        var concert = await _concertService.GetByIdNotNullAsync(dto.ConcertId);
        var category = await _ticketCategoryService.GetByIdNotNullAsync(dto.TicketCategoryId);

        if (concert.TicketsSold >= concert.Venue.Capacity)
            throw new InvalidOperationException(
                $"Concert '{concert.Title}' is sold out ({concert.Venue.Capacity} seats).");

        var price = Math.Round(concert.BasePrice * category.PriceMultiplier, 2);

        var ticket = new Ticket
        {
            UserId = dto.UserId,
            ConcertId = dto.ConcertId,
            TicketCategoryId = dto.TicketCategoryId,
            SeatNumber = dto.SeatNumber,
            Price = price,
            Status = TicketStatus.Reserved
        };

        var result = await _ticketRepository.InsertAsync(ticket);
        await _concertService.IncrementTicketsSoldAsync(dto.ConcertId);

        await SendConfirmationEmailAsync(dto.UserId, concert, category, price, "reserved");

        return await GetByIdNotNullAsync(result.Id);
    }

    public async Task<Ticket> UpdateAsync(Guid id, TicketDto dto)
    {
        var ticket = await GetByIdNotNullAsync(id);
        ticket.SeatNumber = dto.SeatNumber;
        ticket.TicketCategoryId = dto.TicketCategoryId;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> DeleteByIdAsync(Guid id)
    {
        var ticket = await GetByIdNotNullAsync(id);
        if (ticket.Status != TicketStatus.Cancelled)
        {
            await _concertService.DecrementTicketsSoldAsync(ticket.ConcertId);
        }
        return await _ticketRepository.DeleteAsync(ticket);
    }

    public async Task<PaginatedResult<Ticket>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _ticketRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(t => t.User)
                .Include(t => t.Concert)
                .Include(t => t.TicketCategory),
            asNoTracking: true);
    }

    public async Task<List<Ticket>> GetAllByConcertIdAsync(Guid concertId)
    {
        return await _ticketRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.ConcertId == concertId,
            include: x => x.Include(t => t.User).Include(t => t.TicketCategory));
    }

    public async Task<Ticket> MarkAsPaidAsync(Guid id)
    {
        var ticket = await GetByIdNotNullAsync(id);
        ticket.Status = TicketStatus.Paid;
        var result = await _ticketRepository.UpdateAsync(ticket);
        await SendConfirmationEmailAsync(ticket.UserId, ticket.Concert, ticket.TicketCategory, ticket.Price, "paid");
        return result;
    }

    public async Task<Ticket> CheckInAsync(Guid id)
    {
        var ticket = await GetByIdNotNullAsync(id);

        if (ticket.Status == TicketStatus.Cancelled)
            throw new InvalidOperationException("Cannot check in a cancelled ticket.");

        if (ticket.Status == TicketStatus.Used)
            throw new InvalidOperationException("This ticket has already been checked in.");

        ticket.Status = TicketStatus.Used;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<byte[]> GenerateQrCodeAsync(Guid id)
    {
        var ticket = await GetByIdNotNullAsync(id);
        return _qrCodeService.GeneratePng(ticket.Id.ToString());
    }

    public async Task<Ticket> CancelAsync(Guid id)
    {
        var ticket = await GetByIdNotNullAsync(id);

        if (ticket.Status == TicketStatus.Cancelled)
            throw new InvalidOperationException("Ticket is already cancelled.");

        if (ticket.Concert.StartTime <= DateTime.UtcNow.AddHours(24))
            throw new InvalidOperationException("Tickets cannot be cancelled within 24 hours of the concert.");

        ticket.Status = TicketStatus.Cancelled;
        var result = await _ticketRepository.UpdateAsync(ticket);
        await _concertService.DecrementTicketsSoldAsync(ticket.ConcertId);
        return result;
    }

    public async Task<Ticket> UpdateRefundPathByIdAsync(Guid id, string path)
    {
        var ticket = await GetByIdNotNullAsync(id);
        ticket.RefundDocumentPath = path;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    private async Task SendConfirmationEmailAsync(string userId, Concert concert, TicketCategory category, decimal price, string state)
    {
        try
        {
            var email = await _userRepository.GetAsync(
                selector: u => u.Email,
                predicate: u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(email))
                return;

            var body = $"""
                        <h2>Your ticket is {state}</h2>
                        <p>Concert: <strong>{concert.Title}</strong></p>
                        <p>Date: {concert.StartTime:dddd, dd MMM yyyy HH:mm}</p>
                        <p>Category: {category.Name}</p>
                        <p>Price: {price:0.00} EUR</p>
                        """;

            await _emailService.SendAsync(email, $"Ticket {state}: {concert.Title}", body);
        }
        catch (Exception ex)
        {

            _logger.LogWarning(ex, "Failed to send confirmation email for user {UserId}", userId);
        }
    }
}
