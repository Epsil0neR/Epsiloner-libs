using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Epsiloner.Wpf.Behaviors.Gestures
{
    /// <summary>
    /// Base gesture to command class.
    /// Gives ability to bind gestures, disable/enable gesture, choose when gesture hooks input, execute command asynchronously.
    /// </summary>
    public abstract class BaseGestureToCommand : Behavior<UIElement>
    {
        /// <summary>
        /// Associated with gesture command.
        /// </summary>
        public static DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(BaseGestureToCommand));

        /// <summary>
        /// Parameter to command that is passed when <see cref="CommandProperty"/> executes.
        /// </summary>
        public static DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(BaseGestureToCommand));

        /// <summary>
        /// Indicates if gesture hooks input and invokes <see cref="CommandProperty"/>.
        /// </summary>
        public static DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(BaseGestureToCommand), new PropertyMetadata(true));

        /// <summary>
        /// Indicates when gesture will hook input - before or after control.
        /// </summary>
        public static DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(GestureToCommandMode), typeof(BaseGestureToCommand), new PropertyMetadata(GestureToCommandMode.Before, ModePropertyChangedCallback));

        /// <summary>
        /// Indicates if associated <see cref="CommandProperty"/> will be executed synchronously or asynchronously.
        /// </summary>
        public static DependencyProperty ExecuteAsynchronouslyProperty = DependencyProperty.Register(nameof(ExecuteAsynchronously), typeof(bool), typeof(BaseGestureToCommand), new PropertyMetadata(false));

        /// <summary>
        /// Indicates if gesture will hook already handled input.
        /// </summary>
        public static DependencyProperty IgnoreHandledProperty = DependencyProperty.Register(nameof(IgnoreHandled), typeof(bool), typeof(BaseGestureToCommand), new PropertyMetadata(false));

        private static void ModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var g = (BaseGestureToCommand)d;

            if (!g._attached)
                return;

            g.DetachHandlers();
            g.AttachHandlers();
        }

        private static readonly Dictionary<KeyEventArgs, Timer> _partially = new Dictionary<KeyEventArgs, Timer>();
        private bool _attached;
        private readonly RoutedEventHandler _routedEventHandler;
        private readonly TimeSpan _partiallyTimespan;

        /// <summary>
        /// Gets actual gesture.
        /// </summary>
        protected abstract KeyGesture GestureValue { get; }

        /// <summary>
        /// Indicates if gesture hooks input and invokes <see cref="Command"/>.
        /// </summary>
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Associated with gesture command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Parameter to command that is passed when <see cref="Command"/> executes.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Indicates when gesture will hook input - before or after control.
        /// </summary>
        public GestureToCommandMode Mode
        {
            get => (GestureToCommandMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Indicates if associated <see cref="Command"/> will be executed synchronously or asynchronously.
        /// </summary>
        public bool ExecuteAsynchronously
        {
            get => (bool)GetValue(ExecuteAsynchronouslyProperty);
            set => SetValue(ExecuteAsynchronouslyProperty, value);
        }

        /// <summary>
        /// Indicates if gesture will hook already handled input.
        /// </summary>
        public bool IgnoreHandled
        {
            get => (bool)GetValue(IgnoreHandledProperty);
            set => SetValue(IgnoreHandledProperty, value);
        }


        /// <inheritdoc />
        protected BaseGestureToCommand()
        {
            _partiallyTimespan = new TimeSpan(0, 0, 5); //5 sec
#if DEBUG
            _partiallyTimespan = new TimeSpan(0, 5, 0); //5 min
#endif
            _routedEventHandler = Handler;
        }


        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();
            AttachHandlers();
            _attached = true;
        }

        private void AttachHandlers()
        {
            switch (Mode)
            {
                case GestureToCommandMode.Before:
                    AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, _routedEventHandler, true);
                    break;
                case GestureToCommandMode.After:
                    AssociatedObject.AddHandler(UIElement.KeyDownEvent, _routedEventHandler, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DetachHandlers()
        {
            AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, _routedEventHandler);
            AssociatedObject.RemoveHandler(UIElement.KeyDownEvent, _routedEventHandler);
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            _attached = false;
            DetachHandlers();
            base.OnDetaching();
        }

        private void Handler(object sender, RoutedEventArgs args)
        {
            KeyHandled(sender, args as KeyEventArgs);
        }


        private void KeyHandled(object sender, KeyEventArgs e)
        {
            var gesture = GestureValue;
            if (gesture == null)
                return;

            var handled = e.Handled;
            if (e.Handled && _partially.ContainsKey(e))
            {
                handled = false;
            }
            if ((handled && !IgnoreHandled) || !IsEnabled)
                return;

            var cmd = Command;
            var param = CommandParameter;

            var matches = gesture.Matches(AssociatedObject, e);
            if (!matches && e.Handled && !_partially.ContainsKey(e))
            {
                PartiallyMatching(e);
            }
            if (matches && cmd?.CanExecute(param) == true)
            {
                e.Handled = true;

                if (ExecuteAsynchronously)
                    Task.Factory.StartNew(() => cmd.Execute(param), TaskCreationOptions.LongRunning);
                else
                    cmd.Execute(param);
            }
        }

        private void PartiallyMatching(KeyEventArgs e)
        {
            if (_partially.ContainsKey(e))
            {
                var timer = _partially[e];
                try
                {
                    timer.Stop();
                    timer.Start();
                }
                catch (Exception) { }
                return;
            }

            var t = new Timer(_partiallyTimespan.TotalMilliseconds)
            {
                AutoReset = false
            };
            t.Elapsed += (sender, args) =>
            {
                _partially.Remove(e);
                t.Stop();
                t.Dispose();
            };

            _partially[e] = t;
            t.Start();
        }
    }
}