namespace Cirrus.Extensions;

using System;
using System.Threading;
using Cirrus.API;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Polly.Registry;

public static class CirrusServices
{
    /// <summary>
    /// Adds all internal services required by the Cirrus library
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="setupAction">Cirrus configuration class for API Keys, MacAddresses, and Application Key.</param>
    /// <returns>Service Collection.</returns>
    public static IServiceCollection AddCirrusServices(this IServiceCollection services, Action<CirrusConfig> setupAction)
    {
        var policyRegistry = new PolicyRegistry
        {
            { nameof(Policies.GetRetryPolicy), Policies.GetRetryPolicy() },
            { nameof(Policies.CheckAuthorizedPolicy), Policies.CheckAuthorizedPolicy() },
            { nameof(Policies.GetCircuitBreakerPolicy), Policies.GetCircuitBreakerPolicy() },
            { nameof(Policies.GetTooManyRequestsPolicy), Policies.GetTooManyRequestsPolicy() },
            { nameof(Policies.RateLimitPolicy), Policies.RateLimitPolicy() } 
        };

        services
            .Configure(setupAction)
            .AddTransient<LoggingContext>()
            .AddScoped<ICirrusRealtime, CirrusRealtime>()
            .AddScoped<ICirrusRestWrapper, CirrusRestWrapper>()
            .AddScoped<ICirrusWrapper, CirrusWrapper>()
            .AddPolicyRegistry(policyRegistry);

        services
            .AddHttpClient<ICirrusService, CirrusService>(client =>
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.BaseAddress = new Uri("https://rt.ambientweather.net");
            })
            .AddHttpMessageHandler<LoggingContext>()
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
            .AddPolicyHandlerFromRegistry(nameof(Policies.GetRetryPolicy))
            .AddPolicyHandlerFromRegistry(nameof(Policies.CheckAuthorizedPolicy))
            .AddPolicyHandlerFromRegistry(nameof(Policies.GetCircuitBreakerPolicy))
            .AddPolicyHandlerFromRegistry(nameof(Policies.GetTooManyRequestsPolicy))
            .AddPolicyHandlerFromRegistry(nameof(Policies.RateLimitPolicy));

        return services;
    }
}