// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Monogame.Content;
#if !WINDOWS || DIRECTX || XNA
using Monogame.Content.Pipeline;
using Monogame.Content.Pipeline.Graphics;
using Monogame.Content.Pipeline.Processors;
#endif
using Monogame.Graphics;

namespace MonoGame.Tests.ContentPipeline
{
    internal static class AssetTestUtility
    {

        public static Monogame.Graphics.Effect LoadEffect(ContentManager content, string name)
        {
#if DIRECTX
            var gd = ((IGraphicsDeviceService) content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            return CompileEffect(gd, Paths.RawEffect(name));
#else
            return content.Load<Monogame.Graphics.Effect>(Paths.CompiledEffect(name));
#endif
        }

        public static Monogame.Graphics.Effect CompileEffect(GraphicsDevice graphicsDevice, string effectPath)
        {
#if !WINDOWS || DIRECTX || XNA
            var effectProcessor = new EffectProcessor();
            var context = new TestProcessorContext(TargetPlatform.Windows, "notused.xnb");
            var compiledEffect = effectProcessor.Process(new EffectContent
            {
                EffectCode = File.ReadAllText(effectPath),
                Identity = new ContentIdentity(effectPath)
            }, context);

            return new Monogame.Graphics.Effect(graphicsDevice, compiledEffect.GetEffectCode());
#else // OpenGL
            throw new NotImplementedException();
#endif
        }
    }
}
