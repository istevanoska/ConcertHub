namespace Service.Interface;

public interface IReminderService
{
    Task<int> SendUpcomingConcertRemindersAsync();
}
