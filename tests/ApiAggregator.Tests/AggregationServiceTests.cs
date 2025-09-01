using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Enums;
using ApiAggregator.Core.Interfaces;
using ApiAggregator.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiAggregator.Infrastructure.Tests;

public class AggregationServiceTests
{
    private readonly Mock<IOpenMeteoService> _weatherMock;
    private readonly Mock<IHackerNewsService> _newsMock;
    private readonly Mock<IWorldBankCountryService> _countryMock;
    private readonly Mock<ILogger<AggregationService>> _loggerMock;
    private readonly AggregationService _service;

    public AggregationServiceTests()
    {
        _weatherMock = new Mock<IOpenMeteoService>();
        _newsMock = new Mock<IHackerNewsService>();
        _countryMock = new Mock<IWorldBankCountryService>();
        _loggerMock = new Mock<ILogger<AggregationService>>();

        _service = new AggregationService(
            _weatherMock.Object,
            _newsMock.Object,
            _countryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ReturnsAllItems_WhenNoFilter()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var request = new AggregationRequest
        {
            City = "Athens",
            CountryCode = "GR",
            HackerNewsItemId = 123
        };

        var weather = new WeatherInfo("Athens", 25.0, 5.0);

        var news = new HackerNewsItem
        {
            Id = 123,
            By = "user123",
            Parent = 0,
            Text = "Sample news text",
            Time = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Type = "story"
        };

        var country = new WorldBankCountry("GR", "GR", "Greece", "Europe", "", "High income", "IBRD", "Athens", 37.98, 23.73)
        {
            LastUpdated = now.AddDays(-1)
        };

        _weatherMock.Setup(s => s.GetWeatherAsync("Athens")).ReturnsAsync(weather);
        _newsMock.Setup(s => s.GetItemByIdAsync(123)).ReturnsAsync(news);
        _countryMock.Setup(s => s.GetCountryAsync("GR")).ReturnsAsync(country);

        // Act
        var result = await _service.GetAggregatedDataAsync(request);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Contains(result.Items, i => i.RawData.Equals(weather));
        Assert.Contains(result.Items, i => i.RawData.Equals(news));
        Assert.Contains(result.Items, i => i.RawData.Equals(country));
    }

    [Fact]
    public async Task GetAggregatedDataAsync_FiltersByCategory()
    {
        // Arrange
        var request = new AggregationRequest
        {
            City = "Athens",
            CountryCode = "GR",
            HackerNewsItemId = 123,
            Category = Category.HackerNews
        };

        var weather = new WeatherInfo("Athens", 25.0, 5.0);  
        var news = new HackerNewsItem
        {
            Id = 123,
            By = "user123",
            Parent = 0,
            Text = "Sample news text",
            Time = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Type = "story"
        }; 
        var country = new WorldBankCountry("GR", "GR", "Greece", "Europe", "", "High income", "IBRD", "Athens", 37.98, 23.73)
        {
            LastUpdated = DateTime.UtcNow
        };

        _weatherMock.Setup(s => s.GetWeatherAsync("Athens")).ReturnsAsync(weather);
        _newsMock.Setup(s => s.GetItemByIdAsync(123)).ReturnsAsync(news);
        _countryMock.Setup(s => s.GetCountryAsync("GR")).ReturnsAsync(country);

        // Act
        var result = await _service.GetAggregatedDataAsync(request);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(Category.HackerNews, result.Items[0].Category);
    }

    [Fact]
    public async Task GetAggregatedDataAsync_SortsByDateDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var request = new AggregationRequest
        {
            City = "Athens",
            CountryCode = "GR",
            HackerNewsItemId = 123,
            SortBy = SortBy.Date,
            SortOrder = SortOrder.Desc
        };

        var weather = new WeatherInfo("Athens", 25.0, 5.0);

        var news = new HackerNewsItem
        {
            Id = 123,
            By = "user123",
            Parent = 0,
            Text = "Sample news text",
            Time = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds(),
            Type = "story"
        };

        var country = new WorldBankCountry("GR", "GR", "Greece", "Europe", "", "High income", "IBRD", "Athens", 37.98, 23.73)
        {
            LastUpdated = now.AddDays(-1)
        };

        _weatherMock.Setup(s => s.GetWeatherAsync("Athens")).ReturnsAsync(weather);
        _newsMock.Setup(s => s.GetItemByIdAsync(123)).ReturnsAsync(news);
        _countryMock.Setup(s => s.GetCountryAsync("GR")).ReturnsAsync(country);

        // Act
        var result = await _service.GetAggregatedDataAsync(request);

        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.True(result.Items[0].Date >= result.Items[1].Date);
        Assert.True(result.Items[1].Date >= result.Items[2].Date);
    }

    [Fact]
    public async Task GetAggregatedDataAsync_ThrowsException_WhenServiceFails()
    {
        // Arrange
        var request = new AggregationRequest
        {
            City = "Athens",
            CountryCode = "GR",
            HackerNewsItemId = 123
        };

        _weatherMock.Setup(s => s.GetWeatherAsync("Athens")).ThrowsAsync(new Exception("Weather API failed"));
        _newsMock.Setup(s => s.GetItemByIdAsync(123)).ReturnsAsync(new HackerNewsItem());
        _countryMock.Setup(s => s.GetCountryAsync("GR")).ReturnsAsync(new WorldBankCountry("GR", "GR", "Greece", "Europe", "", "High income", "IBRD", "Athens", 37.98, 23.73));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAggregatedDataAsync(request));
    }
}
