using System;
using System.Collections.Generic;
using System.Linq;
using Monogame;
using Monogame.Audio;
using Monogame.Content;
using Monogame.GamerServices;
using Monogame.Graphics;
using Monogame.Input;
using Monogame.Media;

namespace GamePadTest
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";

		}

		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here

			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			Services.AddService (typeof(SpriteBatch), spriteBatch);
		}

		protected override void UnloadContent ()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update (GameTime gameTime)
		{

			base.Update (gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{

			GraphicsDevice.Clear (Color.CornflowerBlue);

			base.Draw(gameTime);

		}

	}
}
