using System;
using System.Collections.Generic;

namespace Epsiloner.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="EnumHelpers"/>.
    /// </summary>
    public static class EnumHelpers
    {
        /// <summary>
        /// Gets all flag values in specified flag.
        /// </summary>
        /// <param name="input">Flags</param>
        /// <returns>List of flag values.</returns>
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}
