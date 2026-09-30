using Domain.Models;

namespace Service.Interface;

public interface IFavoriteArtistService
{
    Task<FavoriteArtist> AddAsync(string userId, Guid artistId);
    Task RemoveAsync(string userId, Guid artistId);
    Task<List<Artist>> GetFavoriteArtistsAsync(string userId);
    Task NotifyFollowersAsync(Artist artist, Concert concert);
}
