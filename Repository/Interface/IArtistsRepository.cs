using Domain.Models;

namespace Repository.Interface;

public interface IArtistsRepository
{
    Task BulkInsertOrUpdateAsync(List<Artist> artists);
}
