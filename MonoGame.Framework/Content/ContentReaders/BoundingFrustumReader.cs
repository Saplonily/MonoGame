// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Monogame;

namespace Monogame.Content
{
    internal class BoundingFrustumReader : ContentTypeReader<BoundingFrustum>
    {
        public BoundingFrustumReader()
        {
        }

        protected internal override BoundingFrustum Read(ContentReader input, BoundingFrustum existingInstance)
        {
            return new BoundingFrustum(input.ReadMatrix());
        }
    }
}
