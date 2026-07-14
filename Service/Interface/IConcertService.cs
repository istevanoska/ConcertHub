using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IConcertService
{
    Task<Concert> GetByIdNotNullAsync(Guid id);
    Task<Concert?> GetByIdAsync(Guid id);
    Task<List<Concert>> GetAllAsync(string? venueName, DateOnly? date);
    Task<Concert> CreateAsync(string title, DateTime startTime, DateTime endTime, Guid venueId, decimal basePrice);
    Task<Concert> UpdateAsync(Guid id, string title, DateTime startTime, DateTime endTime, Guid venueId, decimal basePrice);
    Task<Concert> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Concert>> GetPagedAsync(int pageNumber, int pageSize);

    Task IncrementTicketsSoldAsync(Guid id);
    Task DecrementTicketsSoldAsync(Guid id);

    Task<List<RevenueReportDto>> GetRevenueReportAsync();
}
