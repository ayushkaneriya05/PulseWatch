namespace PulseWatch.Domain.Entities;

public class MonitoringLog
{
    public Guid Id { get; set; }
    public Guid ApiEndpointId { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    public ApiEndpoint ApiEndpoint { get; set; } = null!;
}
