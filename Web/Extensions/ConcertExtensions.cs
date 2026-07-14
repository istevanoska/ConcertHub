using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class ConcertExtensions
{
    public static ConcertResponse ToResponse(this Concert concert)
    {
        var capacity = concert.Venue?.Capacity ?? 0;
        return new ConcertResponse(
            concert.Id,
            concert.Title,
            concert.StartTime,
            concert.EndTime,
            concert.VenueId,
            concert.Venue?.Name ?? string.Empty,
            concert.BasePrice,
            concert.TicketsSold,
            capacity,
            capacity > 0 && concert.TicketsSold >= capacity);
    }

    public static ConcertBasicResponse ToBasicResponse(this Concert concert)
    {
        return new ConcertBasicResponse(
            concert.Id,
            concert.Title,
            concert.VenueId,
            concert.StartTime,
            concert.EndTime);
    }
}
