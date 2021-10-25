using System;
using Cirrus.API;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Polly.Registry;

namespace Cirrus.Extensions
{
    public static class CirrusServices
    {
        public static IServiceCollection AddCirrusServices(this IServiceCollection services, Action<CirrusConfig> setupAction)
        {
            var policyRegistry = new PolicyRegistry
            {
                { nameof(Policies.GetRetryPolicy), Policies.GetRetryPolicy() },
                { nameof(Policies.CheckAuthorizedPolicy), Policies.CheckAuthorizedPolicy() },
                { nameof(Policies.GetCircuitBreakerPolicy), Policies.GetCircuitBreakerPolicy() },
                { nameof(Policies.GetTooManyRequestsPolicy), Policies.GetTooManyRequestsPolicy() }
            };

            services
                .Configure(setupAction)
                .AddTransient<LoggingContext>()
                .AddScoped<Limiter>()
                .AddScoped<ICirrusRestWrapper, CirrusRestWrapper>()
                .AddScoped<ICirrusWrapper, CirrusWrapper>()
                .AddPolicyRegistry(policyRegistry);

            services
                .AddHttpClient<ICirrusService, CirrusService>("CirrusService", client =>
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.BaseAddress = new Uri("https://api.ambientweather.net");
                })
                .AddHttpMessageHandler<LoggingContext>()
                .AddHttpMessageHandler<Limiter>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(2))
                .AddPolicyHandlerFromRegistry(nameof(Policies.GetRetryPolicy))
                .AddPolicyHandlerFromRegistry(nameof(Policies.CheckAuthorizedPolicy))
                .AddPolicyHandlerFromRegistry(nameof(Policies.GetCircuitBreakerPolicy))
                .AddPolicyHandlerFromRegistry(nameof(Policies.GetTooManyRequestsPolicy));

            return services;
        }
    }
}