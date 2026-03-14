using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PulseWatch.Application.Interfaces;
using PulseWatch.Domain.Entities;
using PulseWatch.Infrastructure.Data;

namespace PulseWatch.Infrastructure.Services;

public class MonitoringService : IMonitoringService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        ILogger<MonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task ExecuteMonitoringCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PulseWatchDbContext>();

        var endpoints = await context.ApiEndpoints.ToListAsync(cancellationToken);

        foreach (var endpoint in endpoints)
        {
            if (cancellationToken.IsCancellationRequested) break;

            // Check if enough time has passed since last check
            var lastLog = await context.MonitoringLogs
                .Where(m => m.ApiEndpointId == endpoint.Id)
                .OrderByDescending(m => m.CheckedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastLog != null)
            {
                var elapsed = DateTime.UtcNow - lastLog.CheckedAt;
                if (elapsed.TotalSeconds < endpoint.CheckIntervalSeconds)
                    continue; // Not time yet
            }

            // Execute the health check
            var log = await CheckEndpointAsync(endpoint, cancellationToken);
            context.MonitoringLogs.Add(log);

            // Create alert if failure
            if (!log.IsSuccess)
            {
                var alert = new AlertRecord
                {
                    Id = Guid.NewGuid(),
                    ApiEndpointId = endpoint.Id,
                    AlertType = "Failure",
                    Message = $"API '{endpoint.Name}' returned status {log.StatusCode}. {log.ErrorMessage}",
                    CreatedAt = DateTime.UtcNow
                };
                context.AlertRecords.Add(alert);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<MonitoringLog> CheckEndpointAsync(ApiEndpoint endpoint, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("MonitorClient");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var url = NormalizeUrl(endpoint.Url);
            var request = new HttpRequestMessage(
                new HttpMethod(endpoint.HttpMethod),
                url);

            var response = await client.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var isSuccess = (int)response.StatusCode == endpoint.ExpectedStatusCode;

            return new MonitoringLog
            {
                Id = Guid.NewGuid(),
                ApiEndpointId = endpoint.Id,
                StatusCode = (int)response.StatusCode,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                IsSuccess = isSuccess,
                ErrorMessage = isSuccess ? null : $"Expected {endpoint.ExpectedStatusCode}, got {(int)response.StatusCode}",
                CheckedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error monitoring endpoint {Name} ({Url})", endpoint.Name, endpoint.Url);

            return new MonitoringLog
            {
                Id = Guid.NewGuid(),
                ApiEndpointId = endpoint.Id,
                StatusCode = 0,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                IsSuccess = false,
                ErrorMessage = ex.Message,
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "http://" + url;
        }
        return url;
    }
}
