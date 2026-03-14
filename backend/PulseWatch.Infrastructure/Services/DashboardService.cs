using Microsoft.EntityFrameworkCore;
using PulseWatch.Application.DTOs.Dashboard;
using PulseWatch.Application.Interfaces;
using PulseWatch.Infrastructure.Data;

namespace PulseWatch.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly PulseWatchDbContext _context;

    public DashboardService(PulseWatchDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid userId)
    {
        var apiIds = await _context.ApiEndpoints
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var totalApis = apiIds.Count;
        int healthyCount = 0;
        int unhealthyCount = 0;
        double totalAvgResponse = 0;
        int apisWithLogs = 0;

        foreach (var apiId in apiIds)
        {
            var lastLog = await _context.MonitoringLogs
                .Where(m => m.ApiEndpointId == apiId)
                .OrderByDescending(m => m.CheckedAt)
                .FirstOrDefaultAsync();

            if (lastLog != null)
            {
                if (lastLog.IsSuccess)
                    healthyCount++;
                else
                    unhealthyCount++;

                var avg = await _context.MonitoringLogs
                    .Where(m => m.ApiEndpointId == apiId && m.CheckedAt >= DateTime.UtcNow.AddHours(-24))
                    .AverageAsync(m => (double?)m.ResponseTimeMs) ?? 0;

                totalAvgResponse += avg;
                apisWithLogs++;
            }
        }

        return new DashboardSummaryResponse
        {
            TotalApis = totalApis,
            HealthyApis = healthyCount,
            UnhealthyApis = unhealthyCount,
            AverageResponseTimeMs = apisWithLogs > 0 ? Math.Round(totalAvgResponse / apisWithLogs, 2) : 0
        };
    }

    public async Task<ApiDetailResponse?> GetApiDetailAsync(Guid apiId, Guid userId)
    {
        var api = await _context.ApiEndpoints
            .FirstOrDefaultAsync(a => a.Id == apiId && a.UserId == userId);

        if (api == null) return null;

        var logs = await _context.MonitoringLogs
            .Where(m => m.ApiEndpointId == apiId)
            .OrderByDescending(m => m.CheckedAt)
            .Take(100)
            .ToListAsync();

        var totalChecks = logs.Count;
        var successChecks = logs.Count(l => l.IsSuccess);
        var uptimePercentage = totalChecks > 0 ? Math.Round((double)successChecks / totalChecks * 100, 2) : 100;
        var avgResponseTime = logs.Count > 0 ? Math.Round(logs.Average(l => (double)l.ResponseTimeMs), 2) : 0;

        var lastLog = logs.FirstOrDefault();

        return new ApiDetailResponse
        {
            ApiId = api.Id,
            Name = api.Name,
            Url = api.Url,
            UptimePercentage = uptimePercentage,
            AvgResponseTimeMs = avgResponseTime,
            LastStatus = lastLog != null ? (lastLog.IsSuccess ? "Healthy" : "Unhealthy") : "Pending",
            LastCheckedAt = lastLog?.CheckedAt,
            RecentLogs = logs.Select(l => new MonitoringLogDto
            {
                StatusCode = l.StatusCode,
                ResponseTimeMs = l.ResponseTimeMs,
                IsSuccess = l.IsSuccess,
                ErrorMessage = l.ErrorMessage,
                CheckedAt = l.CheckedAt
            }).ToList()
        };
    }

    public async Task<List<MonitoringLogDto>> GetApiLogsAsync(Guid apiId, Guid userId, int hours = 24)
    {
        // Verify the API belongs to the user
        var api = await _context.ApiEndpoints
            .FirstOrDefaultAsync(a => a.Id == apiId && a.UserId == userId);

        if (api == null) return new List<MonitoringLogDto>();

        var since = DateTime.UtcNow.AddHours(-hours);

        return await _context.MonitoringLogs
            .Where(m => m.ApiEndpointId == apiId && m.CheckedAt >= since)
            .OrderByDescending(m => m.CheckedAt)
            .Select(m => new MonitoringLogDto
            {
                StatusCode = m.StatusCode,
                ResponseTimeMs = m.ResponseTimeMs,
                IsSuccess = m.IsSuccess,
                ErrorMessage = m.ErrorMessage,
                CheckedAt = m.CheckedAt
            })
            .ToListAsync();
    }
}
