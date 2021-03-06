// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game1.cs" company="Gorba AG">
//   Copyright � 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is the main type for your game
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNARendererTest
{
    using System.IO;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D image;
        private Video video;
        private VideoPlayer player;
        private Texture2D videoTexture;
        private SpriteFont font;

        private Video alternateVideo;
        private VideoPlayer alternatePlayer;

        private Texture2D alternatevideoTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game1"/> class.
        /// </summary>
        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.PreferredBackBufferWidth = 1440;
            this.Window.AllowUserResizing = true;
        }

        ///// <summary>
        ///// Allows the game to perform any initialization it needs to before starting to run.
        ///// This is where it can query for any required services and load any non-graphic
        ///// related content.  Calling base.Initialize will enumerate through any components
        ///// and initialize them as well.
        ///// </summary>

        // protected override void Initialize()
        // {
        //    base.Initialize();
        // }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // this.image = this.Content.Load<Texture2D>("Jellyfish");
            this.image = Texture2D.FromStream(
                this.GraphicsDevice, File.OpenRead(@"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg"));
            this.video = this.Content.Load<Video>("Wildlife");
            this.alternateVideo = this.Content.Load<Video>("Wildlife1");
            this.font = this.Content.Load<SpriteFont>("SpriteFont1");
            this.player = new VideoPlayer();
            this.alternatePlayer = new VideoPlayer();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            this.UpdateKeyState();

            // TODO: Add your update logic here
            if (this.player.State == MediaState.Stopped)
            {
                this.player.IsLooped = true;
                this.player.Play(this.video);
            }

            if (this.alternatePlayer.State == MediaState.Stopped)
            {
                this.alternatePlayer.IsLooped = true;
                this.alternatePlayer.Play(this.alternateVideo);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this.spriteBatch.Begin();

            // Frame rate display
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            string fps = string.Format("FPS: {0}", frameRate);
            this.spriteBatch.DrawString(this.font, fps, new Vector2(300, 600), Color.Black);

            // Image rendering
            this.spriteBatch.Draw(this.image, new Rectangle(0, 0, 720, 450), Color.White);

            // Video 1 rendering
            if (this.player.State != MediaState.Stopped)
            {
                this.videoTexture = this.player.GetTexture();
            }

            if (this.videoTexture != null)
            {
                this.spriteBatch.Draw(this.videoTexture, new Rectangle(720, 450, 720, 450), Color.White);
            }

            // Video 2 rendering
            if (this.alternatePlayer.State != MediaState.Stopped)
            {
                this.alternatevideoTexture = this.alternatePlayer.GetTexture();
            }

            if (this.alternatevideoTexture != null)
            {
                this.spriteBatch.Draw(this.alternatevideoTexture, new Rectangle(720, 0, 720, 450), Color.White);
            }

            // Text rendering
            this.spriteBatch.DrawString(this.font, "This is a text", new Vector2(300, 500), Color.Black);

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
        }
    }
}
