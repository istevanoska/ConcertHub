using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface ITicketCategoryService
{
    Task<TicketCategory> GetByIdNotNullAsync(Guid id);
    Task<TicketCategory?> GetByIdAsync(Guid id);
    Task<List<TicketCategory>> GetAllAsync();
    Task<TicketCategory> CreateAsync(string name, decimal priceMultiplier, string? description);
    Task<TicketCategory> UpdateAsync(Guid id, string name, decimal priceMultiplier, string? description);
    Task<TicketCategory> DeleteByIdAsync(Guid id);
}
