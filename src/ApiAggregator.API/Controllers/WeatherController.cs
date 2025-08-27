using Microsoft.AspNetCore.Mvc;
using ApiAggregator.Core.Interfaces;
using ApiAggregator.Core.DTOs;

namespace ApiAggregator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IOpenMeteoService _weatherService;

    public WeatherController(IOpenMeteoService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Gets current weather data for a specified city.
    /// </summary>
    /// <param name="city">City name (e.g. Athens)</param>
    /// <returns>Weather information including temperature, humidity, and wind speed</returns>
    [HttpGet]
    public async Task<ActionResult<WeatherInfo>> GetWeather([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City parameter is required.");

        try
        {
            var result = await _weatherService.GetWeatherAsync(city);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, $"Weather service unavailable: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }
}
