// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Monogame.Content
{
    internal class SByteReader : ContentTypeReader<sbyte>
    {
        public SByteReader()
        {
        }

        protected internal override sbyte Read(ContentReader input, sbyte existingInstance)
        {
            return input.ReadSByte();
        }
    }
}
