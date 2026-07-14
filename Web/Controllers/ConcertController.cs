using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConcertController : ControllerBase
{
    private readonly ConcertMapper _mapper;

    public ConcertController(ConcertMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<ConcertResponse>> GetAllAsync([FromQuery] string? venueName, [FromQuery] DateOnly? date)
        => await _mapper.GetAllAsync(venueName, date);

    [HttpGet("paged")]
    public async Task<PaginatedResponse<ConcertResponse>> GetPagedAsync([FromQuery] PaginatedRequest request)
        => await _mapper.GetAllPaginatedAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ConcertRequest request)
        => Ok(await _mapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ConcertRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _mapper.DeleteAsync(id));
}
