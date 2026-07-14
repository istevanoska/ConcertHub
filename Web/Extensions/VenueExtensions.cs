using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class VenueExtensions
{
    public static VenueResponse ToResponse(this Venue venue)
    {
        return new VenueResponse(
            venue.Id,
            venue.Name,
            venue.City,
            venue.Address,
            venue.Capacity);
    }
}
