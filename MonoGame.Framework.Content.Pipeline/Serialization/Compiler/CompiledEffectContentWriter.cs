// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Monogame.Content.Pipeline.Processors;

namespace Monogame.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class CompiledEffectContentWriter : BuiltInContentWriter<CompiledEffectContent>
    {
        protected internal override void Write(ContentWriter output, CompiledEffectContent value)
        {
            var code = value.GetEffectCode();
            output.Write(code.Length);
            output.Write(code);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            var type = typeof(ContentReader);
            var readerType = type.Namespace + ".EffectReader, " + type.Assembly.FullName;
            return readerType;
        }
    }
}