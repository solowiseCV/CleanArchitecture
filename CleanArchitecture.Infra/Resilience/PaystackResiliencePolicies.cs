using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace CleanArchitecture.Infrastructure.Resilience
{
  
    public static class PaystackResiliencePolicies
    {
       
        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                seconds: 10,
                timeoutStrategy: TimeoutStrategy.Optimistic
            );
        }

       
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
        {
            var jitterer = new Random();

            return HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx + network errors
                .Or<TimeoutRejectedException>() // also retry on Polly timeout
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                    {
                        var exponential = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        var jitter = TimeSpan.FromMilliseconds(jitterer.Next(0, 500));
                        return exponential + jitter;
                    },
                    onRetry: (outcome, timespan, retryAttempt, _) =>
                    {
                        logger.LogWarning(
                            "[Paystack] Retry {RetryAttempt}/3 — waiting {Delay}s — Reason: {Reason}",
                            retryAttempt,
                            timespan.TotalSeconds.ToString("F1"),
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()
                        );
                    });
        }

    
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,           // Open if 50%+ of requests fail...
                    samplingDuration: TimeSpan.FromSeconds(60), // ...within a 60s rolling window
                    minimumThroughput: 5,             // ...but only if at least 5 requests occurred
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, state, duration, _) =>
                    {
                        logger.LogError(
                            "[Paystack] Circuit OPENED for {Duration}s — Reason: {Reason}",
                            duration.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()
                        );
                    },
                    onReset: _ =>
                    {
                        logger.LogInformation("[Paystack] Circuit CLOSED — service returned to normal.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("[Paystack] Circuit HALF-OPEN — sending test probe request.");
                    });
        }
        public static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .Or<Polly.CircuitBreaker.BrokenCircuitException>()
                .FallbackAsync(
                    fallbackValue: new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(
                            "{\"status\":false,\"message\":\"Payment service is temporarily unavailable. Please try again shortly.\"}",
                            System.Text.Encoding.UTF8,
                            "application/json")
                    },
                    onFallbackAsync: (outcome, _) =>
                    {
                        logger.LogCritical(
                            "[Paystack] FALLBACK triggered — all resilience policies exhausted. Reason: {Reason}",
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()
                        );
                        return Task.CompletedTask;
                    });
        }
    }
}
