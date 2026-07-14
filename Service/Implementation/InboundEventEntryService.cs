using Domain.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundEventEntryService : IInboundEventEntryService
{
    private readonly IRepository<InboundEventEntry> _repository;

    public InboundEventEntryService(IRepository<InboundEventEntry> repository)
    {
        _repository = repository;
    }

    public async Task<InboundEventEntry> CreateAsync(string rawPayload, Guid apiClientId)
    {
        var entry = new InboundEventEntry
        {
            RawPayload = rawPayload,
            ApiClientId = apiClientId,
            ReceivedAt = DateTime.UtcNow,
            Status = InboundEventStatus.Pending
        };

        return await _repository.InsertAsync(entry);
    }

    public async Task<InboundEventEntry> GetByIdNotNull(Guid id)
    {
        var result = await _repository.GetAsync(selector: x => x, predicate: x => x.Id == id);
        if (result == null)
            throw new InvalidOperationException($"Inbound event {id} not found");
        return result;
    }
}
