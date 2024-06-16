// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Monogame.Graphics;

namespace Monogame.Content;

internal class TextureReader : ContentTypeReader<Texture>
{
    protected internal override Texture Read(ContentReader reader, Texture existingInstance)
    {
        return existingInstance;
    }
}