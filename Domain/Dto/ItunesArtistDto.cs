using System.Text.Json.Serialization;

namespace Domain.Dto;

public class ItunesArtistDto
{
    [JsonPropertyName("artistId")]
    public long ArtistId { get; set; }

    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = string.Empty;

    [JsonPropertyName("primaryGenreName")]
    public string? PrimaryGenreName { get; set; }

    [JsonPropertyName("artistType")]
    public string? ArtistType { get; set; }

    [JsonPropertyName("artistLinkUrl")]
    public string? ArtistLinkUrl { get; set; }
}

public class ItunesSearchResponse
{
    [JsonPropertyName("resultCount")]
    public int ResultCount { get; set; }

    [JsonPropertyName("results")]
    public List<ItunesArtistDto> Results { get; set; } = new();
}
