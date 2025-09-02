namespace ApiAggregator.Core.Models;

public class RequestStats
{
    public int TotalRequests;
    public double TotalResponseTimeMs;
    public int FastRequests;
    public int AverageRequests;
    public int SlowRequests;
    public double AverageResponseTimeMs => TotalRequests == 0 ? 0 : TotalResponseTimeMs / TotalRequests;
}
