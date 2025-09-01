using System.Text.Json;
using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using ApiAggregator.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace ApiAggregator.Infrastructure.Services;

public class OpenMeteoService : IOpenMeteoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoService> _logger;

    public OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        var coords = CityCoordinates.GetCoordinates(city);
        if (coords == null)
        {
            _logger.LogWarning("Coordinates not found for city: {City}", city);
            throw new ArgumentException($"Coordinates not found for city: {city}");
        }

        var url = $"https://api.open-meteo.com/v1/forecast?latitude={coords.Value.Latitude}&longitude={coords.Value.Longitude}&current=temperature_2m,wind_speed_10m";

        try
        {
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
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for city {City} at URL {Url}", city, url);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing failed for weather data for city {City}.", city);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching weather data for city {City}.", city);
            throw;
        }
    }
}
