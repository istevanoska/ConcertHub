namespace Web.Request;

public record TicketRequest(
    Guid ConcertId,
    Guid TicketCategoryId,
    string UserId,
    string? SeatNumber
);
