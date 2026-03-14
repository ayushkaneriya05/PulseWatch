namespace PulseWatch.Application.DTOs.Dashboard;

public class ApiDetailResponse
{
    public Guid ApiId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public double UptimePercentage { get; set; }
    public double AvgResponseTimeMs { get; set; }
    public string? LastStatus { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public List<MonitoringLogDto> RecentLogs { get; set; } = new();
}

public class MonitoringLogDto
{
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}
