using ApiAggregator.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ApiAggregator.Tests;

public class WorldBankCountryServiceTests
{
    private Mock<HttpMessageHandler> _httpHandlerMock;
    private HttpClient _httpClient;
    private Mock<ILogger<WorldBankCountryService>> _loggerMock;
    private IMemoryCache _memoryCache;

    public WorldBankCountryServiceTests()
    {
        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object);
        _loggerMock = new Mock<ILogger<WorldBankCountryService>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetCountryAsync_FetchesFromApi_WhenNotInCache()
    {
        // Arrange
        var iso2Code = "GR";
        var json = """
        [
            {},
            [
                {
                    "id": "GR",
                    "iso2Code": "GR",
                    "name": "Greece",
                    "region": { "value": "Europe & Central Asia" },
                    "adminregion": { "value": "" },
                    "incomeLevel": { "value": "High income" },
                    "lendingType": { "value": "IBRD" },
                    "capitalCity": "Athens",
                    "latitude": "37.98",
                    "longitude": "23.73"
                }
            ]
        ]
        """;

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

        var service = new WorldBankCountryService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act
        var result = await service.GetCountryAsync(iso2Code);

        // Assert
        Assert.Equal("Greece", result.Name);
        Assert.Equal("Athens", result.CapitalCity);
    }

    [Fact]
    public async Task GetCountryAsync_ThrowsHttpRequestException_OnHttpError()
    {
        // Arrange
        var iso2Code = "GR";

        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        _httpHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        var service = new WorldBankCountryService(_httpClient, _loggerMock.Object, _memoryCache);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCountryAsync(iso2Code));
    }
}
