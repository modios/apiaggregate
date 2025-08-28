using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Net.Http;
using System.Text.Json;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";

    public HackerNewsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HackerNewsItem?> GetItemByIdAsync(int id)
    {
        var url = $"{BaseUrl}/item/{id}.json";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        string rawJson = await response.Content.ReadAsStringAsync();
        var item = JsonSerializer.Deserialize<HackerNewsItem>(rawJson);
        return item;
    }
}