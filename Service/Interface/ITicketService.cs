using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface ITicketService
{
    Task<Ticket> GetByIdNotNullAsync(Guid id);
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket> CreateAsync(TicketDto dto);
    Task<Ticket> UpdateAsync(Guid id, TicketDto dto);
    Task<Ticket> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Ticket>> GetPagedAsync(int pageNumber, int pageSize);

    Task<List<Ticket>> GetAllByConcertIdAsync(Guid concertId);
    Task<Ticket> MarkAsPaidAsync(Guid id);
    Task<Ticket> CheckInAsync(Guid id);
    Task<Ticket> CancelAsync(Guid id);
    Task<Ticket> UpdateRefundPathByIdAsync(Guid id, string path);
}
