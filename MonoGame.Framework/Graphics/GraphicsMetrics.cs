// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Monogame.Graphics;

/// <summary>
/// A snapshot of rendering statistics from <see cref="GraphicsDevice.GraphicsMetrics"/> to be used for runtime debugging and profiling.
/// </summary>
public struct GraphicsMetrics
{
    /// <summary>Number of times Clear was called.</summary>
    public long ClearCount { get; internal set; }

    /// <summary>Number of times Draw was called.</summary>
    public long DrawCount { get; internal set; }

    /// <summary>Number of times the pixel shader was changed on the GPU.</summary>
    public long PixelShaderCount { get; internal set; }

    /// <summary>Number of rendered primitives.</summary>
    public long PrimitiveCount { get; internal set; }

    /// <summary>Number of sprites and text characters rendered via <see cref="SpriteBatch"/>.</summary>
    public long SpriteCount { get; internal set; }

    /// <summary>Number of times a target was changed on the GPU.</summary>
    public long TargetCount { get; internal set; }

    /// <summary>Number of times a texture was changed on the GPU.</summary>
    public long TextureCount { get; internal set; }

    /// <summary>Number of times the vertex shader was changed on the GPU.</summary>
    public long VertexShaderCount { get; internal set; }
}