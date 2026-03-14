using Microsoft.EntityFrameworkCore;
using PulseWatch.Application.DTOs.ApiEndpoint;
using PulseWatch.Application.Interfaces;
using PulseWatch.Domain.Entities;
using PulseWatch.Infrastructure.Data;

namespace PulseWatch.Infrastructure.Services;

public class ApiEndpointService : IApiEndpointService
{
    private readonly PulseWatchDbContext _context;

    public ApiEndpointService(PulseWatchDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApiEndpointResponse>> GetAllAsync(Guid userId)
    {
        var endpoints = await _context.ApiEndpoints
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        var responses = new List<ApiEndpointResponse>();
        foreach (var ep in endpoints)
        {
            var lastLog = await _context.MonitoringLogs
                .Where(m => m.ApiEndpointId == ep.Id)
                .OrderByDescending(m => m.CheckedAt)
                .FirstOrDefaultAsync();

            responses.Add(MapToResponse(ep, lastLog));
        }

        return responses;
    }

    public async Task<ApiEndpointResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var ep = await _context.ApiEndpoints
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (ep == null) return null;

        var lastLog = await _context.MonitoringLogs
            .Where(m => m.ApiEndpointId == ep.Id)
            .OrderByDescending(m => m.CheckedAt)
            .FirstOrDefaultAsync();

        return MapToResponse(ep, lastLog);
    }

    public async Task<ApiEndpointResponse> CreateAsync(CreateApiRequest request, Guid userId)
    {
        var endpoint = new ApiEndpoint
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Url = request.Url,
            HttpMethod = request.HttpMethod,
            CheckIntervalSeconds = request.CheckIntervalSeconds,
            ExpectedStatusCode = request.ExpectedStatusCode,
            Description = request.Description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ApiEndpoints.Add(endpoint);
        await _context.SaveChangesAsync();

        return MapToResponse(endpoint, null);
    }

    public async Task<ApiEndpointResponse?> UpdateAsync(Guid id, UpdateApiRequest request, Guid userId)
    {
        var ep = await _context.ApiEndpoints
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (ep == null) return null;

        ep.Name = request.Name;
        ep.Url = request.Url;
        ep.HttpMethod = request.HttpMethod;
        ep.CheckIntervalSeconds = request.CheckIntervalSeconds;
        ep.ExpectedStatusCode = request.ExpectedStatusCode;
        ep.Description = request.Description;

        await _context.SaveChangesAsync();

        var lastLog = await _context.MonitoringLogs
            .Where(m => m.ApiEndpointId == ep.Id)
            .OrderByDescending(m => m.CheckedAt)
            .FirstOrDefaultAsync();

        return MapToResponse(ep, lastLog);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var ep = await _context.ApiEndpoints
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (ep == null) return false;

        _context.ApiEndpoints.Remove(ep);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ApiEndpointResponse MapToResponse(ApiEndpoint ep, MonitoringLog? lastLog)
    {
        return new ApiEndpointResponse
        {
            Id = ep.Id,
            Name = ep.Name,
            Url = ep.Url,
            HttpMethod = ep.HttpMethod,
            CheckIntervalSeconds = ep.CheckIntervalSeconds,
            ExpectedStatusCode = ep.ExpectedStatusCode,
            Description = ep.Description,
            CreatedAt = ep.CreatedAt,
            LastStatus = lastLog != null ? (lastLog.IsSuccess ? "Healthy" : "Unhealthy") : "Pending",
            LastResponseTimeMs = lastLog?.ResponseTimeMs,
            LastCheckedAt = lastLog?.CheckedAt
        };
    }
}
