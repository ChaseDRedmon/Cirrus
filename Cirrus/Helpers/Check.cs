using System;

namespace Cirrus.Helpers
{
    internal static class Check
    {
        internal static void IsNullOrWhitespace(string val)
        {
            if(string.IsNullOrWhiteSpace(val))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(val));
        }
    }
}