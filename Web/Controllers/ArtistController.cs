using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly ArtistMapper _mapper;

    public ArtistController(ArtistMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<ArtistResponse>> GetAllAsync([FromQuery] string? name, [FromQuery] Genre? genre)
        => await _mapper.GetAllAsync(name, genre);

    [HttpGet("paged")]
    public async Task<PaginatedResponse<ArtistResponse>> GetPagedAsync([FromQuery] PaginatedRequest request)
        => await _mapper.GetAllPaginatedAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ArtistRequest request)
        => Ok(await _mapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ArtistRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _mapper.DeleteAsync(id));
}
