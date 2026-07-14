using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class PerformanceMapper
{
    private readonly IPerformanceService _performanceService;

    public PerformanceMapper(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    public async Task<List<PerformanceResponse>> GetAllByConcertIdAsync(Guid concertId)
    {
        var result = await _performanceService.GetAllByConcertIdAsync(concertId);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<PerformanceResponse> GetByIdAsync(Guid id)
    {
        var result = await _performanceService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<PerformanceResponse> InsertAsync(PerformanceRequest request)
    {
        var result = await _performanceService.CreateAsync(
            request.ArtistId, request.ConcertId, request.SlotOrder, request.DurationMinutes);
        return result.ToResponse();
    }

    public async Task<PerformanceResponse> UpdateAsync(Guid id, PerformanceRequest request)
    {
        var result = await _performanceService.UpdateAsync(
            id, request.ArtistId, request.ConcertId, request.SlotOrder, request.DurationMinutes);
        return result.ToResponse();
    }

    public async Task<PerformanceResponse> DeleteAsync(Guid id)
    {
        var result = await _performanceService.DeleteByIdAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<PerformanceResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _performanceService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}
