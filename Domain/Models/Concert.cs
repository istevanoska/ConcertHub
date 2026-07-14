using Domain.Common;

namespace Domain.Models;

public class Concert : BaseAuditableEntity<ConcertApplicationUser>
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public decimal BasePrice { get; set; }

    public int TicketsSold { get; set; }

    public Guid VenueId { get; set; }
    public virtual Venue Venue { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>();
}
