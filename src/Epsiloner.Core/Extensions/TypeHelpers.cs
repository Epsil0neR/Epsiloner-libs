using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsiloner.Extensions;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeHelpers
{
    extension(Type type)
    {
        /// <summary>
        /// Gets a list of interfaces which implements specified type and not it's base type.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetDirectlyImplementedInterfaces()
        {
            ArgumentNullException.ThrowIfNull(type);
            IEnumerable<Type> interfaces = type.GetInterfaces();

            var baseType = type.BaseType;
            if (baseType is null)
                return interfaces;

            var baseInterfaces = baseType.GetInterfaces();
            return interfaces.Where(x => !baseInterfaces.Contains(x)).ToList();
        }
    }

    /// <summary>
    /// Checks if <paramref name="toCheck"/> is subclass of generic type <paramref name="generic"/>.
    /// </summary>
    /// <param name="generic">Generic type</param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    /// <example>
    /// <see cref="generic"/> is <code>List/<T/></code> 
    /// <see cref="toCheck"/> is <code>Names: List/<string/></code> 
    /// </example>
    private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
    {
        if (toCheck is null || !toCheck.IsClass) 
            return false;

        var interfaces = toCheck.GetInterfaces();
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
                return true;

            toCheck = toCheck.BaseType;
        }

        return interfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == generic) != null;
    }
}