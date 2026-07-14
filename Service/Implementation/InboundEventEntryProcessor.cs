using System.Text.Json;
using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundEventEntryProcessor : IInboundEventEntryProcessor
{
    private readonly IRepository<InboundEventEntry> _repository;
    private readonly ITicketService _ticketService;
    private readonly ILogger<InboundEventEntryProcessor> _logger;

    public InboundEventEntryProcessor(
        IRepository<InboundEventEntry> repository,
        ITicketService ticketService,
        ILogger<InboundEventEntryProcessor> logger)
    {
        _repository = repository;
        _ticketService = ticketService;
        _logger = logger;
    }

    public async Task ProcessPendingEventsAsync()
    {
        var pending = await _repository.GetAllAsync(
            selector: x => x,
            predicate: e => e.Status == InboundEventStatus.Pending,
            orderBy: q => q.OrderBy(e => e.ReceivedAt),
            take: 10);

        foreach (var entry in pending)
        {
            await ProcessEventEntry(entry);
        }
    }

    public async Task<Ticket?> ProcessEventEntry(InboundEventEntry entry)
    {
        try
        {
            entry.Status = InboundEventStatus.Processing;
            await _repository.UpdateAsync(entry);

            var request = JsonSerializer.Deserialize<TicketRequestDto>(entry.RawPayload)
                          ?? throw new InvalidOperationException("Malformed inbound payload.");

            var ticket = await _ticketService.CreateAsync(new TicketDto
            {
                UserId = request.UserId,
                ConcertId = request.ConcertId,
                TicketCategoryId = request.TicketCategoryId,
                SeatNumber = request.SeatNumber
            });

            entry.Status = InboundEventStatus.Completed;
            entry.ProcessedAt = DateTime.UtcNow;
            entry.CreatedTicketId = ticket.Id;
            await _repository.UpdateAsync(entry);

            return ticket;
        }
        catch (Exception ex)
        {
            entry.Status = InboundEventStatus.Failed;
            entry.ErrorMessage = ex.Message;
            entry.ProcessedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entry);
            _logger.LogWarning(ex, "Failed to process inbound event {Id}", entry.Id);
            return null;
        }
    }
}
