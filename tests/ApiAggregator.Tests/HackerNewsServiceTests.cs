using ApiAggregator.Core.DTOs;
using ApiAggregator.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ApiAggregator.Infustructure.Tests;

public class HackerNewsServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<HackerNewsService>> _loggerMock;
    private readonly IMemoryCache _memoryCache;

    public HackerNewsServiceTests()
    {
        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object);
        _loggerMock = new Mock<ILogger<HackerNewsService>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetItemByIdAsync_ReturnsFromCache_WhenAvailable()
    {
        // Arrange
        var id = 123;
        var expected = new HackerNewsItem { Id = id, Text = "Cached Item" };
        _memoryCache.Set($"hackernews:item:{id}", expected);

        var service = new HackerNewsService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act
        var result = await service.GetItemByIdAsync(id);

        // Assert
        Assert.Equal(expected.Text, result.Text);
    }

    [Fact]
    public async Task GetItemByIdAsync_FetchesFromApi_WhenNotInCache()
    {
        // Arrange
        var id = 456;
        var json = JsonSerializer.Serialize(new HackerNewsItem
        {
            Id = id,
            Text = "API Item",
            By = "user"
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        var service = new HackerNewsService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act
        var result = await service.GetItemByIdAsync(id);

        // Assert
        Assert.Equal("API Item", result.Text);
        Assert.Equal("user", result.By);
    }

    [Fact]
    public async Task GetItemByIdAsync_ThrowsInvalidOperationException_WhenDeserializationFails()
    {
        // Arrange
        var id = 789;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        var service = new HackerNewsService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetItemByIdAsync(id));
    }

    [Fact]
    public async Task GetItemByIdAsync_ThrowsHttpRequestException_OnHttpError()
    {
        // Arrange
        var id = 999;
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        var service = new HackerNewsService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetItemByIdAsync(id));
    }
}
