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
        return Key != VirtualKey.None;
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