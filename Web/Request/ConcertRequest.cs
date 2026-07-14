namespace Web.Request;

public record ConcertRequest(
    string Title,
    Guid VenueId,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice
);
