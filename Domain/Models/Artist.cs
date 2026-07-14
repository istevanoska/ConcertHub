using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Artist : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Genre Genre { get; set; }
    public string Country { get; set; } = string.Empty;
    public int FormedYear { get; set; }
    public string? Bio { get; set; }

    public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>();
}
