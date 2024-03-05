using Epsiloner.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Epsiloner.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyHelpers
    {
        /// <summary>
        /// Gets all derived types from specified <paramref name="baseType" /> in specified <paramref name="assembly" />.
        /// </summary>
        /// <param name="assembly">Assembly where to search derived types</param>
        /// <param name="baseType">Base type</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDerivedTypes(this Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t)).ToList();
        }

        /// <summary>
        /// Gets all loaded to current <see cref="AppDomain"/> assemblies which has references to specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetDependentAssemblies(this Assembly assembly)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetNamesOfAssembliesReferencedBy().Contains(assembly.FullName)).ToList();
        }

        /// <summary>
        /// Gets names of assemblies which is referenced by <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetNamesOfAssembliesReferencedBy(this Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().Select(assemblyName => assemblyName.FullName).ToList();
        }

        /// <summary>
        /// Checks <paramref name="assembly"/> for having <see cref="InitializeOnLoadAttribute"/> and runs static constructors for found types.
        /// If static constructor of type already executed, nothing happens.
        /// </summary>
        /// <param name="assembly"></param>
        public static void InitializeTypesFromAttribute(this Assembly assembly)
        {
            var attrType = typeof(InitializeOnLoadAttribute);
            foreach (InitializeOnLoadAttribute attr in assembly.GetCustomAttributes(attrType, false))
                RuntimeHelpers.RunClassConstructor(attr.Type.TypeHandle); //Static constructor for same type will be executed only once.
        }
    }
}