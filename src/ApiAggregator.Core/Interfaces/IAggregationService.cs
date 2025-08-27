namespace ApiAggregator.Core.Interfaces;

public interface IAggregationService
{
    Task<AggregatedResponse> GetAggregatedDataAsync(string city, int hackerNewsItemId, string countryCode);
}
