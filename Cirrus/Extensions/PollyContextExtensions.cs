﻿using Microsoft.Extensions.Logging;
using Polly;

namespace Cirrus.Extensions
{
    public static class PollyContextExtensions
    {
        private static readonly string LoggerKey = "ILogger";

        public static Context WithLogger<T>(this Context context, ILogger logger)
        {
            context[LoggerKey] = logger;
            return context;
        }

        public static ILogger? GetLogger(this Context context)
        {
            if (context.TryGetValue(LoggerKey, out object logger))
            {
                return logger as ILogger;
            }

            return null;
        }
    }
}