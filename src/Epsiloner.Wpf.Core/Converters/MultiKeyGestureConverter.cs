using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Epsiloner.Wpf.Gestures;

namespace Epsiloner.Wpf.Converters
{
    /// <summary>
    /// Converts <see cref="System.String"/> to <see cref="MultiKeyGesture"/>.
    /// Gestures must be split by space.
    /// Modifiers can be added with plus(+) sign.
    /// Modifiers can be used without key.
    /// </summary>
    public class MultiKeyGestureConverter : TypeConverter
    {
        private readonly ModifierKeysConverter _modifierKeysConverter;
        private readonly KeyConverter _keyConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyGestureConverter"/> class.
        /// </summary>
        public MultiKeyGestureConverter()
        {
            _modifierKeysConverter = new ModifierKeysConverter();
            _keyConverter = new KeyConverter();
        }

        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var input = (string)value; // a ctrl+b ctrl+alt+c space
            var items = input.Split(' ');
            var rv = new List<Gesture>();
            foreach (var item in items)
            {
                var pair = item.Split('+');
                var modifier = ModifierKeys.None;
                var key = Key.None;
                try
                {
                    modifier = (ModifierKeys)_modifierKeysConverter.ConvertFrom(item);
                }
                catch (NotSupportedException e)
                {
                    if (pair.Length > 1)
                    {
                        var stringModifier = string.Join("+", pair.Take(pair.Length - 1));
                        modifier = (ModifierKeys)_modifierKeysConverter.ConvertFrom(stringModifier);
                    }
                    key = (Key)_keyConverter.ConvertFrom(pair.Last());
                }

                var gesture = new Gesture(key, modifier);
                if (gesture.IsValid())
                    rv.Add(gesture);
            }

            return rv.Count > 0 ? new MultiKeyGesture(rv) : null;
        }
    }
}
