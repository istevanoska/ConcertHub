using Domain.Enums;

namespace Web.Request;

public record ArtistRequest(
    string Name,
    Genre Genre,
    string Country,
    int FormedYear,
    string? Bio
);
