using System;
using System.Linq;
using Epsiloner.Helpers;
using Windows.System;

namespace Epsiloner.WinUi.Gestures;

/// <summary>
/// Represents single key gesture.
/// </summary>
public class Gesture
{
    public static bool IsValidKey(VirtualKey key)
    {
        return key switch
        {
            >= VirtualKey.F1 and <= VirtualKey.F24 => true,
            >= VirtualKey.A and <= VirtualKey.Z => true,
            >= VirtualKey.Number0 and <= VirtualKey.Number9 => true,
            >= VirtualKey.NumberPad0 and <= VirtualKey.NumberPad9 => true,
            >= VirtualKey.Left and <= VirtualKey.Down => true, // All 4 arrows

            VirtualKey.Multiply => true, // "*"
            VirtualKey.Add => true, // "+"
            VirtualKey.Separator => true, // "|"
            VirtualKey.Subtract => true, // "-"

            VirtualKey.Decimal => true, // "."
            VirtualKey.Divide => true, // "/"

            VirtualKey.Space => true,
            VirtualKey.Tab => true,
            VirtualKey.CapitalLock => true,
            VirtualKey.Back => true,
            VirtualKey.Enter => true,

            (VirtualKey)188 => true, // ",",
            (VirtualKey)190 => true, // ".",
            (VirtualKey)191 => true, // "/",
            (VirtualKey)192 => true, // "`",
            (VirtualKey)186 => true, // ";",
            (VirtualKey)222 => true, // "'",
            (VirtualKey)220 => true, // "\\",
            (VirtualKey)219 => true, // "[",
            (VirtualKey)221 => true, // "]",
            (VirtualKey)189 => true, // "-",
            (VirtualKey)187 => true, // "=",

            _ => false
        };
    }

    /// <summary>Gets the key associated with this <see cref="Gesture" />.</summary>
    /// <returns>The key associated with the gesture.</returns>
    public VirtualKey Key { get; }

    /// <summary>Gets the modifier keys associated with this <see cref="Gesture" />.</summary>
    /// <returns>The modifier keys associated with the gesture. The default value is <see cref="VirtualKeyModifiers.None" />.</returns>
    public VirtualKeyModifiers Modifiers { get; }

    /// <summary>
    /// Creates instance that represents single key gesture.
    /// </summary>
    /// <param name="key">For work only with modifiers, key must be set to <seealso cref="VirtualKey.None"/>.</param>
    /// <param name="modifiers"></param>
    public Gesture(VirtualKey key, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
    {
        if (key is VirtualKey.None)
            throw new ArgumentException("Key cannot be None.");

        Key = key;
        Modifiers = modifiers;
    }

    /// <summary>Determines whether this matches the input associated with the specified parameters.</summary>
    /// <param name="key">Pressed key.</param>
    /// <param name="modifiers">Modifier keys.</param>
    /// <returns><see langword="true" /> if the event data matches this <see cref="Gesture" />; otherwise, <see langword="false" />.</returns>
    public bool Matches(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        return IsDefinedKey(key)
            && key == Key
            && Modifiers == modifiers;
    }

    /// <summary>Determines whether this partially matches the input associated with the specified parameters.</summary>
    /// <param name="key">Pressed key.</param>
    /// <param name="modifiers">Modifier keys.</param>
    /// <returns><see langword="true" /> if the event data partially matches this <see cref="Gesture" />; otherwise, <see langword="false" />.</returns>
    public bool PartiallyMatches(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        if (key != VirtualKey.None)
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
    internal static bool IsDefinedKey(VirtualKey key)
    {
        if (key >= VirtualKey.None)
            return key <= VirtualKey.GamepadRightThumbstickLeft;
        return false;
    }

    /// <summary>
    /// Checks if current gesture is valid.
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return IsValidKey(Key);
    }

    /// <summary>
    /// Validates specified gesture.
    /// </summary>
    /// <param name="gesture">Gesture to check.</param>
    /// <returns></returns>
    public static bool IsValid(Gesture gesture) => gesture?.IsValid() ?? false;

    /// <inheritdoc />
    public override string ToString() =>
        Modifiers == VirtualKeyModifiers.None
            ? $"{Key}"
            : $"{Modifiers}-{Key}";


}