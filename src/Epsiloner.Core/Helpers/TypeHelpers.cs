using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsiloner.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeHelpers
    {
        /// <summary>
        /// Gets a list of interfaces which implements specified type and not it's base type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetDirectlyImplementedInterfaces(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            IEnumerable<Type> interfaces = type.GetInterfaces();

            if (type.BaseType == null)
                return interfaces;

            var baseInterfaces = type.BaseType.GetInterfaces();
            return interfaces.Where(x => !baseInterfaces.Contains(x)).ToList();
        }


        /// <summary>
        /// Checks if <paramref name="toCheck"/> is sub-class of generic type <paramref name="generic"/>.
        /// </summary>
        /// <param name="generic">Generic type</param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        /// <example>
        /// <see cref="generic"/> is <code>List/<T/></code> 
        /// <see cref="toCheck"/> is <code>Names: List/<string/></code> 
        /// </example>
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (toCheck.IsClass)
            {
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

            return false;
        }
    }
}