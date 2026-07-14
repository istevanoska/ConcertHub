using Domain.Config;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ConcertService : IConcertService
{
    private readonly IRepository<Concert> _concertRepository;
    private readonly IVenueService _venueService;
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _cacheSettings;

    public ConcertService(
        IRepository<Concert> concertRepository,
        IVenueService venueService,
        IMemoryCache cache,
        IOptions<CacheSettings> cacheSettings)
    {
        _concertRepository = concertRepository;
        _venueService = venueService;
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<Concert> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Concert with id {id} not found");
        return result;
    }

    public async Task<Concert?> GetByIdAsync(Guid id)
    {
        return await _concertRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id,
            include: x => x.Include(c => c.Venue));
    }

    public async Task<List<Concert>> GetAllAsync(string? venueName, DateOnly? date)
    {
        var cacheKey = $"concerts:{venueName}:{date:yyyy-MM-dd}";

        if (_cache.TryGetValue(cacheKey, out List<Concert>? cached) && cached != null)
        {
            return cached;
        }

        var results = await _concertRepository.GetAllAsync(
            selector: x => x,
            predicate: x => (venueName == null || x.Venue.Name.Contains(venueName)) &&
                            (date == null || DateOnly.FromDateTime(x.StartTime) == date),
            orderBy: x => x.OrderBy(c => c.StartTime),
            include: x => x.Include(c => c.Venue));

        _cache.Set(cacheKey, results, TimeSpan.FromMinutes(_cacheSettings.ListCacheDurationMinutes));

        return results;
    }

    public async Task<Concert> CreateAsync(string title, DateTime startTime, DateTime endTime, Guid venueId, decimal basePrice)
    {
        if (endTime <= startTime)
            throw new InvalidOperationException("Concert end time must be after start time.");

        await _venueService.GetByIdNotNullAsync(venueId);

        var concert = new Concert
        {
            Title = title,
            StartTime = startTime,
            EndTime = endTime,
            VenueId = venueId,
            BasePrice = basePrice,
            TicketsSold = 0
        };

        return await _concertRepository.InsertAsync(concert);
    }

    public async Task<Concert> UpdateAsync(Guid id, string title, DateTime startTime, DateTime endTime, Guid venueId, decimal basePrice)
    {
        var concert = await GetByIdNotNullAsync(id);

        if (concert.TicketsSold > 0)
            throw new InvalidOperationException($"Concert {id} already has sold tickets and cannot be modified.");

        if (endTime <= startTime)
            throw new InvalidOperationException("Concert end time must be after start time.");

        await _venueService.GetByIdNotNullAsync(venueId);

        concert.Title = title;
        concert.StartTime = startTime;
        concert.EndTime = endTime;
        concert.VenueId = venueId;
        concert.BasePrice = basePrice;
        return await _concertRepository.UpdateAsync(concert);
    }

    public async Task<Concert> DeleteByIdAsync(Guid id)
    {
        var concert = await GetByIdNotNullAsync(id);

        if (concert.TicketsSold > 0)
            throw new InvalidOperationException($"Concert {id} already has sold tickets and cannot be deleted.");

        return await _concertRepository.DeleteAsync(concert);
    }

    public async Task<PaginatedResult<Concert>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _concertRepository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(c => c.Venue),
            orderBy: x => x.OrderBy(c => c.StartTime),
            asNoTracking: true);
    }

    public async Task IncrementTicketsSoldAsync(Guid id)
    {
        var concert = await GetByIdNotNullAsync(id);
        concert.TicketsSold += 1;
        await _concertRepository.UpdateAsync(concert);
    }

    public async Task DecrementTicketsSoldAsync(Guid id)
    {
        var concert = await GetByIdNotNullAsync(id);
        if (concert.TicketsSold > 0)
        {
            concert.TicketsSold -= 1;
            await _concertRepository.UpdateAsync(concert);
        }
    }

    public async Task<List<RevenueReportDto>> GetRevenueReportAsync()
    {
        return await _concertRepository.GetAllAsync(
            selector: c => new RevenueReportDto
            {
                ConcertId = c.Id,
                Title = c.Title,
                VenueName = c.Venue.Name,
                StartTime = c.StartTime,
                TicketsSold = c.TicketsSold,
                VenueCapacity = c.Venue.Capacity,
                TotalRevenue = c.Tickets
                    .Where(t => t.Status != Domain.Enums.TicketStatus.Cancelled)
                    .Sum(t => (decimal?)t.Price) ?? 0m
            },
            orderBy: x => x.OrderByDescending(c => c.StartTime),
            include: x => x.Include(c => c.Venue).Include(c => c.Tickets));
    }
}
