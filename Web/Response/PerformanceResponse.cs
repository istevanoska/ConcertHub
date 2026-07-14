namespace Web.Response;

public record PerformanceResponse(
    Guid Id,
    Guid ArtistId,
    string ArtistName,
    Guid ConcertId,
    int SlotOrder,
    int DurationMinutes
);
