// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;

namespace Monogame;

partial class TitleContainer
{
    static partial void PlatformInit()
    {

    }

    private static Stream PlatformOpenStream(string safeName)
    {
        return Stream.Null;
    }
}
