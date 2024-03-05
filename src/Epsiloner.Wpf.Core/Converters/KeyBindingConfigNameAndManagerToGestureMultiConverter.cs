using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using Epsiloner.Wpf.KeyBinding;

namespace Epsiloner.Wpf.Converters
{
    /// <summary>
    /// Finds <see cref="KeyGesture"/> by specified at index=0 <see cref="KeyBindingConfig.Name"/> and optionally specified <see cref="KeyBindingManager"/> at index=1 (if not specified - uses <see cref="KeyBindingManager.Default"/>).
    /// </summary>
    public class KeyBindingConfigNameAndManagerToGestureMultiConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            var name = values[0] as string;
            var manager = values[1] as KeyBindingManager ?? KeyBindingManager.Default;
            return manager?[name];
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}