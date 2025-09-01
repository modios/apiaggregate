using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace ApiAggregator.Infrastructure.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsService> _logger;
    private readonly IMemoryCache _cache;
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";

    public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<HackerNewsItem> GetItemByIdAsync(int id)
    {
        string cacheKey = $"hackernews:item:{id}";

        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is HackerNewsItem cachedItem)
        {
            _logger.LogInformation("Returning cached HackerNews item with ID {Id}", id);
            return cachedItem;
        }

        var url = $"{BaseUrl}/item/{id}.json";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string rawJson = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<HackerNewsItem>(rawJson);

            if (item == null)
            {
                _logger.LogWarning("Deserialization failed for HackerNews item with ID {Id}. Raw JSON: {Json}", id, rawJson);
                throw new InvalidOperationException($"Item with id {id} not found or could not be deserialized.");
            }

            _cache.Set(cacheKey, item, TimeSpan.FromMinutes(10));
            return item;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for HackerNews item with ID {Id}", id);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed for HackerNews item with ID {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching HackerNews item with ID {Id}", id);
            throw;
        }
    }
}
