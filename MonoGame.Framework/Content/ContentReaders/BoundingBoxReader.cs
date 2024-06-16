﻿// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Monogame.Content
{
    class BoundingBoxReader : ContentTypeReader<BoundingBox>
    {
        protected internal override BoundingBox Read(ContentReader input, BoundingBox existingInstance)
        {
            var min = input.ReadVector3();
            var max = input.ReadVector3();
            var result = new BoundingBox(min, max);
            return result;
        }
    }
}
