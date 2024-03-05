using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using Epsiloner.Wpf.Converters;
using Epsiloner.Wpf.Serializers;

namespace Epsiloner.Wpf.Gestures
{
    /// <summary>
    /// Gestures must be split by space.
    /// Modifiers can be added with plus(+) sign.
    /// Modifiers can be used without key.
    /// </summary>
    [TypeConverter(typeof(MultiKeyGestureConverter))]
    [ValueSerializer(typeof(MultiKeyGestureSerializer))]
    public sealed class MultiKeyGesture : KeyGesture
    {
        /// <summary>
        /// Maximum delay between matching executing.
        /// </summary>
        private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(1);
        private DateTime _lastKeyPress = DateTime.MinValue;
        private IEnumerator<Gesture> _enumerator;

        public IEnumerable<Gesture> Gestures { get; }

        public MultiKeyGesture(IEnumerable<Gesture> gestures)
            : base(Key.None)
        {
            if (gestures == null || !gestures.Any())
                throw new ArgumentNullException(nameof(gestures));
            var g = gestures.Where(Gesture.IsValid).ToList();
            if (!g.Any())
                throw new ArgumentException("Gestures must have at least 1 valid gesture.", nameof(gestures));

            Gestures = g.AsReadOnly();
        }

        /// <summary>
        /// Initializes <see cref="MultiKeyGesture"/> with custom maximum delay between Matches invokinig to reset gesture to initial state.
        /// </summary>
        /// <param name="gestures"></param>
        /// <param name="maxDelay"></param>
        public MultiKeyGesture(IEnumerable<Gesture> gestures, TimeSpan maxDelay)
            : this(gestures)
        {
            _maxDelay = maxDelay;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var e = inputEventArgs as KeyEventArgs;
            if (e == null || !Gesture.IsDefinedKey(e.Key))
                return false;

            var now = DateTime.UtcNow;
            if (_enumerator == null || (now - _lastKeyPress) > _maxDelay)
            {
                _enumerator?.Dispose();
                _enumerator = Gestures.GetEnumerator();
                if (!_enumerator.MoveNext())
                    return false;
            }

            var g = _enumerator.Current;
            var key = ProceedKey(e.Key);
            var rv = g.Matches(key, System.Windows.Input.Keyboard.Modifiers);

            _lastKeyPress = now;

            //If pressed modifier and current gesture is not matching modier keys yet, then just swallow that key event.
            if (!rv && g.PartiallyMatches(key, System.Windows.Input.Keyboard.Modifiers))
            {
                e.Handled = true;
                return false;
            }

            // This line must be after proceeding modifier key
            var next = _enumerator.MoveNext();

            // Reset enumerator when not matching current gesture or finished
            if (!rv || !next)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            // If matching, but not finished yet
            if (rv && next)
                e.Handled = true;

            // Return matching and also has no next gestures.
            return rv && !next;
        }

        /// <summary>
        /// Proceeses key and returns key which should be checked in <see cref="Gesture"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static Key ProceedKey(Key key)
        {
            switch (key)
            {
                case Key.System:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LWin:
                case Key.RWin:
                case Key.LeftShift:
                case Key.RightShift:
                    return Key.None;
                default:
                    return key;
            }
        }
    }
}