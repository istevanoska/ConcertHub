using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class ArtistExtensions
{
    public static ArtistResponse ToResponse(this Artist artist)
    {
        return new ArtistResponse(
            artist.Id,
            artist.Name,
            artist.Genre.ToString(),
            artist.Country,
            artist.FormedYear,
            artist.Bio);
    }
}
