namespace XnaVideoTest
{
    using System;

    using DirectShowLib;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Renderer : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private VideoPlayer video;
        private bool videoPlayCompleted;

        public Renderer()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.Window.AllowUserResizing = true;
            this.Window.Title = "Infomedia 2";
            this.graphics.IsFullScreen = true;
            video = new VideoPlayer();
            video.OnVideoComplete += this.OnVideoComplete;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            this.UpdateKeyState();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this.spriteBatch.Begin();

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateKeyState()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                this.graphics.ToggleFullScreen();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                this.graphics.ToggleFullScreen();
                video.Play();
            }
        }

        private void OnVideoComplete(object sender, EventArgs e)
        {
            video.Dispose();
            videoPlayCompleted = true;

            if (!this.graphics.IsFullScreen)
            {
                this.graphics.ToggleFullScreen();
            }
        }
    }
}

