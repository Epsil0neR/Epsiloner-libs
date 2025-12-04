using Epsiloner.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Epsiloner.Extensions;

/// <summary>
/// Extension methods for <see cref="Assembly"/>.
/// </summary>
public static class AssemblyHelpers
{
    /// <param name="assembly">Assembly where to search derived types</param>
    extension(Assembly assembly)
    {
        /// <summary>
        /// Gets all derived types from specified <paramref name="baseType" /> in specified <paramref name="assembly" />.
        /// </summary>
        /// <param name="baseType">Base type</param>
        /// <returns></returns>
        public IEnumerable<Type> FindDerivedTypes(Type baseType)
        {
            return assembly.GetTypes()
                .Where(t => t != baseType && baseType.IsAssignableFrom(t))
                .ToList();
        }

        /// <summary>
        /// Gets all loaded to current <see cref="AppDomain"/> assemblies which has references to specified <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetDependentAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetNamesOfAssembliesReferencedBy().Contains(assembly.FullName))
                .ToList();
        }

        /// <summary>
        /// Gets names of assemblies which is referenced by <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNamesOfAssembliesReferencedBy()
        {
            return assembly.GetReferencedAssemblies()
                .Select(assemblyName => assemblyName.FullName)
                .ToList();
        }

        /// <summary>
        /// Checks <paramref name="assembly"/> for having <see cref="InitializeOnLoadAttribute"/> and runs static constructors for found types.
        /// If static constructor of type already executed, nothing happens.
        /// </summary>
        public void InitializeTypesFromAttribute()
        {
            var attrType = InitializeOnLoadAttribute.AttrType;
            foreach (InitializeOnLoadAttribute attr in assembly.GetCustomAttributes(attrType, false))
                RuntimeHelpers.RunClassConstructor(attr.Type.TypeHandle); //Static constructor for same type will be executed only once.
        }
    }
}