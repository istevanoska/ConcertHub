using System.Text.Json;
using Domain.Dto;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/external/tickets")]
public class InboundController : ControllerBase
{
    private readonly IInboundEventEntryService _service;

    public InboundController(IInboundEventEntryService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    [EnableRateLimiting("external-api")]
    public async Task<IActionResult> ReceiveEvent([FromBody] TicketRequestDto request)
    {
        var payload = JsonSerializer.Serialize(request);
        var apiClientId = HttpContext.Items["ApiClientId"] as Guid? ?? Guid.Empty;

        var entry = await _service.CreateAsync(payload, apiClientId);

        return Accepted(new
        {
            id = entry.Id,
            status = InboundEventStatus.Pending.ToString().ToLower()
        });
    }

    [HttpGet("register/{id}/status")]
    [EnableRateLimiting("external-api")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var entry = await _service.GetByIdNotNull(id);

        return Ok(new
        {
            id = entry.Id,
            status = entry.Status.ToString().ToLower(),
            ticketId = entry.CreatedTicketId,
            error = entry.ErrorMessage
        });
    }
}
