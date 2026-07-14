namespace Domain.Dto;

public class TicketRequestDto
{
    public Guid ConcertId { get; set; }
    public Guid TicketCategoryId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? SeatNumber { get; set; }
}
