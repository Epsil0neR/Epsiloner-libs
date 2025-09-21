using Epsiloner.Wpf.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Epsiloner.Wpf.Utils
{
    /// <summary>
    /// Provides <see cref="ViewModel"/> behavior to any type that cannot inherit from <see cref="ViewModel"/>.
    /// </summary>
    public class ViewModelUtil
    {
        #region "Static"
        private static readonly ConcurrentDictionary<Type, Dictionary<string, HashSet<string>>?> Dependencies = new();

        /// <summary>
        /// Scans all properties in <paramref name="type"/> and caches for fast access in the future.
        /// </summary>
        /// <param name="type">Type to scan.</param>
        private static void ProcessType(Type type)
        {
            lock (Dependencies)
            {
                if (Dependencies.ContainsKey(type)) //TODO: [2025.09.14] What if type has generic params?
                    return;

                var t = typeof(DependsOnAttribute);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var dict = new Dictionary<string, HashSet<string>>();

                foreach (var prop in props)
                {
                    var attributes = prop.GetCustomAttributes(t, true);
                    foreach (var attribute in attributes)
                    {
                        var attr = (DependsOnAttribute)attribute;
                        var name = attr.Name;
                        if (name == null)
                            continue;

                        var list = dict.TryGetValue(name, out var value)
                            ? value
                            : (dict[name] = new()); // Create new entry in dictionary and return it.
                        list.Add(prop.Name);
                    }
                }

                Dependencies[type] = dict.Any() ? dict : null;
            }
        }
        #endregion

        private readonly Type _ownerType;
        private readonly Action<string> _propertyChangedInvoker;

        /// <summary>
        /// Creates instances of util.
        /// </summary>
        /// <param name="ownerType">Type where util is used. In this type dependencies will be looked up.</param>
        /// <param name="propertyChangedInvoker">Actions that raises <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
        public ViewModelUtil(Type ownerType, Action<string> propertyChangedInvoker)
        {
            _ownerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
            _propertyChangedInvoker = propertyChangedInvoker ?? throw new ArgumentNullException(nameof(propertyChangedInvoker));

            ProcessType(_ownerType);
        }

        /// <summary>
        /// Sets new value for backing field and raises <see cref="INotifyPropertyChanged.PropertyChanged"/> event for <paramref name="propertyName"/> and all depending properties.
        /// Depending properties can be specified in param <paramref name="dependingPropertyNames"/> or via attribute <see cref="DependsOnAttribute"/> in same class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">Backing field</param>
        /// <param name="newValue">new value</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="dependingPropertyNames">Depending properties</param>
        /// <returns></returns>
        public bool Set<T>(ref T backingField, T newValue, [CallerMemberName] string? propertyName = null, params string[]? dependingPropertyNames)
        {
            // Check if same value
            if (EqualityComparer<T>.Default.Equals(backingField, newValue)) 
                return false;

            backingField = newValue;
            RaisePropertyChanged(propertyName!);

            if (dependingPropertyNames != null)
                foreach (var name in dependingPropertyNames)
                    RaisePropertyChanged(name);

            Dictionary<string, HashSet<string>>? dependencies;

            lock (Dependencies)
                dependencies = Dependencies.GetValueOrDefault(_ownerType);

            if (propertyName != null && dependencies?.TryGetValue(propertyName, out var names) is true)
            {
                if (dependingPropertyNames != null)
                    names = names.Where(x => !dependingPropertyNames.Contains(x)).ToHashSet();
                foreach (var name in names)
                {
                    RaisePropertyChanged(name);
                }
            }
            return true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            _propertyChangedInvoker.Invoke(propertyName);
        }
    }
}
