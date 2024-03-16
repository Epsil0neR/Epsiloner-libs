using System;
using Epsiloner.WinUi.Gestures;

namespace Epsiloner.WinUi.Services;

public interface IHotkeysService
{
    /// <summary>
    /// Restarts system hooks to retrieve keyboard input.
    /// </summary>
    void ReattachHooks();

    /// <summary>
    /// Change handler for named handler with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Handler name.</param>
    /// <param name="handler">Handler method. Null to remove handler.</param>
    /// <exception cref="ArgumentNullException"></exception>
    void Change(string name, Action? handler);

    /// <summary>
    /// Change gesture for named handler with specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Handler name.</param>
    /// <param name="gesture">Gesture to associate with named handler. Null to remove gesture.</param>
    /// <exception cref="ArgumentNullException"></exception>
    void Change(string name, MultiKeyGesture? gesture);
}