public record AggregatedResponse
{
    public List<AggregatedItem> Items { get; set; }

    public AggregatedResponse(List<AggregatedItem> items)
    {
        Items = items;
    }
}