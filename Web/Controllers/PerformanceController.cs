using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceController : ControllerBase
{
    private readonly PerformanceMapper _mapper;

    public PerformanceController(PerformanceMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("paged")]
    public async Task<PaginatedResponse<PerformanceResponse>> GetPagedAsync([FromQuery] PaginatedRequest request)
        => await _mapper.GetAllPaginatedAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpGet("concert/{concertId}")]
    public async Task<IActionResult> GetByConcertAsync(Guid concertId)
        => Ok(await _mapper.GetAllByConcertIdAsync(concertId));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PerformanceRequest request)
        => Ok(await _mapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] PerformanceRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _mapper.DeleteAsync(id));
}
