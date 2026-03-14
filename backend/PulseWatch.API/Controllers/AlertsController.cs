using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseWatch.Infrastructure.Data;

namespace PulseWatch.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly PulseWatchDbContext _context;

    public AlertsController(PulseWatchDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var userId = GetUserId();

        var apiIds = await _context.ApiEndpoints
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var alerts = await _context.AlertRecords
            .Where(a => apiIds.Contains(a.ApiEndpointId) && !a.IsDismissed)
            .OrderByDescending(a => a.CreatedAt)
            .Take(50)
            .Select(a => new
            {
                a.Id,
                a.ApiEndpointId,
                a.AlertType,
                a.Message,
                a.IsDismissed,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(alerts);
    }

    [HttpPost("{id:guid}/dismiss")]
    public async Task<IActionResult> DismissAlert(Guid id)
    {
        var userId = GetUserId();

        var apiIds = await _context.ApiEndpoints
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var alert = await _context.AlertRecords
            .FirstOrDefaultAsync(a => a.Id == id && apiIds.Contains(a.ApiEndpointId));

        if (alert == null) return NotFound();

        alert.IsDismissed = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Alert dismissed." });
    }
}
