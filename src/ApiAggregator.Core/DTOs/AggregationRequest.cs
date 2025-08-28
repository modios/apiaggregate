using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Core.DTOs;
public record AggregationRequest
{
    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "CountryCode is required.")]
    public string CountryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "HackerNewsItemId is required.")]
    public int HackerNewsItemId { get; set; }

    public string? Category { get; set; }
    public string? SortBy { get; set; } // "date"
    public string SortOrder { get; set; } = "asc"; // "asc" or "desc"
}