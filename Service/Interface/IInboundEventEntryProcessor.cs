using Domain.Models;

namespace Service.Interface;

public interface IInboundEventEntryProcessor
{
    Task ProcessPendingEventsAsync();
    Task<Ticket?> ProcessEventEntry(InboundEventEntry entry);
}
