using Epsiloner.Helpers;
using System;

namespace Epsiloner.Attributes
{
    /// <summary>
    /// Specifies which types should execute static constructor when assembly is loaded.
    /// Note: <see cref="AppDomainHelpers.InitializeTypesFromAttribute"/> method must be invoked outside to execute static constructors for all existing attributes and also handle new assemblies load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class InitializeOnLoadAttribute : Attribute
    {
        private static readonly Type AttrType = typeof(InitializeOnLoadAttribute);

        /// <inheritdoc />
        public InitializeOnLoadAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type, which static constructor should be executed.
        /// </summary>
        public Type Type { get; }

    }
}

