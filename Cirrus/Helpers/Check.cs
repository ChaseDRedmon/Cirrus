using System;

namespace Cirrus.Helpers
{
    internal static class Check
    {
        internal static void IsNullOrWhitespace(string? val)
        {
            IsNullOrWhitespace(val, nameof(val));
        }

        internal static void IsNullOrWhitespace(string? val, string paramName)
        {
            if (string.IsNullOrWhiteSpace(val))
                throw new ArgumentException("Value cannot by null or whitespace.", paramName);
        }
    }
}