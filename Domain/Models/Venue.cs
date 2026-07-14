using Domain.Common;

namespace Domain.Models;

public class Venue : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public virtual ICollection<Concert> Concerts { get; set; } = new List<Concert>();
}
