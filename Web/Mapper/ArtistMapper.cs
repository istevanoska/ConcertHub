using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ArtistMapper
{
    private readonly IArtistService _artistService;

    public ArtistMapper(IArtistService artistService)
    {
        _artistService = artistService;
    }

    public async Task<List<ArtistResponse>> GetAllAsync(string? name, Domain.Enums.Genre? genre)
    {
        var result = await _artistService.GetAllAsync(name, genre);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ArtistResponse> GetByIdAsync(Guid id)
    {
        var result = await _artistService.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<ArtistResponse> InsertAsync(ArtistRequest request)
    {
        var result = await _artistService.CreateAsync(
            request.Name, request.Genre, request.Country, request.FormedYear, request.Bio);
        return result.ToResponse();
    }

    public async Task<ArtistResponse> UpdateAsync(Guid id, ArtistRequest request)
    {
        var result = await _artistService.UpdateAsync(
            id, request.Name, request.Genre, request.Country, request.FormedYear, request.Bio);
        return result.ToResponse();
    }

    public async Task<ArtistResponse> DeleteAsync(Guid id)
    {
        var result = await _artistService.DeleteByIdAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<ArtistResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _artistService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}
