using Domain.Common;

namespace Domain.Models;

public class TicketCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal PriceMultiplier { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
