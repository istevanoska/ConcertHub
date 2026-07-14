using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Ticket : BaseEntity
{
    public TicketStatus Status { get; set; }
    public decimal Price { get; set; }
    public string? SeatNumber { get; set; }

    public required string UserId { get; set; }
    public virtual ConcertApplicationUser User { get; set; } = null!;

    public Guid ConcertId { get; set; }
    public virtual Concert Concert { get; set; } = null!;

    public Guid TicketCategoryId { get; set; }
    public virtual TicketCategory TicketCategory { get; set; } = null!;

    public string? RefundDocumentPath { get; set; }
}
