using ApiAggregator.Core.Enums;

public class AggregatedItem
{
    public string Source { get; set; } = string.Empty;
    public Category Category { get; set; }
    public DateTime? Date { get; set; }
    public object RawData { get; set; } = default!;
}