using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace Cirrus.Extensions
{
    internal static class Policies
    {
        internal static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: 5);
            
            return Policy
                .HandleResult<HttpResponseMessage>(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(delay);
        }

        internal static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(6, TimeSpan.FromMinutes(1));
        }

        internal static IAsyncPolicy<HttpResponseMessage> CheckAuthorizedPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(x => x.StatusCode == HttpStatusCode.Unauthorized)
                .CircuitBreakerAsync(0, TimeSpan.FromMinutes(1));
        }
    }
}