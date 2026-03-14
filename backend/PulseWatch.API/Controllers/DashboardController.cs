using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.DTOs.Dashboard;
using PulseWatch.Application.Interfaces;

namespace PulseWatch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
    {
        var result = await _dashboardService.GetSummaryAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("api/{id:guid}")]
    public async Task<ActionResult<ApiDetailResponse>> GetApiDetail(Guid id)
    {
        var result = await _dashboardService.GetApiDetailAsync(id, GetUserId());
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("api/{id:guid}/logs")]
    public async Task<ActionResult<List<MonitoringLogDto>>> GetApiLogs(Guid id, [FromQuery] int hours = 24)
    {
        var result = await _dashboardService.GetApiLogsAsync(id, GetUserId(), hours);
        return Ok(result);
    }
}
