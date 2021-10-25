using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace Cirrus.Extensions
{
    /// <summary>
    /// Polly Policies for HTTP Client Retries and Circuit Breakers
    /// </summary>
    internal static class Policies
    {
        /// <summary>
        /// Creates a Polly Policy to handle <see cref="HttpStatusCode"/> 429 (Too Many Requests)
        /// </summary>
        /// <remarks> Retries the request using decorrelated jitter up to 5 times </remarks>
        /// <returns> Returns a Polly Policy object </returns>
        internal static IAsyncPolicy<HttpResponseMessage> GetTooManyRequestsPolicy()
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: 5);

            return Policy
                .HandleResult<HttpResponseMessage>(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    delay, 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        context.GetLogger()?.LogWarning("Too many requests submitted within 1 second; retrying in {Timespan}ms for the {RetryAttempt} time", timespan.TotalMilliseconds, retryAttempt);
                    });
        }
        
        /// <summary>
        /// Creates a Polly Policy to handle <see cref="HttpStatusCode"/> 5XX and 408 status code errors
        /// </summary>
        /// <remarks> Retries the request using decorrelated jitter up to 6 times </remarks>
        /// <returns> Returns a Polly Policy object </returns>
        internal static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: 6);

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    delay, 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        context.GetLogger()?.LogWarning("Delaying for {Delay}ms, then making retry {Retry}", timespan.TotalMilliseconds, retryAttempt);
                    });
        }
        
        /// <summary>
        /// Creates a Polly Policy to handle <see cref="HttpStatusCode"/> 5XX and 408 status code errors
        /// </summary>
        /// <remarks>
        /// This policy triggers a circuit breaker for 1 minute, causing all requests to be automatically rejected at the library level.
        /// This works in conjunction with the <see cref="GetRetryPolicy"/>
        /// </remarks>
        /// <returns> Returns a Polly Policy object </returns>
        internal static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(6,
                    onBreak: (outcome, state, timespan, context) =>
                    {
                        context.GetLogger()?.LogWarning("Too many transient HTTP errors have occured; we are pausing for {Delay} seconds", timespan.Seconds);
                    },
                    onReset: context =>
                    {
                        context.GetLogger()?.LogInformation("Resetting the circuit breaker");
                    },
                    durationOfBreak: TimeSpan.FromMinutes(1), 
                    onHalfOpen: () => { }
                );
        }
        
        /// <summary>
        /// Creates a Polly Policy to handle <see cref="HttpStatusCode"/> 401 status code errors
        /// </summary>
        /// <remarks>
        /// This policy triggers a circuit breaker for 1 minute, causing all requests to be automatically rejected at the library level.
        /// </remarks>
        /// <returns> Returns a Polly Policy object </returns>
        internal static IAsyncPolicy<HttpResponseMessage> CheckAuthorizedPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .CircuitBreakerAsync(1, 
                    onBreak: (outcome, state, timespan, context) =>
                    {
                        context.GetLogger()?.LogWarning("Invalid API or Application Key; circuit breaker open for {Timespan} seconds", timespan.Seconds);
                    },
                    onReset: context =>
                    {
                        context.GetLogger()?.LogInformation("Resetting the circuit breaker");
                    },
                    durationOfBreak: TimeSpan.FromMinutes(1), 
                    onHalfOpen: () => { }
                );
        }
    }
}