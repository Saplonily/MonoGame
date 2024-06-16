// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Monogame.Content;

internal class UInt32Reader : ContentTypeReader<uint>
{
    public UInt32Reader()
    {
    }

    protected internal override uint Read(ContentReader input, uint existingInstance)
    {
        return input.ReadUInt32();
    }
}