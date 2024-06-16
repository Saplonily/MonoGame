// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Monogame.Content;
using Monogame.Graphics;

namespace MonoGame.Tests
{
    internal static class AssetTestUtility
    {
        public static Monogame.Graphics.Effect LoadEffect(ContentManager content, string name)
        {
            return content.Load<Monogame.Graphics.Effect>(Paths.CompiledEffect(name));
        }
    }
}
