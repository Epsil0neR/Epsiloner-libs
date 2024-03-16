using System;
using Epsiloner.WinUi.Gestures;

namespace Epsiloner.WinUi.Services;

public class HotkeysServiceGestureChangedEventArgs : EventArgs
{
    public HotkeysServiceGestureChangedEventArgs(string name, MultiKeyGesture? gesture)
    {
        Name = name;
        Gesture = gesture;
    }

    /// <summary>
    /// Name for which gesture is changed.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// New gesture value. If null - gesture is removed.
    /// </summary>
    public MultiKeyGesture? Gesture { get; }
}