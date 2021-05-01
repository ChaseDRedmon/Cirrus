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
            services.AddTransient<ICirrusService, CirrusService>();
            services.AddTransient<ICirrusRestWrapper, CirrusRestWrapper>();
            services.AddTransient<ICirrusWrapper, CirrusWrapper>();
            
            return services;
        }
    }
}