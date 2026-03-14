using PulseWatch.Application.DTOs.Dashboard;

namespace PulseWatch.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(Guid userId);
    Task<ApiDetailResponse?> GetApiDetailAsync(Guid apiId, Guid userId);
    Task<List<MonitoringLogDto>> GetApiLogsAsync(Guid apiId, Guid userId, int hours = 24);
}
