using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.DTOs.ApiEndpoint;
using PulseWatch.Application.Interfaces;

namespace PulseWatch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EndpointsController : ControllerBase
{
    private readonly IApiEndpointService _apiEndpointService;

    public EndpointsController(IApiEndpointService apiEndpointService)
    {
        _apiEndpointService = apiEndpointService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<ActionResult<List<ApiEndpointResponse>>> GetAll()
    {
        var result = await _apiEndpointService.GetAllAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiEndpointResponse>> GetById(Guid id)
    {
        var result = await _apiEndpointService.GetByIdAsync(id, GetUserId());
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiEndpointResponse>> Create([FromBody] CreateApiRequest request)
    {
        var result = await _apiEndpointService.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiEndpointResponse>> Update(Guid id, [FromBody] UpdateApiRequest request)
    {
        var result = await _apiEndpointService.UpdateAsync(id, request, GetUserId());
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _apiEndpointService.DeleteAsync(id, GetUserId());
        if (!success) return NotFound();
        return NoContent();
    }
}
