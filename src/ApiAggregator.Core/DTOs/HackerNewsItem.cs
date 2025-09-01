using ApiAggregator.Core.Enums;
using System.Text.Json.Serialization;

namespace ApiAggregator.Core.DTOs;

public record HackerNewsItem
{
    [JsonPropertyName("by")] public string? By { get; set; }
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("parent")] public int Parent { get; set; }
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("time")] public long Time { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }

    public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime.ToUniversalTime();
    public Category Category => Category.HackerNews;
}