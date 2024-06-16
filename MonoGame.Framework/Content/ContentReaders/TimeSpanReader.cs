// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

using Monogame.Content;

namespace Monogame.Content
{
    internal class TimeSpanReader : ContentTypeReader<TimeSpan>
    {
        public TimeSpanReader()
        {
        }

        protected internal override TimeSpan Read(ContentReader input, TimeSpan existingInstance)
        {
            // Could not find any information on this really but from all the searching it looks
            // like the constructor of number of ticks is long so I have placed that here for now
            // long is a Int64 so we read with 64
            // <Duration>PT2S</Duration>
            // 

            return new TimeSpan(input.ReadInt64());
        }
    }
}
