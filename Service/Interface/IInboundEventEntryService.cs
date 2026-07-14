using Domain.Models;

namespace Service.Interface;

public interface IInboundEventEntryService
{
    Task<InboundEventEntry> CreateAsync(string rawPayload, Guid apiClientId);
    Task<InboundEventEntry> GetByIdNotNull(Guid id);
}
