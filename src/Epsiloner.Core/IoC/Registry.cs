using Epsiloner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsiloner.IoC
{
    /// <summary>
    /// Registry for IoC container.
    /// </summary>
    /// <typeparam name="T">IoC container</typeparam>
    public abstract class Registry<T>
    {
        /// <summary>
        /// Performs types registration in specified <paramref name="container"/>
        /// </summary>
        /// <param name="container">IoC container where types will be registered.</param>
        public abstract void Register(T container);

        /// <summary>
        /// Finds all derived registries in all loaded assemblies.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> FindImplementingTypes()
        {
            var t = typeof(Registry<T>);
            var asm = t.Assembly;
            var assemblies = asm.GetDependentAssemblies();
            
            var rv = assemblies.SelectMany(x => x.FindDerivedTypes(t)).ToList();
            return rv;
        }
    }
}
