namespace PulseWatch.Domain.Entities;

public class ApiEndpoint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "GET";
    public int CheckIntervalSeconds { get; set; } = 60;
    public int ExpectedStatusCode { get; set; } = 200;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<MonitoringLog> MonitoringLogs { get; set; } = new List<MonitoringLog>();
    public ICollection<AlertRecord> AlertRecords { get; set; } = new List<AlertRecord>();
}
