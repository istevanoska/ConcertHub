using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IPerformanceService
{
    Task<Performance> GetByIdNotNullAsync(Guid id);
    Task<Performance?> GetByIdAsync(Guid id);
    Task<List<Performance>> GetAllByConcertIdAsync(Guid concertId);
    Task<Performance> CreateAsync(Guid artistId, Guid concertId, int slotOrder, int durationMinutes);
    Task<Performance> UpdateAsync(Guid id, Guid artistId, Guid concertId, int slotOrder, int durationMinutes);
    Task<Performance> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Performance>> GetPagedAsync(int pageNumber, int pageSize);
}
