using System.Text.Json;
using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using ApiAggregator.Core.Helpers; // Assuming CityCoordinates lives here

namespace ApiAggregator.Infrastructure.Services;

public class OpenMeteoService : IOpenMeteoService
{
    private readonly HttpClient _httpClient;

    public OpenMeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        var coords = CityCoordinates.GetCoordinates(city);
        if (coords == null)
            throw new ArgumentException($"Coordinates not found for city: {city}");

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={coords.Value.Latitude}&longitude={coords.Value.Longitude}&current=temperature_2m,wind_speed_10m";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json);
        var current = data.RootElement.GetProperty("current");

        return new WeatherInfo(
            City: city,
            Temperature: current.GetProperty("temperature_2m").GetDouble(),
            WindSpeed: current.GetProperty("wind_speed_10m").GetDouble()
        );
    }
}
