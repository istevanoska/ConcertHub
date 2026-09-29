using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/favorite/artist")]
public class FavoriteController : ControllerBase
{
    private readonly FavoriteMapper _mapper;

    public FavoriteController(FavoriteMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] FavoriteRequest request)
    {
        await _mapper.AddAsync(request);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveAsync([FromBody] FavoriteRequest request)
    {
        await _mapper.RemoveAsync(request);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetFavoritesAsync(string userId)
        => Ok(await _mapper.GetFavoriteArtistsAsync(userId));
}
