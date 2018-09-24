namespace WindowsMediaPlayerTest
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public abstract class GameBase : Game
    {
        public static string VideoPath;

        protected GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;

        static GameBase()
        {
            var basePath = Path.GetDirectoryName(typeof(WindowsMediaPlayerGame).Assembly.Location);
            Debug.Assert(basePath != null, "basePath != null");
            VideoPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\..\VideoEvaluation\VideoEvaluation\video.mpg"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMediaPlayerGame"/> class.
        /// </summary>
        public GameBase()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.DeviceCreated += this.GraphicsOnDeviceCreated;
            this.graphics.DeviceReset += this.GraphicsOnDeviceReset;
            this.graphics.DeviceResetting += this.GraphicsOnDeviceResetting;
            this.Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                this.PlayVideo();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                this.graphics.ToggleFullScreen();
            }

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        protected abstract void PlayVideo();

        private void GraphicsOnDeviceResetting(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} graphics device resetting", DateTime.Now);
        }

        private void GraphicsOnDeviceReset(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} graphics device reset", DateTime.Now);
        }

        private void GraphicsOnDeviceCreated(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("{0:HH:mm:ss.fff} graphics device created", DateTime.Now);
        }
    }
}