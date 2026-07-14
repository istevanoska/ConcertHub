using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TicketCategoryService : ITicketCategoryService
{
    private readonly IRepository<TicketCategory> _repository;

    public TicketCategoryService(IRepository<TicketCategory> repository)
    {
        _repository = repository;
    }

    public async Task<TicketCategory> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Ticket category with id {id} not found");
        return result;
    }

    public async Task<TicketCategory?> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<List<TicketCategory>> GetAllAsync()
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            orderBy: x => x.OrderBy(c => c.Name));
    }

    public async Task<TicketCategory> CreateAsync(string name, decimal priceMultiplier, string? description)
    {
        var category = new TicketCategory
        {
            Name = name,
            PriceMultiplier = priceMultiplier,
            Description = description
        };
        return await _repository.InsertAsync(category);
    }

    public async Task<TicketCategory> UpdateAsync(Guid id, string name, decimal priceMultiplier, string? description)
    {
        var category = await GetByIdNotNullAsync(id);
        category.Name = name;
        category.PriceMultiplier = priceMultiplier;
        category.Description = description;
        return await _repository.UpdateAsync(category);
    }

    public async Task<TicketCategory> DeleteByIdAsync(Guid id)
    {
        var category = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(category);
    }
}
