using Domain.Common;

namespace Domain.Models;

public class Performance : BaseAuditableEntity<ConcertApplicationUser>
{
    public Guid ArtistId { get; set; }
    public virtual Artist Artist { get; set; } = null!;

    public Guid ConcertId { get; set; }
    public virtual Concert Concert { get; set; } = null!;

    public int SlotOrder { get; set; }
    public int DurationMinutes { get; set; }
}
