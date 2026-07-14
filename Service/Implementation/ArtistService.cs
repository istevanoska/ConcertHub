using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ArtistService : IArtistService
{
    private readonly IRepository<Artist> _artistRepository;

    public ArtistService(IRepository<Artist> artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<Artist> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Artist with id {id} not found");
        return result;
    }

    public async Task<Artist?> GetByIdAsync(Guid id)
    {
        return await _artistRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<List<Artist>> GetAllAsync(string? name, Genre? genre)
    {
        return await _artistRepository.GetAllAsync(
            selector: x => x,
            predicate: x => (name == null || x.Name.Contains(name)) &&
                            (genre == null || x.Genre == genre),
            orderBy: x => x.OrderBy(a => a.Name));
    }

    public async Task<Artist> CreateAsync(string name, Genre genre, string country, int formedYear, string? bio)
    {
        var artist = new Artist
        {
            Name = name,
            Genre = genre,
            Country = country,
            FormedYear = formedYear,
            Bio = bio
        };
        return await _artistRepository.InsertAsync(artist);
    }

    public async Task<Artist> UpdateAsync(Guid id, string name, Genre genre, string country, int formedYear, string? bio)
    {
        var artist = await GetByIdNotNullAsync(id);
        artist.Name = name;
        artist.Genre = genre;
        artist.Country = country;
        artist.FormedYear = formedYear;
        artist.Bio = bio;
        return await _artistRepository.UpdateAsync(artist);
    }

    public async Task<Artist> DeleteByIdAsync(Guid id)
    {
        var artist = await GetByIdNotNullAsync(id);
        return await _artistRepository.DeleteAsync(artist);
    }

    public async Task<PaginatedResult<Artist>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _artistRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: x => x.OrderBy(a => a.Name),
            asNoTracking: true);
    }
}
