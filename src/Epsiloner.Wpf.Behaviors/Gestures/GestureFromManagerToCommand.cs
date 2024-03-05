using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Epsiloner.Wpf.Converters;
using Epsiloner.Wpf.KeyBinding;

namespace Epsiloner.Wpf.Behaviors.Gestures
{
    /// <summary>
    /// Binds command to gesture from <see cref="KeyBindingConfig"/> in <see cref="KeyBindingManager"/>.
    /// </summary>
    public class GestureFromManagerToCommand : GestureToCommand
    {
        /// <summary>
        /// Key binding configs manager.
        /// </summary>
        public static DependencyProperty ManagerProperty = DependencyProperty.Register(nameof(Manager), typeof(KeyBindingManager), typeof(GestureFromManagerToCommand), new PropertyMetadata(null, ManagerPropertyChangedCallback));

        private static void ManagerPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var g = (GestureFromManagerToCommand)d;
            var o = e.OldValue as KeyBindingManager;
            var n = e.NewValue as KeyBindingManager;
            g.ManagerChanged(o, n);
        }

        private void ManagerChanged(KeyBindingManager o, KeyBindingManager n)
        {
            if (o != null)
            {
                o.PropertyChanged -= ManagerOnPropertyChanged;
            }
            if (n != null)
            {
                n.PropertyChanged += ManagerOnPropertyChanged;
            }
        }

        private void ManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BindingOperations.GetBindingExpressionBase(this, GestureProperty)?.UpdateTarget();
        }

        /// <summary>
        /// Key binding configs manager.
        /// </summary>
        public KeyBindingManager Manager
        {
            get => (KeyBindingManager)GetValue(ManagerProperty) ?? KeyBindingManager.Default;
            set => SetValue(ManagerProperty, value);
        }

        /// <inheritdoc />
        public GestureFromManagerToCommand()
        {
            var c = new KeyBindingConfigNameAndManagerToGestureMultiConverter();
            var b = new MultiBinding()
            {
                Converter = c
            };
            b.Bindings.Add(new Binding(nameof(ConfigName)) { Source = this, Mode = BindingMode.OneWay });
            b.Bindings.Add(new Binding(nameof(Manager)) { Source = this, Mode = BindingMode.OneWay });


            BindingOperations.SetBinding(this, GestureProperty, b);
        }

        /// <summary>
        /// Name of <see cref="KeyBindingConfig"/>.
        /// </summary>
        public static DependencyProperty ConfigNameProperty = DependencyProperty.Register(nameof(ConfigName), typeof(string), typeof(GestureFromManagerToCommand));

        /// <summary>
        /// Name of <see cref="KeyBindingConfig"/>.
        /// </summary>
        public string ConfigName
        {
            get => (string)GetValue(ConfigNameProperty);
            set => SetValue(ConfigNameProperty, value);
        }
    }
}