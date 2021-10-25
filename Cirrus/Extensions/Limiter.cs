using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ComposableAsync;
using RateLimiter;

namespace Cirrus.Extensions
{
    public sealed class Limiter : DelegatingHandler
    {
        private readonly TimeLimiter _constraint;
        
        public Limiter()
        {
            // 1.5 seconds to prevent retries from happening too often
            _constraint = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(1.5));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _constraint;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}