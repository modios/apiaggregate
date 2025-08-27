namespace ApiAggregator.Core.DTOs;

public record WeatherInfo(
    string City,
    double Temperature,
    double WindSpeed
);