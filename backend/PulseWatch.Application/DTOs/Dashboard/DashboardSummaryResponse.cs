namespace PulseWatch.Application.DTOs.Dashboard;

public class DashboardSummaryResponse
{
    public int TotalApis { get; set; }
    public int HealthyApis { get; set; }
    public int UnhealthyApis { get; set; }
    public double AverageResponseTimeMs { get; set; }
}
