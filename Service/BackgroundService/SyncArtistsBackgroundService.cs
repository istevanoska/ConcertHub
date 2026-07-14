using Domain.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Interface;

namespace Service.BackgroundService;

public class SyncArtistsBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MusicApiSettings _settings;
    private readonly ILogger<SyncArtistsBackgroundService> _logger;

    public SyncArtistsBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<MusicApiSettings> settings,
        ILogger<SyncArtistsBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation(
                "Artist ETL background sync is disabled (MusicApiSettings.Enabled = false). " +
                "Enable it in configuration, or trigger manually via POST /api/report/etl/run.");
            return;
        }

        try { await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); }
        catch (TaskCanceledException) { return; }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var etlService = scope.ServiceProvider.GetRequiredService<IEtlSyncService>();
                await etlService.SyncAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Artist ETL sync iteration failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
