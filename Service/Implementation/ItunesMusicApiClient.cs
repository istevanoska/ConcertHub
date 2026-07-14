using System.Net.Http.Json;
using Domain.Dto;
using Service.Interface;

namespace Service.Implementation;

public class ItunesMusicApiClient : IMusicApiClient
{
    private readonly HttpClient _client;

    public ItunesMusicApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<ItunesArtistDto>> SearchArtistsAsync(string term, int limit)
    {
        var path = $"/search?term={Uri.EscapeDataString(term)}&entity=musicArtist&limit={limit}";

        var response = await _client.GetAsync(path);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ItunesSearchResponse>();
        return result?.Results ?? new List<ItunesArtistDto>();
    }
}
