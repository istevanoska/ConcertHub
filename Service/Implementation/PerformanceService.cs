using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class PerformanceService : IPerformanceService
{
    private readonly IRepository<Performance> _performanceRepository;
    private readonly IArtistService _artistService;
    private readonly IConcertService _concertService;
    private readonly IFavoriteArtistService _favoriteArtistService;

    public PerformanceService(
        IRepository<Performance> performanceRepository,
        IArtistService artistService,
        IConcertService concertService,
        IFavoriteArtistService favoriteArtistService)
    {
        _performanceRepository = performanceRepository;
        _artistService = artistService;
        _concertService = concertService;
        _favoriteArtistService = favoriteArtistService;
    }

    public async Task<Performance> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Performance with id {id} not found");
        return result;
    }

    public async Task<Performance?> GetByIdAsync(Guid id)
    {
        return await _performanceRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id,
            include: x => x.Include(p => p.Artist));
    }

    public async Task<List<Performance>> GetAllByConcertIdAsync(Guid concertId)
    {
        return await _performanceRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.ConcertId == concertId,
            orderBy: x => x.OrderBy(p => p.SlotOrder),
            include: x => x.Include(p => p.Artist));
    }

    public async Task<Performance> CreateAsync(Guid artistId, Guid concertId, int slotOrder, int durationMinutes)
    {

        var artist = await _artistService.GetByIdNotNullAsync(artistId);
        var concert = await _concertService.GetByIdNotNullAsync(concertId);

        var performance = new Performance
        {
            ArtistId = artistId,
            ConcertId = concertId,
            SlotOrder = slotOrder,
            DurationMinutes = durationMinutes
        };
        var result = await _performanceRepository.InsertAsync(performance);

        await _favoriteArtistService.NotifyFollowersAsync(artist, concert);

        return result;
    }

    public async Task<Performance> UpdateAsync(Guid id, Guid artistId, Guid concertId, int slotOrder, int durationMinutes)
    {
        var performance = await GetByIdNotNullAsync(id);

        await _artistService.GetByIdNotNullAsync(artistId);
        await _concertService.GetByIdNotNullAsync(concertId);

        performance.ArtistId = artistId;
        performance.ConcertId = concertId;
        performance.SlotOrder = slotOrder;
        performance.DurationMinutes = durationMinutes;
        return await _performanceRepository.UpdateAsync(performance);
    }

    public async Task<Performance> DeleteByIdAsync(Guid id)
    {
        var performance = await GetByIdNotNullAsync(id);
        return await _performanceRepository.DeleteAsync(performance);
    }

    public async Task<PaginatedResult<Performance>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _performanceRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(p => p.Artist),
            orderBy: x => x.OrderBy(p => p.SlotOrder),
            asNoTracking: true);
    }
}
