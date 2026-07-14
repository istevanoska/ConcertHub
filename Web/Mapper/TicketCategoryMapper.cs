using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class TicketCategoryMapper
{
    private readonly ITicketCategoryService _service;

    public TicketCategoryMapper(ITicketCategoryService service)
    {
        _service = service;
    }

    public async Task<List<TicketCategoryResponse>> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<TicketCategoryResponse> GetByIdAsync(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<TicketCategoryResponse> InsertAsync(TicketCategoryRequest request)
    {
        var result = await _service.CreateAsync(request.Name, request.PriceMultiplier, request.Description);
        return result.ToResponse();
    }

    public async Task<TicketCategoryResponse> UpdateAsync(Guid id, TicketCategoryRequest request)
    {
        var result = await _service.UpdateAsync(id, request.Name, request.PriceMultiplier, request.Description);
        return result.ToResponse();
    }

    public async Task<TicketCategoryResponse> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);
        return result.ToResponse();
    }
}
