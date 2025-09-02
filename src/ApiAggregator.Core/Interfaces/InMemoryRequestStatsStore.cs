using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Interfaces;


public interface IRequestStatsStore
{
    void Record(string path, double responseTimeMs);
    Dictionary<string, RequestStats> GetStats();
}
