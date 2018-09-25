// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using XNA3MDXTest.MDX;

    /// <summary>
    /// This is the XNA renderer
    /// </summary>
    public class Renderer : Game
    {
        private readonly MdxVideoRenderer videoRenderer;

        private GraphicsDeviceManager graphics;
        private readonly RenderManagerFactory renderManagerFactory;
        private readonly XnaRenderContext renderContext = new XnaRenderContext();
        private readonly AlphaAnimator<RootRenderManager<IXnaRenderContext>> rootRenderManagers =
new AlphaAnimator<RootRenderManager<IXnaRenderContext>>(null);

        private SpriteBatch spriteBatch;
        private RootItem currentScreen;
        private Texture2D image;

        private bool playingVideo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class. 
        /// </summary>
        public Renderer()
        {
            // firstly, I initialize the graphics device manager
            this.graphics = new GraphicsDeviceManager(this) { IsFullScreen = true };

            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.renderManagerFactory = new RenderManagerFactory(this.graphics);

            // secondly, I apply some properties to the window.
            this.Window.AllowUserResizing = true;
            this.Window.Title = "XNA + Video in Managed DirectX";

            // thirdly, I reset the context.
            this.Content = new ContentManager(this.Content.ServiceProvider);
            this.renderContext.Reset();

            // finally, I instantiate the video renderer.
            this.videoRenderer = new MdxVideoRenderer(false);
            this.videoRenderer.VideoEventReceived += this.OnVideoEventReceived;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            MessageDispatcher.Instance.Subscribe<ScreenChange>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<ScreenUpdate>(this.OnScreenUpdate);
            MessageDispatcher.Instance.Broadcast(new ScreenRequest());

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

            // use this.Content to load your game content here
            this.image = Texture2D.FromFile(this.GraphicsDevice, File.OpenRead(@"Backplane.jpg"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.UpdateKeyState();

            // Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var screen = this.currentScreen;
            if (screen != null && !this.graphics.IsFullScreen
                &&
                (screen.Width != this.graphics.PreferredBackBufferWidth
                 && screen.Height != this.graphics.PreferredBackBufferHeight))
            {
                this.graphics.PreferredBackBufferHeight = screen.Height;
                this.graphics.PreferredBackBufferWidth = screen.Width;
                this.graphics.ApplyChanges();
            }

            // Add your drawing code here
            this.spriteBatch.Begin();

            try
            {
                this.Render();
            }
            finally
            {
                this.spriteBatch.End();
            }

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

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if (!this.playingVideo)
                {
                    // the user wants to play the video
                    this.PlayVideo();
                }
            }
        }

        private void Render()
        {
            lock (this.rootRenderManagers)
            {
                this.renderContext.Update();
                this.rootRenderManagers.Update(this.renderContext);
                this.rootRenderManagers.DoWithValues((m, a) => m.Render(a, this.renderContext));

                // now I render my image
                int screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
                var screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                this.spriteBatch.Draw(this.image, screenRectangle, Color.White);
            }
        }

        private void LoadScreen(ScreenChange change)
        {
            lock (this.rootRenderManagers)
            {
                var manager = this.renderManagerFactory.CreateRenderManager(change.Root);
                this.rootRenderManagers.Animate(change.Animation, manager);
                this.currentScreen = change.Root;

                // make it configurable if we should reset the context here
                this.renderContext.Reset();
            }
        }

        private void OnScreenUpdate(object sender, MessageEventArgs<ScreenUpdate> e)
        {
            lock (this.rootRenderManagers)
            {
                this.rootRenderManagers.DoWithValues((m, a) => m.Update(e.Message));
            }
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChange> e)
        {
            this.LoadScreen(e.Message);
        }

        private void PlayVideo()
        {
            // XNA has to abandon the fullscreen mode...
            this.graphics.ToggleFullScreen();
            this.graphics.ApplyChanges();
            this.videoRenderer.Play();
            this.playingVideo = true;
        }

        private void OnVideoEventReceived(object sender, VideoEventArgs e)
        {
            if (e.VideoState == VideoEvent.VideoStopped)
            {
                // I make sure that the video is stopped.
                this.videoRenderer.Stop();

                // and now XNA can get again the fullscreen mode
                this.graphics.ToggleFullScreen();
                this.graphics.ApplyChanges();
            }
        }
    }
}
