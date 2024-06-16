﻿// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using TOutput = Monogame.Ray;

namespace Monogame.Content.Pipeline.Serialization.Compiler
{
    /// <summary>
    /// Writes the Ray value to the output.
    /// </summary>
    [ContentTypeWriter]
    class RayWriter : BuiltInContentWriter<TOutput>
    {
        /// <summary>
        /// Writes the value to the output.
        /// </summary>
        /// <param name="output">The output writer object.</param>
        /// <param name="value">The value to write to the output.</param>
        protected internal override void Write(ContentWriter output, TOutput value)
        {
            output.Write(value.Position);
            output.Write(value.Direction);
        }
    }
}