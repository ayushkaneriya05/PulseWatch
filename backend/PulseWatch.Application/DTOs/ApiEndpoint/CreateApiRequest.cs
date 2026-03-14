namespace PulseWatch.Application.DTOs.ApiEndpoint;

public class CreateApiRequest
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "GET";
    public int CheckIntervalSeconds { get; set; } = 60;
    public int ExpectedStatusCode { get; set; } = 200;
    public string? Description { get; set; }
}
