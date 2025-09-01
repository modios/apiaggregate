using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsService> _logger;
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";

    public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HackerNewsItem> GetItemByIdAsync(int id)
    {
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
