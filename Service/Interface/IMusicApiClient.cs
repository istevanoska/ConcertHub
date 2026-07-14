using Domain.Dto;

namespace Service.Interface;

public interface IMusicApiClient
{
    Task<List<ItunesArtistDto>> SearchArtistsAsync(string term, int limit);
}
