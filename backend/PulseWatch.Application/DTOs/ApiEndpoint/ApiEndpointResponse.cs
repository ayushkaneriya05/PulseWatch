namespace PulseWatch.Application.DTOs.ApiEndpoint;

public class ApiEndpointResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; }
    public int ExpectedStatusCode { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Computed from latest log
    public string? LastStatus { get; set; }
    public long? LastResponseTimeMs { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}
