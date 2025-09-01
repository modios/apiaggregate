using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Helpers;
using ApiAggregator.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ApiAggregator.Infrastructure.Services;

public class OpenMeteoService : IOpenMeteoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenMeteoService> _logger;
    private readonly IMemoryCache _cache;

    public OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        string cacheKey = $"weather:{city}";

        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is WeatherInfo cached)
        {
            _logger.LogInformation("Returning cached weather data for city {City}", city);
            return cached;
        }

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

            var weather = new WeatherInfo(
                City: city,
                Temperature: current.GetProperty("temperature_2m").GetDouble(),
                WindSpeed: current.GetProperty("wind_speed_10m").GetDouble()
            );

            _cache.Set(cacheKey, weather, TimeSpan.FromMinutes(10));
            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather data for city {City}", city);
            throw;
        }
    }
}
