﻿// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Monogame.Input;

/// <summary>
/// Allows getting keystrokes from keyboard.
/// </summary>
public static partial class Keyboard
{
    /// <summary>
    /// Returns the current keyboard state.
    /// </summary>
    /// <returns>Current keyboard state.</returns>
    public static KeyboardState GetState()
    {
        return PlatformGetState();
    }
}