using ApiAggregator.Core.Enums;
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

    public Category? Category { get; set; }
    public SortBy? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Asc;

}