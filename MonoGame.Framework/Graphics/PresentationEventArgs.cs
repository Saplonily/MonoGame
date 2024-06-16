// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Monogame.Graphics
{
    internal class PresentationEventArgs : EventArgs
    {
        public PresentationParameters PresentationParameters { get; private set; }

        public PresentationEventArgs(PresentationParameters presentationParameters)
        {
            PresentationParameters = presentationParameters;
        }
    }
}