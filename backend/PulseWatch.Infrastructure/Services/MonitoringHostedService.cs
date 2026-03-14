using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PulseWatch.Application.Interfaces;

namespace PulseWatch.Infrastructure.Services;

public class MonitoringHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonitoringHostedService> _logger;
    private Timer? _timer;

    public MonitoringHostedService(
        IServiceProvider serviceProvider,
        ILogger<MonitoringHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("PulseWatch Monitoring Service started.");
        // Run every 15 seconds
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var monitoringService = scope.ServiceProvider.GetRequiredService<IMonitoringService>();
            await monitoringService.ExecuteMonitoringCycleAsync(CancellationToken.None);
            _logger.LogInformation("Monitoring cycle completed at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during monitoring cycle.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("PulseWatch Monitoring Service stopped.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
