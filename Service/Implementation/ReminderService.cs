using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ReminderService : IReminderService
{
    private readonly IRepository<Concert> _concertRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(
        IRepository<Concert> concertRepository,
        IEmailService emailService,
        ILogger<ReminderService> logger)
    {
        _concertRepository = concertRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<int> SendUpcomingConcertRemindersAsync()
    {
        var now = DateTime.UtcNow;
        var window = now.AddHours(24);

        var dueConcerts = await _concertRepository.GetAllAsync(
            selector: c => c,
            predicate: c => c.ReminderSentAt == null && c.StartTime > now && c.StartTime <= window,
            include: x => x.Include(c => c.Venue)
                .Include(c => c.Tickets).ThenInclude(t => t.User));

        var sentCount = 0;
        foreach (var concert in dueConcerts)
        {
            var recipients = concert.Tickets
                .Where(t => t.Status != TicketStatus.Cancelled)
                .Select(t => t.User.Email)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct();

            foreach (var email in recipients)
            {
                try
                {
                    var body = $"""
                                <h2>Потсетник: {concert.Title} е наскоро!</h2>
                                <p>Датум: {concert.StartTime:dddd, dd MMM yyyy HH:mm}</p>
                                <p>Локација: {concert.Venue.Name}, {concert.Venue.City}</p>
                                """;
                    await _emailService.SendAsync(email!, $"Потсетник: {concert.Title} е наскоро", body);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send reminder to {Email}", email);
                }
            }

            concert.ReminderSentAt = now;
            await _concertRepository.UpdateAsync(concert);
        }

        return sentCount;
    }
}
