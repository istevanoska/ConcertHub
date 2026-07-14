namespace Web.Response;

public record ConcertResponse(
    Guid Id,
    string Title,
    DateTime StartTime,
    DateTime EndTime,
    Guid VenueId,
    string VenueName,
    decimal BasePrice,
    int TicketsSold,
    int VenueCapacity,
    bool IsSoldOut
);
