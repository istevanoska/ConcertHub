using Domain.Common;

namespace Domain.Models;

public class FavoriteArtist : BaseEntity
{
    public required string UserId { get; set; }
    public virtual ConcertApplicationUser User { get; set; } = null!;

    public Guid ArtistId { get; set; }
    public virtual Artist Artist { get; set; } = null!;
}
