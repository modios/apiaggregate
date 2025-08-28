using ApiAggregator.Core.DTOs;

public class AggregatedItem
{
    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public object RawData { get; set; } = default!;
}