using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ConcertMapper
{
    private readonly IConcertService _concertService;

    public ConcertMapper(IConcertService concertService)
    {
        _concertService = concertService;
    }

    public async Task<List<ConcertResponse>> GetAllAsync(string? venueName, DateOnly? date)
    {
        var result = await _concertService.GetAllAsync(venueName, date);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ConcertResponse> GetByIdAsync(Guid id)
    {
        var result = await _concertService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<ConcertBasicResponse> InsertAsync(ConcertRequest request)
    {
        var result = await _concertService.CreateAsync(
            request.Title, request.StartTime, request.EndTime, request.VenueId, request.BasePrice);
        return result.ToBasicResponse();
    }

    public async Task<ConcertBasicResponse> UpdateAsync(Guid id, ConcertRequest request)
    {
        var result = await _concertService.UpdateAsync(
            id, request.Title, request.StartTime, request.EndTime, request.VenueId, request.BasePrice);
        return result.ToBasicResponse();
    }

    public async Task<ConcertBasicResponse> DeleteAsync(Guid id)
    {
        var result = await _concertService.DeleteByIdAsync(id);
        return result.ToBasicResponse();
    }

    public async Task<PaginatedResponse<ConcertResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _concertService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}
