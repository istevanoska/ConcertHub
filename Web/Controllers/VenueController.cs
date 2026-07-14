using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly VenueMapper _mapper;

    public VenueController(VenueMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<VenueResponse>> GetAllAsync([FromQuery] string? city)
        => await _mapper.GetAllAsync(city);

    [HttpGet("paged")]
    public async Task<PaginatedResponse<VenueResponse>> GetPagedAsync([FromQuery] PaginatedRequest request)
        => await _mapper.GetAllPaginatedAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] VenueRequest request)
        => Ok(await _mapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] VenueRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _mapper.DeleteAsync(id));
}
