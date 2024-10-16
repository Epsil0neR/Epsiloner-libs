﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Epsiloner.WinUi.Services;

/// <summary>
/// Low-level keyboard hook service that catches all keyboard input even in other apps while running.
/// </summary>
public sealed class KeyboardHookService : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private IntPtr _hookId;
    private readonly user32dll.LowLevelKeyboardProc _hookHandler;

    public event EventHandler<KeyboardHookServiceEventArgs> KeyDown;
    public event EventHandler<KeyboardHookServiceEventArgs> KeyUp;

    public KeyboardHookService()
    {
        _hookHandler = HookCallback;
        AddHook();
    }

    public void Dispose()
    {
        RemoveHook();
    }

    /// <summary>
    /// Removes hook (in case when it exists) and hooks again.
    /// </summary>
    public void ReHook()
    {
        RemoveHook();
        AddHook();
    }

    private void AddHook()
    {
        _hookId = SetHook(_hookHandler);
    }

    public void RemoveHook()
    {
        user32dll.UnhookWindowsHookEx(_hookId);
    }

    private static IntPtr SetHook(user32dll.LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;

        return user32dll.SetWindowsHookEx(WH_KEYBOARD_LL, proc, user32dll.GetModuleHandle(curModule.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return user32dll.CallNextHookEx(_hookId, nCode, wParam, lParam);

        switch (wParam.ToInt32())
        {
            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
            {
                var vkCode = Marshal.ReadInt32(lParam);
                KeyDown?.Invoke(this, new(vkCode));
                break;
            }
            case WM_KEYUP:
            case WM_SYSKEYUP:
            {
                var vkCode = Marshal.ReadInt32(lParam);
                KeyUp?.Invoke(this, new(vkCode));
                break;
            }
        }

        return user32dll.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}