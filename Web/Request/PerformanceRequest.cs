namespace Web.Request;

public record PerformanceRequest(
    Guid ArtistId,
    Guid ConcertId,
    int SlotOrder,
    int DurationMinutes
);
