namespace Web.Response;

public record ArtistResponse(
    Guid Id,
    string Name,
    string Genre,
    string Country,
    int FormedYear,
    string? Bio
);
