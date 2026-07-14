namespace Web.Response;

public record TicketResponse(
    Guid Id,
    string UserId,
    string AttendeeName,
    Guid ConcertId,
    string ConcertTitle,
    string Category,
    string Status,
    decimal Price,
    string? SeatNumber
);
