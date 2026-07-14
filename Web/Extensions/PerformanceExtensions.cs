using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class PerformanceExtensions
{
    public static PerformanceResponse ToResponse(this Performance performance)
    {
        return new PerformanceResponse(
            performance.Id,
            performance.ArtistId,
            performance.Artist?.Name ?? string.Empty,
            performance.ConcertId,
            performance.SlotOrder,
            performance.DurationMinutes);
    }
}
