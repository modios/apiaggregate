using ApiAggregator.Core.Interfaces;
using ApiAggregator.Core.Models;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

public class InMemoryRequestStatsStore : IRequestStatsStore
{
    private readonly ConcurrentDictionary<string, RequestStats> _stats = new();

    public void Record(string path, double responseTimeMs)
    {
        var stat = _stats.GetOrAdd(path, _ => new RequestStats());

        Interlocked.Increment(ref stat.TotalRequests);
        Interlocked.Add(ref Unsafe.As<double, long>(ref stat.TotalResponseTimeMs), BitConverter.DoubleToInt64Bits(responseTimeMs));

        if (responseTimeMs < 100)
            Interlocked.Increment(ref stat.FastRequests);
        else if (responseTimeMs <= 200)
            Interlocked.Increment(ref stat.AverageRequests);
        else
            Interlocked.Increment(ref stat.SlowRequests);
    }

    public Dictionary<string, RequestStats> GetStats() => _stats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}
