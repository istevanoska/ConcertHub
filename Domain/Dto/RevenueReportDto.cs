namespace Domain.Dto;

public class RevenueReportDto
{
    public Guid ConcertId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public int TicketsSold { get; set; }
    public int VenueCapacity { get; set; }
    public decimal TotalRevenue { get; set; }
    public double OccupancyRate => VenueCapacity == 0 ? 0 : (double)TicketsSold / VenueCapacity;
    public bool IsSoldOut => VenueCapacity > 0 && TicketsSold >= VenueCapacity;
}
