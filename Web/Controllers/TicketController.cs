using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly TicketMapper _mapper;

    public TicketController(TicketMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("paged")]
    public async Task<PaginatedResponse<TicketResponse>> GetPagedAsync([FromQuery] PaginatedRequest request)
        => await _mapper.GetAllPaginatedAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpGet("concert/{concertId}")]
    public async Task<IActionResult> GetByConcertAsync(Guid concertId)
        => Ok(await _mapper.GetAllByConcertIdAsync(concertId));

    [HttpPost("buy")]
    public async Task<IActionResult> BuyAsync([FromBody] TicketRequest request)
        => Ok(await _mapper.BuyAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] TicketRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        await _mapper.DeleteAsync(id);
        return Ok();
    }

    [HttpPatch("{id}/pay")]
    public async Task<IActionResult> PayAsync(Guid id)
        => Ok(await _mapper.MarkAsPaidAsync(id));

    [HttpPatch("{id}/check-in")]
    public async Task<IActionResult> CheckInAsync(Guid id)
        => Ok(await _mapper.CheckInAsync(id));

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAsync(Guid id)
        => Ok(await _mapper.CancelAsync(id));

    [HttpPost("{id}/refund-document")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadRefundDocumentAsync([FromRoute] Guid id, IFormFile file)
        => Ok(await _mapper.UploadRefundDocumentAsync(id, file));
}
