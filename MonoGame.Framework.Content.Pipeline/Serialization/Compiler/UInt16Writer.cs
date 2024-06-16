﻿// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using TOutput = System.UInt16;

namespace Monogame.Content.Pipeline.Serialization.Compiler;

/// <summary>
/// Writes the unsigned short value to the output.
/// </summary>
[ContentTypeWriter]
class UInt16Writer : BuiltInContentWriter<TOutput>
{
    /// <summary>
    /// Writes the value to the output.
    /// </summary>
    /// <param name="output">The output writer object.</param>
    /// <param name="value">The value to write to the output.</param>
    protected internal override void Write(ContentWriter output, TOutput value)
    {
        output.Write(value);
    }
}