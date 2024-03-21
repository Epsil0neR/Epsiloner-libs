using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;

namespace Epsiloner.WinUi.Gestures;

/// <summary>
/// Gestures must be split by space.
/// Modifiers can be added with plus(+) sign.
/// Modifiers can be used without key.
/// </summary>
//TODO:
//[TypeConverter(typeof(MultiKeyGestureConverter))]
//[ValueSerializer(typeof(MultiKeyGestureSerializer))]
public sealed class MultiKeyGesture
{
    /// <summary>
    /// Maximum delay between matching executing.
    /// </summary>
    private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(1);
    private DateTime _lastKeyPress = DateTime.MinValue;
    private IEnumerator<Gesture>? _enumerator;

    public IReadOnlyList<Gesture> Gestures { get; }

    public MultiKeyGesture(IReadOnlyList<Gesture> gestures)
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
    public MultiKeyGesture(IReadOnlyList<Gesture> gestures, TimeSpan maxDelay)
        : this(gestures)
    {
        _maxDelay = maxDelay;
    }

    public void ResetMatchState()
    {
        _enumerator?.Dispose();
        _enumerator = null;
    }

    public MultiKeyGestureMatch Matches(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        if (!Gesture.IsDefinedKey(key))
            return MultiKeyGestureMatch.NoMatch;

        var now = DateTime.UtcNow;
        if (_enumerator == null || (now - _lastKeyPress) > _maxDelay)
        {
            _enumerator?.Dispose();
            _enumerator = Gestures.GetEnumerator();
            if (!_enumerator.MoveNext())
                return MultiKeyGestureMatch.NoMatch;
        }

        var g = _enumerator.Current;
        key = ProceedKey(key);
        var rv = g.Matches(key, modifiers);

        _lastKeyPress = now;

        //If pressed modifier and current gesture is not matching modifier keys yet, then just swallow that key event.
        if (!rv && g.PartiallyMatches(key, modifiers))
        {
            return MultiKeyGestureMatch.MatchNotFinal;
        }

        // This line must be after proceeding modifier key
        var hasMoreGestures = _enumerator.MoveNext();

        // Reset enumerator when not matching current gesture or finished
        if (!rv || !hasMoreGestures)
        {
            _enumerator.Dispose();
            _enumerator = null;
        }

        if (!rv)
            return MultiKeyGestureMatch.NoMatch;

        return hasMoreGestures
            ? MultiKeyGestureMatch.MatchNotFinal
            : MultiKeyGestureMatch.Matches;
    }

    public override string ToString() => string.Join(";", Gestures.Select(x => x.ToString()));

    /// <summary>
    /// Processes key and returns key which should be checked in <see cref="Gesture"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static VirtualKey ProceedKey(VirtualKey key)
    {
        switch (key)
        {
            case VirtualKey.Control:
            case VirtualKey.LeftControl:
            case VirtualKey.RightControl:
            case VirtualKey.Menu:
            case VirtualKey.LeftMenu:
            case VirtualKey.RightMenu:
            case VirtualKey.LeftWindows:
            case VirtualKey.RightWindows:
            case VirtualKey.Shift:
            case VirtualKey.LeftShift:
            case VirtualKey.RightShift:
                return VirtualKey.None;
            default:
                return key;
        }
    }
}