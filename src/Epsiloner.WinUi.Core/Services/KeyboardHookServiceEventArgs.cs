using System;

namespace Epsiloner.WinUi.Services;

public class KeyboardHookServiceEventArgs : EventArgs
{
    public KeyboardHookServiceEventArgs(int keyCode)
    {
        KeyCode = keyCode;
    }

    public int KeyCode { get; }
}