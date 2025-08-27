namespace ApiAggregator.Core.Interfaces;

using ApiAggregator.Core.DTOs;

public interface IOpenMeteoService
{
    Task<WeatherInfo> GetWeatherAsync(string city);
}