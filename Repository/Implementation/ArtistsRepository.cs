using Domain.Models;
using EFCore.BulkExtensions;
using Repository.Interface;

namespace Repository.Implementation;

public class ArtistsRepository : Repository<Artist>, IArtistsRepository
{
    public ArtistsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task BulkInsertOrUpdateAsync(List<Artist> artists)
    {
        await _context.BulkInsertOrUpdateAsync(artists);
    }
}
