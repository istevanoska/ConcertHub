using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Service.BackgroundService;

public class ConcertReminderBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConcertReminderBackgroundService> _logger;

    public ConcertReminderBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ConcertReminderBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                await reminderService.SendUpcomingConcertRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Concert reminder iteration failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
