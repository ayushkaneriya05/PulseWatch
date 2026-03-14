namespace PulseWatch.Domain.Entities;

public class AlertRecord
{
    public Guid Id { get; set; }
    public Guid ApiEndpointId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsDismissed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApiEndpoint ApiEndpoint { get; set; } = null!;
}
