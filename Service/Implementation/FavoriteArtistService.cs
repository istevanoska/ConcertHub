using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class FavoriteArtistService : IFavoriteArtistService
{
    private readonly IRepository<FavoriteArtist> _favoriteRepository;
    private readonly IArtistService _artistService;
    private readonly IEmailService _emailService;
    private readonly ILogger<FavoriteArtistService> _logger;

    public FavoriteArtistService(
        IRepository<FavoriteArtist> favoriteRepository,
        IArtistService artistService,
        IEmailService emailService,
        ILogger<FavoriteArtistService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _artistService = artistService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<FavoriteArtist> AddAsync(string userId, Guid artistId)
    {
        await _artistService.GetByIdNotNullAsync(artistId);

        var exists = await _favoriteRepository.ExistsAsync(f => f.UserId == userId && f.ArtistId == artistId);
        if (exists)
            throw new InvalidOperationException("Artist is already in favorites.");

        return await _favoriteRepository.InsertAsync(new FavoriteArtist { UserId = userId, ArtistId = artistId });
    }

    public async Task RemoveAsync(string userId, Guid artistId)
    {
        var favorite = await _favoriteRepository.GetAsync(
            selector: f => f,
            predicate: f => f.UserId == userId && f.ArtistId == artistId);

        if (favorite == null)
            throw new InvalidOperationException("Artist is not in favorites.");

        await _favoriteRepository.DeleteAsync(favorite);
    }

    public async Task<List<Artist>> GetFavoriteArtistsAsync(string userId)
    {
        return await _favoriteRepository.GetAllAsync(
            selector: f => f.Artist,
            predicate: f => f.UserId == userId);
    }

    public async Task NotifyFollowersAsync(Artist artist, Concert concert)
    {
        var followerEmails = await _favoriteRepository.GetAllAsync(
            selector: f => f.User.Email!,
            predicate: f => f.ArtistId == artist.Id);

        foreach (var email in followerEmails)
        {
            if (string.IsNullOrWhiteSpace(email))
                continue;

            try
            {
                var body = $"""
                            <h2>{artist.Name} настапува на нов концерт!</h2>
                            <p>Концерт: <strong>{concert.Title}</strong></p>
                            <p>Датум: {concert.StartTime:dddd, dd MMM yyyy HH:mm}</p>
                            """;
                await _emailService.SendAsync(email, $"{artist.Name} е закажан/а за {concert.Title}", body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send favorite-artist notification to {Email}", email);
            }
        }
    }
}
