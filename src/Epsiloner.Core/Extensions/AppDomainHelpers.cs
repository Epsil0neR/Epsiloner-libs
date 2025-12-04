using Epsiloner.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Epsiloner.Extensions;

/// <summary>
/// Extension methods for <see cref="AppDomain"/>.
/// </summary>
public static class AppDomainHelpers
{
    /// <param name="appDomain">Application domain.</param>
    extension(AppDomain appDomain)
    {
        /// <summary>
        /// Loads all matching search pattern assemblies except assemblies.
        /// If assembly already loaded, it's OK.
        /// </summary>
        /// <param name="searchPattern">
        ///  The search string to match against the names of files in path. This parameter
        ///  can contain a combination of valid literal path and wildcard (* and ?) characters
        ///  (see Remarks), but doesn't support regular expressions.</param>
        /// <param name="fileMatchFunc">Optional function to check if assembly with specified name should be loaded.</param>
        /// <example>
        /// AppDomain.CurrentDomain.LoadAssemblies("test.*.dll");
        /// </example>
        public void LoadAssemblies(string? searchPattern = null, Func<string, bool>? fileMatchFunc = null)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
                searchPattern = "*.dll";

            var ad = AppDomain.CurrentDomain;
            var path = ad.BaseDirectory;
            var matching = Directory.GetFiles(path, searchPattern);
            foreach (var file in matching)
            {
                var loaded = ad.GetAssemblies().Any(x =>
                {
                    //Assembly of type System.Reflection.Emit.InternalAssemblyBuilder
                    //has property .Location which throws exception, but that type is internal,
                    //so single way is to use try...catch
                    try
                    {
                        return x.Location == file;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
                if (loaded)
                    continue;

                if (fileMatchFunc?.Invoke(file) == false)
                    continue;

                var asm = Assembly.LoadFile(file);
                _ = asm.DefinedTypes.ToList(); // This is hack line which loads all referenced assemblies.
            }
        }

        /// <summary>
        /// Checks all existing assemblies for having <see cref="InitializeOnLoadAttribute"/> and runs static constructors for found types.
        /// If static constructor of type already executed, nothing happens.
        /// Also initializes all assemblies that will be loaded in the future.
        /// </summary>
        public Task InitializeTypesFromAttribute()
        {
            return Task.Factory.StartNew(() =>
            {
                appDomain.EnableInitializerForNewlyLoadedAssemblies();

                //Proceed all loaded assemblies.
                var assemblies = appDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                    assembly.InitializeTypesFromAttribute();
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Disables check for having <see cref="InitializeOnLoadAttribute"/> and running static constructors for found types for newly loaded assemblies.
        /// </summary>
        public void DisableInitializerForNewlyLoadedAssemblies()
        {
            appDomain.AssemblyLoad -= CurrentDomainOnAssemblyLoad;
        }

        /// <summary>
        /// Enables check for having <see cref="InitializeOnLoadAttribute"/> and running static constructors for found types for newly loaded assemblies.
        /// </summary>
        public void EnableInitializerForNewlyLoadedAssemblies()
        {
            //To prevent multiple event handlers, first we remove existing handler, only then we add handler to always have only 1 active handler.
            appDomain.DisableInitializerForNewlyLoadedAssemblies();
            appDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
        }
    }

    private static void CurrentDomainOnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        args.LoadedAssembly.InitializeTypesFromAttribute();
    }
}