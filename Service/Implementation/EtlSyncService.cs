using Domain.Config;
using Domain.Dto;
using Domain.Enums;
using Domain.ExternalModels;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlSyncService : IEtlSyncService
{
    private const string JobName = "ArtistsItunesSync";

    private readonly IRepository<EtlSyncLog> _etlSyncLogRepository;
    private readonly IMusicApiClient _musicApiClient;
    private readonly IArtistsRepository _artistsRepository;
    private readonly MusicApiSettings _settings;
    private readonly ILogger<EtlSyncService> _logger;

    public EtlSyncService(
        IRepository<EtlSyncLog> etlSyncLogRepository,
        IMusicApiClient musicApiClient,
        IArtistsRepository artistsRepository,
        IOptions<MusicApiSettings> settings,
        ILogger<EtlSyncService> logger)
    {
        _etlSyncLogRepository = etlSyncLogRepository;
        _musicApiClient = musicApiClient;
        _artistsRepository = artistsRepository;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SyncAllAsync()
    {
        var log = new EtlSyncLog
        {
            JobName = JobName,
            StartedAt = DateTime.UtcNow
        };

        try
        {

            var external = new Dictionary<long, ItunesArtistDto>();
            foreach (var term in _settings.SearchTerms)
            {
                var results = await _musicApiClient.SearchArtistsAsync(term, _settings.LimitPerTerm);
                foreach (var artist in results)
                {
                    if (artist.ArtistId != 0 && !string.IsNullOrWhiteSpace(artist.ArtistName))
                        external[artist.ArtistId] = artist;
                }
            }

            var artists = external.Values.Select(x => new Artist
            {
                Id = GuidHelper.FromLegacyId("Artist", x.ArtistId.ToString()),
                Name = x.ArtistName,
                Genre = MapGenre(x.PrimaryGenreName),
                Country = "Unknown",
                FormedYear = 0,
                Bio = $"Imported from iTunes ({x.PrimaryGenreName ?? "Unknown genre"})."
            }).ToList();

            await _artistsRepository.BulkInsertOrUpdateAsync(artists);

            log.RecordsProcessed = artists.Count;
            log.Success = true;
            _logger.LogInformation("ETL {Job} imported {Count} artists from iTunes.", JobName, artists.Count);
        }
        catch (Exception ex)
        {
            log.Success = false;
            log.ErrorMessage = ex.Message;
            _logger.LogError(ex, "ETL {Job} failed.", JobName);
        }
        finally
        {
            log.CompletedAt = DateTime.UtcNow;
            await _etlSyncLogRepository.InsertAsync(log);
        }
    }

    private static Genre MapGenre(string? genreName)
    {
        var g = (genreName ?? string.Empty).ToLowerInvariant();
        if (g.Contains("metal")) return Genre.Metal;
        if (g.Contains("rock") || g.Contains("alternative") || g.Contains("punk")) return Genre.Rock;
        if (g.Contains("jazz") || g.Contains("blues")) return Genre.Jazz;
        if (g.Contains("class")) return Genre.Classical;
        if (g.Contains("electro") || g.Contains("dance") || g.Contains("house")) return Genre.Electronic;
        if (g.Contains("hip") || g.Contains("rap")) return Genre.HipHop;
        if (g.Contains("folk") || g.Contains("country")) return Genre.Folk;
        return Genre.Pop;
    }
}
