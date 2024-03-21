using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epsiloner.WinUi.Gestures;
using Microsoft.Extensions.Logging;
using Windows.System;
using Epsiloner.WinUi.Configurations;

namespace Epsiloner.WinUi.Services;

/// <summary>
/// <para>
/// Provides functionality to invoke named handlers by associating <see cref="MultiKeyGesture"/> with handler name. <br />
/// Gestures are processed on <see cref="Services.KeyboardHookService.KeyUp"/> event.
/// </para>
/// CAUTION: This service gets low-level keyboard input from all processes.
/// </summary>
public class HotkeysService : IHotkeysService
{
    /// <summary>
    /// List of <see cref="VirtualKey"/> that will be converted to <see cref="VirtualKey.None"/> and modifier will be assigned of it value. <br/>
    /// In case of ignorance of specific keys - this dictionary can have virtual keys that maps to <see cref="VirtualKeyModifiers.None"/>.
    /// </summary>
    private static readonly Dictionary<VirtualKey, VirtualKeyModifiers> KeysToModifiers = new()
    {
        {VirtualKey.Control, VirtualKeyModifiers.Control},
        {VirtualKey.LeftControl, VirtualKeyModifiers.Control},
        {VirtualKey.RightControl, VirtualKeyModifiers.Control},

        {VirtualKey.Menu, VirtualKeyModifiers.Menu},
        {VirtualKey.LeftMenu, VirtualKeyModifiers.Menu},
        {VirtualKey.RightMenu, VirtualKeyModifiers.Menu},

        {VirtualKey.Shift, VirtualKeyModifiers.Shift},
        {VirtualKey.LeftShift, VirtualKeyModifiers.Shift},
        {VirtualKey.RightShift, VirtualKeyModifiers.Shift},

        {VirtualKey.LeftWindows, VirtualKeyModifiers.Windows},
        {VirtualKey.RightWindows, VirtualKeyModifiers.Windows},
    };

    private readonly ILogger<HotkeysService> _logger;
    private readonly Dictionary<string, Action> _handlers = new();
    private readonly Dictionary<string, MultiKeyGesture> _gestures = new();

    /// <summary>
    /// All currently pressed modifiers. System way of checking each modifier is not working properly.
    /// </summary>
    private readonly HashSet<VirtualKeyModifiers> _modifiers = new();

    /// <summary>
    /// Event that notifies about changed gesture for named handler.
    /// </summary>
    public event EventHandler<HotkeysServiceGestureChangedEventArgs> GestureChanged;

    public KeyboardHookService KeyboardHookService { get; }

    /// <summary>
    /// Service can be paused during editing gestures.
    /// </summary>
    public bool IsPaused { get; set; }

    public HotkeysService(
        KeyboardHookService keyboardHookService, 
        ILogger<HotkeysService> logger,
        IHotkeyServiceConfiguration? configuration = null)
    {
        _logger = logger;
        KeyboardHookService = keyboardHookService;
        KeyboardHookService.KeyDown += OnKeyDown;
        KeyboardHookService.KeyUp += OnKeyUp;

        configuration?.Configure(this);
    }

    /// <inheritdoc />
    public void ReattachHooks() => KeyboardHookService.ReHook();

    /// <inheritdoc />
    public void Change(string name, Action? handler)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be empty or whitespace.");

        if (handler is null)
            _handlers.Remove(name);
        else
            _handlers[name] = handler;
    }

    /// <inheritdoc />
    public void Change(string name, MultiKeyGesture? gesture)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be empty or whitespace.");

        if (gesture is null)
            _gestures.Remove(name);
        else
            _gestures[name] = gesture;

        gesture?.ResetMatchState();

        GestureChanged?.Invoke(this, new(name, gesture));
    }

    private void OnKeyDown(object? sender, KeyboardHookServiceEventArgs e)
    {
        var key = ParseKey(e.KeyCode);
        if (TryParseModifier(e.KeyCode, out var m))
            _modifiers.Add(m);

        if (IsPaused)
            return;

        // Check gestures with assigned key.
        if (key == VirtualKey.None)
            return;

        var modifiers = GetModifiers();
        TryFindGestureAndRunHandler(key, modifiers);
    }

    private void OnKeyUp(object? sender, KeyboardHookServiceEventArgs e)
    {
        TryRemoveModifier(e.KeyCode);
    }

    private void TryFindGestureAndRunHandler(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        var pair = _gestures.FirstOrDefault(x => x.Value.Matches(key, modifiers) == MultiKeyGestureMatch.Matches);
        if (pair.Value is null)
            return;

        if (!_handlers.TryGetValue(pair.Key, out var handler))
            return;

        foreach (var (_, gesture) in _gestures) 
            gesture.ResetMatchState();

        _logger.LogInformation("Hotkey invoked for {name}: {gesture}", pair.Key, pair.Value);
        Task.Run(handler.Invoke);
    }

    private static VirtualKey ParseKey(int keyCode)
    {
        var key = (VirtualKey)keyCode;
        if (KeysToModifiers.ContainsKey(key))
            key = VirtualKey.None;

        return key;
    }

    private static bool TryParseModifier(int keyCode, out VirtualKeyModifiers modifier)
    {
        var key = (VirtualKey)keyCode;
        modifier = KeysToModifiers.GetValueOrDefault(key, VirtualKeyModifiers.None);
        return modifier != VirtualKeyModifiers.None;
    }

    private void TryRemoveModifier(int keyCode)
    {
        if (TryParseModifier(keyCode, out var m))
            _modifiers.Remove(m);
    }

    private VirtualKeyModifiers GetModifiers() => _modifiers.Aggregate(VirtualKeyModifiers.None, (current, modifier) => current | modifier);
}