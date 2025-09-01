using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly.Bulkhead;
using System.Net.Http;

namespace ApiAggregator.API.Policies
{
    public static class ResiliencePolicies
    {
        /// <summary>
        /// Combines Retry, Timeout, Circuit Breaker, and Bulkhead policies
        /// to make HttpClient calls resilient against transient faults, delays, and overloads.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            // Retry Policy:
            // Retries up to 3 times with exponential backoff (2s, 4s, 8s)
            // Useful for transient errors like 500, 502, 503, 504
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Polly] Retry attempt {retryAttempt} after {timespan.TotalSeconds}s.");
                    });

            // Timeout Policy:
            // Cancels the request if it takes longer than 5 seconds
            // Prevents hanging requests from consuming resources
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(5),
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: (context, timespan, task) =>
                {
                    Console.WriteLine($"[Polly] Request timed out after {timespan.TotalSeconds}s.");
                    return Task.CompletedTask;
                });

            // Circuit Breaker Policy:
            // Opens the circuit after 5 consecutive failures
            // Keeps it open for 30 seconds before allowing a test request
            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine($"[Polly] Circuit broken for {breakDelay.TotalSeconds}s.");
                    },
                    onReset: () => Console.WriteLine("[Polly] Circuit reset."),
                    onHalfOpen: () => Console.WriteLine("[Polly] Circuit half-open: testing service.")
                );

            // Bulkhead Policy:
            // Limits concurrent executions to 10
            // Queues up to 5 additional requests
            // Prevents overload by isolating resources
            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 10,
                maxQueuingActions: 5,
                onBulkheadRejectedAsync: context =>
                {
                    Console.WriteLine("[Polly] Bulkhead limit reached. Request rejected.");
                    return Task.CompletedTask;
                });

            // 🧩 Combine all policies into a single wrapper
            return Policy.WrapAsync(retryPolicy, timeoutPolicy, circuitBreakerPolicy, bulkheadPolicy);
        }
    }
}
