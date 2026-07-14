using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketCategoryController : ControllerBase
{
    private readonly TicketCategoryMapper _mapper;

    public TicketCategoryController(TicketCategoryMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<TicketCategoryResponse>> GetAllAsync()
        => await _mapper.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _mapper.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] TicketCategoryRequest request)
        => Ok(await _mapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] TicketCategoryRequest request)
        => Ok(await _mapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _mapper.DeleteAsync(id));
}
