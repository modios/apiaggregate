namespace ApiAggregator.Core.DTOs;

public record WeatherInfo(string City, double Temperature, double WindSpeed)
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Category => "Environment";
}