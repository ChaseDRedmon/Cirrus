using System;

namespace Cirrus.Helpers
{
    public static class Check
    {
        public static void IsNullOrWhitespace(string val)
        {
            if(string.IsNullOrWhiteSpace(val))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(val));
        }
    }
}