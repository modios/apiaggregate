using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Text.Json;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";

    public HackerNewsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HackerNewsItem> GetItemByIdAsync(int id)
    {
        var url = $"{BaseUrl}/item/{id}.json";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string rawJson = await response.Content.ReadAsStringAsync();
        var item = JsonSerializer.Deserialize<HackerNewsItem>(rawJson);

        if (item == null)
            throw new InvalidOperationException($"Item with id {id} not found or could not be deserialized.");

        return item;
    }
}