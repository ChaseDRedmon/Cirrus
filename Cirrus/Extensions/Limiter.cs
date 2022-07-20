namespace Cirrus.Extensions;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ComposableAsync;
using RateLimiter;

/// <summary>
/// Rate Limiter for respecting the Ambient weather API
/// </summary>
internal sealed class Limiter : DelegatingHandler
{
    private readonly TimeLimiter _constraint;

    public Limiter()
    {
        // 1.5 seconds to prevent retries from happening too often
        _constraint = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(1.5));
    }

    /// <summary>
    /// Awaits the constraint and sends the request to the server
    /// </summary>
    /// <param name="request">Http Request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Return the response.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _constraint;
        return await base.SendAsync(request, cancellationToken);
    }
}