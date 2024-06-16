// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using Monogame.Content;

namespace Monogame.Content
{
    internal class Vector2Reader : ContentTypeReader<Vector2>
    {
        public Vector2Reader()
        {
        }

        protected internal override Vector2 Read(ContentReader input, Vector2 existingInstance)
        {
            return input.ReadVector2();
        }
    }
}