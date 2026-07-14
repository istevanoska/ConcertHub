using Domain.Dto;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class TicketMapper
{
    private readonly ITicketService _ticketService;
    private readonly IFileUploadService _fileUploadService;

    public TicketMapper(ITicketService ticketService, IFileUploadService fileUploadService)
    {
        _ticketService = ticketService;
        _fileUploadService = fileUploadService;
    }

    public async Task<TicketResponse> BuyAsync(TicketRequest request)
    {
        var result = await _ticketService.CreateAsync(new TicketDto
        {
            UserId = request.UserId,
            ConcertId = request.ConcertId,
            TicketCategoryId = request.TicketCategoryId,
            SeatNumber = request.SeatNumber
        });
        return result.ToResponse();
    }

    public async Task<TicketResponse> UpdateAsync(Guid id, TicketRequest request)
    {
        var result = await _ticketService.UpdateAsync(id, new TicketDto
        {
            UserId = request.UserId,
            ConcertId = request.ConcertId,
            TicketCategoryId = request.TicketCategoryId,
            SeatNumber = request.SeatNumber
        });
        return result.ToResponse();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _ticketService.DeleteByIdAsync(id);
    }

    public async Task<TicketResponse> GetByIdAsync(Guid id)
    {
        var result = await _ticketService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<List<TicketResponse>> GetAllByConcertIdAsync(Guid concertId)
    {
        var result = await _ticketService.GetAllByConcertIdAsync(concertId);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<TicketResponse> MarkAsPaidAsync(Guid id)
    {
        var result = await _ticketService.MarkAsPaidAsync(id);
        return result.ToResponse();
    }

    public async Task<TicketResponse> CheckInAsync(Guid id)
    {
        var result = await _ticketService.CheckInAsync(id);
        return result.ToResponse();
    }

    public async Task<TicketResponse> CancelAsync(Guid id)
    {
        var result = await _ticketService.CancelAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<TicketResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _ticketService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }

    public async Task<TicketResponse> UploadRefundDocumentAsync(Guid id, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var path = await _fileUploadService.UploadFileAsync(ms.ToArray(), file.FileName);
        var result = await _ticketService.UpdateRefundPathByIdAsync(id, path);
        return result.ToResponse();
    }
}
