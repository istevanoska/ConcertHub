namespace Domain.Dto;

public class TicketDto
{
    public required string UserId { get; set; }
    public Guid ConcertId { get; set; }
    public Guid TicketCategoryId { get; set; }
    public string? SeatNumber { get; set; }
}
