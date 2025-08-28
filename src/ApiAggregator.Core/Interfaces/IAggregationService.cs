using ApiAggregator.Core.DTOs;

namespace ApiAggregator.Core.Interfaces;

public interface IAggregationService
{
    Task<AggregatedResponse> GetAggregatedDataAsync(AggregationRequest request);
}
