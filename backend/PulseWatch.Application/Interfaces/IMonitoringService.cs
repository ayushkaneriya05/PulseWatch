namespace PulseWatch.Application.Interfaces;

public interface IMonitoringService
{
    Task ExecuteMonitoringCycleAsync(CancellationToken cancellationToken);
}
