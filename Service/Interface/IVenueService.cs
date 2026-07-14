using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IVenueService
{
    Task<Venue> GetByIdNotNullAsync(Guid id);
    Task<Venue?> GetByIdAsync(Guid id);
    Task<List<Venue>> GetAllAsync(string? city);
    Task<Venue> CreateAsync(string name, string city, string address, int capacity);
    Task<Venue> UpdateAsync(Guid id, string name, string city, string address, int capacity);
    Task<Venue> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Venue>> GetPagedAsync(int pageNumber, int pageSize);
}
