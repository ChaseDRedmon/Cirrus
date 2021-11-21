using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirrus.Helpers
{
    /// <summary>
    /// Check Helper Class for Guard Statements
    /// </summary>
    internal static class Check
    {
        /// <summary>
        /// Check to see if this string is null or whitespace
        /// </summary>
        /// <param name="val">String to check.</param>
        /// <exception cref="ArgumentException">Throws an argument exception if all strings are null or whitespace.</exception>
        internal static void IsNullOrWhitespace(string? val)
        {
            if (string.IsNullOrWhiteSpace(val))
                throw new ArgumentException("Value cannot be null or whitespace", nameof(val));
        }

        /// <summary>
        /// Checks to see if all strings in the array are null or whitespace
        /// </summary>
        /// <param name="val">Strings to check.</param>
        /// <exception cref="ArgumentException">Throws an argument exception if all strings are null or whitespace.</exception>
        internal static void AreAllNullOrWhiteSpace(params string?[] val)
        {
            var result = val.All(string.IsNullOrWhiteSpace);
            if (result) throw new ArgumentException("Value cannot be null or whitespace", nameof(val));
        }

        /// <summary>
        /// Checks to see if all strings in the enumerable are null or whitespace
        /// </summary>
        /// <param name="strings">Strings to check.</param>
        /// <exception cref="ArgumentException">Throws an argument exception if all strings are null or whitespace.</exception>
        internal static void AreAllNullOrWhiteSpace(IEnumerable<string?> strings)
        {
            var result = strings.All(string.IsNullOrWhiteSpace);
            if (result) throw new ArgumentException("Value cannot be null or whitespace", nameof(strings));
        }
    }
}