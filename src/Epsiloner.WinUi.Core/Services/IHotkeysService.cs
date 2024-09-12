using System;
using Epsiloner.WinUi.Gestures;

namespace Epsiloner.WinUi.Services;

/// <summary>
/// <para>
/// Provides functionality to invoke named handlers by associating <see cref="MultiKeyGesture"/> with handler name. <br />
/// Gestures are processed on <see cref="Services.KeyboardHookService.KeyUp"/> event.
/// </para>
/// CAUTION: This service gets low-level keyboard input from all processes.
/// </summary>
public interface IHotkeysService
{
    /// <summary>
    /// Event that notifies about changed gesture for named handler.
    /// </summary>
    event EventHandler<HotkeysServiceGestureChangedEventArgs> GestureChanged;

    /// <summary>
    /// Service can be paused during editing gestures.
    /// </summary>
    bool IsPaused { get; set; }

    /// <summary>
    /// Restarts system hooks to retrieve keyboard input.
    /// </summary>
    void ReattachHooks();


    /// <summary>
    /// Stops system hooks from retrieving keyboard input.
    /// </summary>
    void StopHooks();

    /// <summary>
    /// Gets the gesture associated with the <paramref name="name"/>. 
    /// </summary>
    /// <param name="name">Handler name.</param>
    /// <param name="gesture">Gesture associated with <paramref name="name"/>.</param>
    /// <returns><see langword="true" /> if the contains a gesture for specified <paramref name="name"/>>; otherwise, <see langword="false" />.</returns>
    bool TryGetGesture(string name, out MultiKeyGesture? gesture);

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