using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Epsiloner.Wpf.Behaviors
{
    /// <summary>
    /// Focuses associated object when it becomes visible.
    /// </summary>
    public class AutoFocusBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.IsVisibleChanged += AssociatedObjectOnIsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObjectOnIsVisibleChanged;
            base.OnDetaching();
        }

        private void AssociatedObjectOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!AssociatedObject.IsFocused && AssociatedObject.Focusable)
                AssociatedObject.Focus();
        }
    }
}