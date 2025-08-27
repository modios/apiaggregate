using System.Text.Json.Serialization;

namespace ApiAggregator.Core.DTOs;

public record HackerNewsItem
{
    // Use JsonPropertyName to map C# properties to JSON keys.
    [JsonPropertyName("by")]
    public string? By { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("parent")]
    public int Parent { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}