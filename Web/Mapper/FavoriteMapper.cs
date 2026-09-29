using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class FavoriteMapper
{
    private readonly IFavoriteArtistService _favoriteArtistService;

    public FavoriteMapper(IFavoriteArtistService favoriteArtistService)
    {
        _favoriteArtistService = favoriteArtistService;
    }

    public async Task AddAsync(FavoriteRequest request)
    {
        await _favoriteArtistService.AddAsync(request.UserId, request.ArtistId);
    }

    public async Task RemoveAsync(FavoriteRequest request)
    {
        await _favoriteArtistService.RemoveAsync(request.UserId, request.ArtistId);
    }

    public async Task<List<ArtistResponse>> GetFavoriteArtistsAsync(string userId)
    {
        var result = await _favoriteArtistService.GetFavoriteArtistsAsync(userId);
        return result.Select(a => a.ToResponse()).ToList();
    }
}
