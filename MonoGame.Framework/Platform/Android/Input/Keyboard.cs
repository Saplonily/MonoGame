// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;

namespace Monogame.Input;

public static class Keyboard
{
    private static List<Key> keys = new List<Key>();

    private static readonly IDictionary<Keycode, Key> KeyMap = LoadKeyMap();

    internal static bool KeyDown(Keycode keyCode)
    {
        Key key;
        if (KeyMap.TryGetValue(keyCode, out key) && key != Key.None)
        {
            if (!keys.Contains(key))
                keys.Add(key);
            return true;
        }
        return false;
    }

    internal static bool KeyUp(Keycode keyCode)
    {
        Key key;
        if (KeyMap.TryGetValue(keyCode, out key) && key != Key.None)
        {
            if (keys.Contains(key))
                keys.Remove(key);
            return true;
        }
        return false;
    }

    private static IDictionary<Keycode, Key> LoadKeyMap()
    {
        // create a map for every Keycode and default it to none so that every possible key is mapped
        var maps = Enum.GetValues(typeof(Keycode))
            .Cast<Keycode>()
            .ToDictionary(key => key, key => Key.None);

        // then update it with the actual mappings
        maps[Keycode.DpadLeft] = Key.Left;
        maps[Keycode.DpadRight] = Key.Right;
        maps[Keycode.DpadUp] = Key.Up;
        maps[Keycode.DpadDown] = Key.Down;
        maps[Keycode.DpadCenter] = Key.Enter;
        maps[Keycode.Num0] = Key.D0;
        maps[Keycode.Num1] = Key.D1;
        maps[Keycode.Num2] = Key.D2;
        maps[Keycode.Num3] = Key.D3;
        maps[Keycode.Num4] = Key.D4;
        maps[Keycode.Num5] = Key.D5;
        maps[Keycode.Num6] = Key.D6;
        maps[Keycode.Num7] = Key.D7;
        maps[Keycode.Num8] = Key.D8;
        maps[Keycode.Num9] = Key.D9;
        maps[Keycode.A] = Key.A;
        maps[Keycode.B] = Key.B;
        maps[Keycode.C] = Key.C;
        maps[Keycode.D] = Key.D;
        maps[Keycode.E] = Key.E;
        maps[Keycode.F] = Key.F;
        maps[Keycode.G] = Key.G;
        maps[Keycode.H] = Key.H;
        maps[Keycode.I] = Key.I;
        maps[Keycode.J] = Key.J;
        maps[Keycode.K] = Key.K;
        maps[Keycode.L] = Key.L;
        maps[Keycode.M] = Key.M;
        maps[Keycode.N] = Key.N;
        maps[Keycode.O] = Key.O;
        maps[Keycode.P] = Key.P;
        maps[Keycode.Q] = Key.Q;
        maps[Keycode.R] = Key.R;
        maps[Keycode.S] = Key.S;
        maps[Keycode.T] = Key.T;
        maps[Keycode.U] = Key.U;
        maps[Keycode.V] = Key.V;
        maps[Keycode.W] = Key.W;
        maps[Keycode.X] = Key.X;
        maps[Keycode.Y] = Key.Y;
        maps[Keycode.Z] = Key.Z;
        maps[Keycode.Space] = Key.Space;
        maps[Keycode.Escape] = Key.Escape;
        maps[Keycode.Back] = Key.Back;
        maps[Keycode.Home] = Key.Home;
        maps[Keycode.Enter] = Key.Enter;
        maps[Keycode.Period] = Key.OemPeriod;
        maps[Keycode.Comma] = Key.OemComma;
        maps[Keycode.Menu] = Key.Help;
        maps[Keycode.Search] = Key.BrowserSearch;
        maps[Keycode.VolumeUp] = Key.VolumeUp;
        maps[Keycode.VolumeDown] = Key.VolumeDown;
        maps[Keycode.MediaPause] = Key.Pause;
        maps[Keycode.MediaPlayPause] = Key.MediaPlayPause;
        maps[Keycode.MediaStop] = Key.MediaStop;
        maps[Keycode.MediaNext] = Key.MediaNextTrack;
        maps[Keycode.MediaPrevious] = Key.MediaPreviousTrack;
        maps[Keycode.Mute] = Key.VolumeMute;
        maps[Keycode.AltLeft] = Key.LeftAlt;
        maps[Keycode.AltRight] = Key.RightAlt;
        maps[Keycode.ShiftLeft] = Key.LeftShift;
        maps[Keycode.ShiftRight] = Key.RightShift;
        maps[Keycode.Tab] = Key.Tab;
        maps[Keycode.Del] = Key.Delete;
        maps[Keycode.Minus] = Key.OemMinus;
        maps[Keycode.LeftBracket] = Key.OemOpenBrackets;
        maps[Keycode.RightBracket] = Key.OemCloseBrackets;
        maps[Keycode.Backslash] = Key.OemBackslash;
        maps[Keycode.Semicolon] = Key.OemSemicolon;
        maps[Keycode.PageUp] = Key.PageUp;
        maps[Keycode.PageDown] = Key.PageDown;
        maps[Keycode.CtrlLeft] = Key.LeftControl;
        maps[Keycode.CtrlRight] = Key.RightControl;
        maps[Keycode.CapsLock] = Key.CapsLock;
        maps[Keycode.ScrollLock] = Key.Scroll;
        maps[Keycode.NumLock] = Key.NumLock;
        maps[Keycode.Insert] = Key.Insert;
        maps[Keycode.F1] = Key.F1;
        maps[Keycode.F2] = Key.F2;
        maps[Keycode.F3] = Key.F3;
        maps[Keycode.F4] = Key.F4;
        maps[Keycode.F5] = Key.F5;
        maps[Keycode.F6] = Key.F6;
        maps[Keycode.F7] = Key.F7;
        maps[Keycode.F8] = Key.F8;
        maps[Keycode.F9] = Key.F9;
        maps[Keycode.F10] = Key.F10;
        maps[Keycode.F11] = Key.F11;
        maps[Keycode.F12] = Key.F12;
        maps[Keycode.NumpadDivide] = Key.Divide;
        maps[Keycode.NumpadMultiply] = Key.Multiply;
        maps[Keycode.NumpadSubtract] = Key.Subtract;
        maps[Keycode.NumpadAdd] = Key.Add;

        return maps;
    }

    public static KeyboardState GetState()
    {
        return new KeyboardState(keys);
    }

    public static KeyboardState GetState(PlayerIndex playerIndex)
    {
        return new KeyboardState(keys);
    }
}