using ApiAggregator.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiAggregator.Infrastructure.Services.Monitoring;

public class PerformanceAnalyzerService : BackgroundService
{
    private readonly IRequestStatsStore _statsStore;
    private readonly ILogger<PerformanceAnalyzerService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public PerformanceAnalyzerService(IRequestStatsStore statsStore, ILogger<PerformanceAnalyzerService> logger)
    {
        _statsStore = statsStore;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _semaphore.WaitAsync(0, stoppingToken))
            {
                try
                {
                    AnalyzeStats();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during performance analysis.");
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                _logger.LogWarning("Performance analysis skipped because a previous session is still running.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void AnalyzeStats()
    {
        var stats = _statsStore.GetStats();
        if (stats.Count == 0) return;

        var overallAvg = stats.Values
            .Select(s => s.TotalResponseTimeMs / s.TotalRequests)
            .Average();

        foreach (var kvp in stats)
        {
            var path = kvp.Key;
            var stat = kvp.Value;
            var avg = stat.TotalResponseTimeMs / stat.TotalRequests;

            if (avg > overallAvg * 1.5)
            {
                _logger.LogWarning("Anomaly: Path '{Path}' has avg response time {Avg}ms vs overall {OverallAvg}ms",
                    path, avg, overallAvg);
            }
        }
    }
}
