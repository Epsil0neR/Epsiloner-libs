using System.Linq;
using System.Windows.Input;
using Epsiloner.Helpers;

namespace Epsiloner.Wpf.Gestures
{
    /// <summary>
    /// Represents single key gesture.
    /// </summary>
    public class Gesture
    {
        /// <summary>Gets the key associated with this <see cref="KeyGesture" />.</summary>
        /// <returns>The key associated with the gesture.  The default value is <see cref="Key.None" />.</returns>
        public Key Key { get; }

        /// <summary>Gets the modifier keys associated with this <see cref="KeyGesture" />.</summary>
        /// <returns>The modifier keys associated with the gesture. The default value is <see cref="ModifierKeys.None" />.</returns>
        public ModifierKeys Modifiers { get; }

        /// <summary>
        /// Creates instance that represents single key gesture.
        /// </summary>
        /// <param name="key">For work only with modifiers, key must be set to <seealso cref="Key.None"/>.</param>
        /// <param name="modifiers"></param>
        public Gesture(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>Determines whether this <see cref="T:System.Windows.Input.KeyGesture" /> matches the input associated with the specified <see cref="T:System.Windows.Input.InputEventArgs" /> object.</summary>
        /// <param name="key">Pressed key.</param>
        /// <param name="modifiers">Modifier keys.</param>
        /// <returns><see langword="true" /> if the event data matches this <see cref="Gesture" />; otherwise, <see langword="false" />.</returns>
        public bool Matches(Key key, ModifierKeys modifiers)
        {
            return IsDefinedKey(key)
                && key == Key
                && Modifiers == modifiers;
        }

        /// <summary>Determines whether this <see cref="T:System.Windows.Input.KeyGesture" /> partially matches the input associated with the specified <see cref="T:System.Windows.Input.InputEventArgs" /> object.</summary>
        /// <param name="key">Pressed key.</param>
        /// <param name="modifiers">Modifier keys.</param>
        /// <returns><see langword="true" /> if the event data partially matches this <see cref="Gesture" />; otherwise, <see langword="false" />.</returns>
        public bool PartiallyMatches(Key key, ModifierKeys modifiers)
        {
            if (key != Key.None)
                return false;

            var inp = modifiers.GetFlags();
            foreach (var m in inp)
            {
                if (!Modifiers.HasFlag(m))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if <paramref name="key"/> is defined.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool IsDefinedKey(Key key)
        {
            if (key >= Key.None) return key <= Key.OemClear;
            return false;
        }

        /// <summary>
        /// Checks if current gesture is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return Key != Key.None || Modifiers != ModifierKeys.None;
        }

        /// <summary>
        /// Validates specified gesture.
        /// </summary>
        /// <param name="gesture">Gesture to check.</param>
        /// <returns></returns>
        public static bool IsValid(Gesture gesture)
        {
            if (gesture == null)
                return false;

            return gesture.IsValid();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var rv = string.Empty;
            if (Modifiers != ModifierKeys.None)
            {
                var modifiers = Modifiers.GetFlags().Where(x => (ModifierKeys)x != ModifierKeys.None);
                rv = string.Join("+", modifiers);
            }

            if (Modifiers != ModifierKeys.None && Key != Key.None)
            {
                rv += "+" + Key.ToString();
            }
            else if (Key != Key.None)
            {
                rv = Key.ToString();
            }

            return rv;
        }
    }
}
