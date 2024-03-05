using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Epsiloner.Wpf.Converters;
using Epsiloner.Wpf.Gestures;
using Epsiloner.Wpf.Serializers;

namespace Epsiloner.Wpf.Behaviors.Gestures
{
    /// <summary>
    /// Binds multi-gesture to command on associated <see cref="Control"/>.
    /// </summary>
    public class MultiGestureToCommand : BaseGestureToCommand
    {
        /// <summary>
        /// Multi-gesture to check when <see cref="BaseGestureToCommand.CommandProperty"/>
        /// </summary>
        public static DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(MultiKeyGesture), typeof(MultiGestureToCommand));

        /// <summary>
        /// Multi-gesture to check when <see cref="BaseGestureToCommand.Command"/>
        /// </summary>
        [TypeConverter(typeof(MultiKeyGestureConverter))]
        [ValueSerializer(typeof(MultiKeyGestureSerializer))]
        public MultiKeyGesture Gesture
        {
            get => (MultiKeyGesture)GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }

        /// <inheritdoc />
        protected override KeyGesture GestureValue => Gesture;
    }
}