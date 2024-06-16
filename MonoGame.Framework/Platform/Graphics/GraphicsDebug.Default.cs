// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Monogame.Graphics
{
    public partial class GraphicsDebug
    {
        private bool PlatformTryDequeueMessage(out GraphicsDebugMessage message)
        {
            message = null;
            return false;
        }
    }
}
