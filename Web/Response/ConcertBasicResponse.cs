namespace Web.Response;

public record ConcertBasicResponse(
    Guid Id,
    string Title,
    Guid VenueId,
    DateTime StartTime,
    DateTime EndTime
);
