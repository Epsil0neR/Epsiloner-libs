using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsiloner.Extensions;

/// <summary>
/// Extension methods for <see cref="EnumHelpers"/>.
/// </summary>
public static class EnumHelpers
{
    extension(Enum input)
    {
        /// <summary>
        /// Gets all flag values in specified flag.
        /// </summary>
        /// <returns>List of flag values.</returns>
        public IEnumerable<Enum> GetFlags()
        {
            return Enum
                .GetValues(input.GetType())
                .Cast<Enum>()
                .Where(input.HasFlag);
        }
    }
}