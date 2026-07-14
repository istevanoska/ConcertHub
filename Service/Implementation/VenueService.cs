using Domain.Dto;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class VenueService : IVenueService
{
    private readonly IRepository<Venue> _venueRepository;

    public VenueService(IRepository<Venue> venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<Venue> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Venue with id {id} not found");
        return result;
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await _venueRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<List<Venue>> GetAllAsync(string? city)
    {
        return await _venueRepository.GetAllAsync(
            selector: x => x,
            predicate: x => city == null || x.City.Contains(city),
            orderBy: x => x.OrderBy(v => v.Name));
    }

    public async Task<Venue> CreateAsync(string name, string city, string address, int capacity)
    {
        var venue = new Venue
        {
            Name = name,
            City = city,
            Address = address,
            Capacity = capacity
        };
        return await _venueRepository.InsertAsync(venue);
    }

    public async Task<Venue> UpdateAsync(Guid id, string name, string city, string address, int capacity)
    {
        var venue = await GetByIdNotNullAsync(id);
        venue.Name = name;
        venue.City = city;
        venue.Address = address;
        venue.Capacity = capacity;
        return await _venueRepository.UpdateAsync(venue);
    }

    public async Task<Venue> DeleteByIdAsync(Guid id)
    {
        var venue = await GetByIdNotNullAsync(id);
        return await _venueRepository.DeleteAsync(venue);
    }

    public async Task<PaginatedResult<Venue>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _venueRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: x => x.OrderBy(v => v.Name),
            asNoTracking: true);
    }
}
