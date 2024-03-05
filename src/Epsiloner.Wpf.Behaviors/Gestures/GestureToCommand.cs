using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Epsiloner.Wpf.Behaviors.Gestures
{
    /// <summary>
    /// Binds gesture to command on associated <see cref="Control"/>.
    /// </summary>
    public class GestureToCommand : BaseGestureToCommand
    {
        /// <summary>
        /// Gesture to check when <see cref="BaseGestureToCommand.CommandProperty"/>
        /// </summary>
        public static DependencyProperty GestureProperty = DependencyProperty.Register(nameof(Gesture), typeof(KeyGesture), typeof(GestureToCommand));

        /// <summary>
        /// Gesture to check when <see cref="BaseGestureToCommand.Command"/>
        /// </summary>
        public KeyGesture Gesture
        {
            get => (KeyGesture)GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }

        /// <inheritdoc />
        protected override KeyGesture GestureValue => Gesture;
    }
}
