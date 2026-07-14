using Domain.Dto;
using Domain.Enums;
using Domain.Models;

namespace Service.Interface;

public interface IArtistService
{
    Task<Artist> GetByIdNotNullAsync(Guid id);
    Task<Artist?> GetByIdAsync(Guid id);
    Task<List<Artist>> GetAllAsync(string? name, Genre? genre);
    Task<Artist> CreateAsync(string name, Genre genre, string country, int formedYear, string? bio);
    Task<Artist> UpdateAsync(Guid id, string name, Genre genre, string country, int formedYear, string? bio);
    Task<Artist> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Artist>> GetPagedAsync(int pageNumber, int pageSize);
}
