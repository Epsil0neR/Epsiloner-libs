using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Epsiloner.Wpf.KeyBinding
{
    /// <summary>
    /// Manager of key binding configurations.
    /// </summary>
    public class KeyBindingManager : INotifyPropertyChanged
    {
        private const string IndexerName = "Item[]";

        /// <summary>
        /// Default manager instance.
        /// </summary>
        public static KeyBindingManager Default { get; set; } = new KeyBindingManager();

        /// <summary>
        /// Raised when configs collection have been modified.
        /// </summary>
        public event EventHandler ConfigsChanged;

        /// <summary>
        /// Collection of all key binding configs.
        /// </summary>
        public ObservableCollection<KeyBindingConfig> Configs { get; }

        /// <summary>
        /// Creates a new instance of key bindings manager.
        /// </summary>
        public KeyBindingManager()
        {
            Configs = new ObservableCollection<KeyBindingConfig>();
        }

        /// <summary>
        /// Gets config with specified name.
        /// If manager has no config with specified name, then returns null.
        /// </summary>
        /// <param name="name">Config name</param>
        [IndexerName("Item")]
        public KeyGesture this[string name]
        {
            get { return Configs.FirstOrDefault(x => x.Name == name)?.Gesture; }
        }

        /// <summary>
        /// Gets configs for editing gestures.
        /// </summary>
        /// <returns></returns>
        public KeyBindingConfigs ToEdit()
        {
            var rv = new KeyBindingConfigs();
            rv.AddRange(Configs.Select(x => x.Clone(false)));
            return rv;
        }

        /// <summary>
        /// Loads <paramref name="configs"/> into manager.
        /// </summary>
        /// <param name="configs">Configs to laod</param>
        /// <param name="mode">Update mode</param>
        public void LoadFrom(IEnumerable<KeyBindingConfig> configs, KeyBindingManagerUpdateMode mode)
        {
            configs = configs.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Gesture != null).ToList();

            if (mode == KeyBindingManagerUpdateMode.Full)
            {
                foreach (var config in configs)
                {
                    var c = Configs.FirstOrDefault(x => x.Name == config.Name);
                    if (c != null)
                        Configs.Remove(c);

                    Configs.Add(config.Clone(true));
                }
            }

            if (mode == KeyBindingManagerUpdateMode.OnlyUpdateExisting)
            {
                foreach (var config in configs)
                {
                    var c = Configs.FirstOrDefault(x => x.Name == config.Name && !x.IsHidden);
                    if (c != null)
                    {
                        var ind = Configs.IndexOf(c);
                        var clone = c.Clone(false);
                        clone.Gesture = config.Gesture;
                        clone.Lock();

                        Configs[ind] = clone;
                    }
                }
            }

            ConfigsChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(IndexerName);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event for specified property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}