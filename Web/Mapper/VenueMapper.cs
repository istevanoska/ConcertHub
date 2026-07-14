using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class VenueMapper
{
    private readonly IVenueService _venueService;

    public VenueMapper(IVenueService venueService)
    {
        _venueService = venueService;
    }

    public async Task<List<VenueResponse>> GetAllAsync(string? city)
    {
        var result = await _venueService.GetAllAsync(city);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<VenueResponse> GetByIdAsync(Guid id)
    {
        var result = await _venueService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<VenueResponse> InsertAsync(VenueRequest request)
    {
        var result = await _venueService.CreateAsync(request.Name, request.City, request.Address, request.Capacity);
        return result.ToResponse();
    }

    public async Task<VenueResponse> UpdateAsync(Guid id, VenueRequest request)
    {
        var result = await _venueService.UpdateAsync(id, request.Name, request.City, request.Address, request.Capacity);
        return result.ToResponse();
    }

    public async Task<VenueResponse> DeleteAsync(Guid id)
    {
        var result = await _venueService.DeleteByIdAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<VenueResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _venueService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}
