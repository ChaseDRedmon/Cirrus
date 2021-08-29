using System;
using Cirrus.API;
using Cirrus.Models;
using Cirrus.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace Cirrus.Extensions
{
    public static class CirrusServices
    {
        public static IServiceCollection AddCirrusServices(this IServiceCollection services, Action<CirrusConfig> setupAction)
        {
            services.Configure(setupAction);
            services.AddScoped<ICirrusRestWrapper, CirrusRestWrapper>();
            services.AddScoped<ICirrusWrapper, CirrusWrapper>();
            services
                .AddHttpClient<ICirrusService, CirrusService>("CirrusService", client =>
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                })
                .AddPolicyHandler(Policies.GetTooManyRequestsPolicy())
                .AddPolicyHandler(Policies.GetRetryPolicy())
                .AddPolicyHandler(Policies.GetCircuitBreakerPolicy())
                .AddPolicyHandler(Policies.CheckAuthorizedPolicy())
                .AddHttpMessageHandler(() => new RateLimitHttpMessageHandler(1, TimeSpan.FromSeconds(1)));

            return services;
        }
    }
}