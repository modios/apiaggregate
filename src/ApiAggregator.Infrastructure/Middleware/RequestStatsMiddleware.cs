using ApiAggregator.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

public class RequestStatsMiddleware
{
    private readonly RequestDelegate _next;
    public RequestStatsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRequestStatsStore statsStore)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        var path = context.Request.Path.Value ?? "unknown";
        statsStore.Record(path, stopwatch.Elapsed.TotalMilliseconds);
    }
}
