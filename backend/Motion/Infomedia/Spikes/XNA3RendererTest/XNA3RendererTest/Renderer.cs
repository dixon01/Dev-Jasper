// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Renderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;

    /// <summary>
    /// XNA renderer
    /// </summary>
    public class Renderer : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly RenderManagerFactory renderManagerFactory;
        private readonly XnaRenderContext renderContext = new XnaRenderContext();

        private readonly AlphaAnimator<RootRenderManager<IXnaRenderContext>> rootRenderManagers =
    new AlphaAnimator<RootRenderManager<IXnaRenderContext>>(null);

        private SpriteBatch spriteBatch;
        private RootItem currentScreen;

        private SpriteFont font;
        private VideoPlayer videoPlayer;
        private Video video;


        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class. 
        /// </summary>
        public Renderer()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content = new MyContentManager(this.Content.ServiceProvider);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.PreferredBackBufferWidth = 1440;
            this.Window.AllowUserResizing = true;
            this.Window.Title = "Infomedia 2";
            this.renderManagerFactory = new RenderManagerFactory(this.graphics);
            this.renderContext.Reset();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.DeviceCreated += this.OnCreateDevice;
            this.graphics.DeviceDisposing += this.OnDestroyDevice;
            this.graphics.DeviceReset += this.OnResetDevice;

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

            // TODO: use this.Content to load your game content here
            //this.video = this.Content.Load<Video>("Wildlife.wmv");
            //this.font = this.Content.Load<SpriteFont>("SpriteFont1");
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
            this.UpdateKeyState();

            // TODO: Add your update logic here
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
                
            // TODO: Add your drawing code here
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
        }

        private void Render()
        {
            lock (this.rootRenderManagers)
            {
                this.renderContext.Update();
                this.rootRenderManagers.Update(this.renderContext);

                this.rootRenderManagers.DoWithValues((m, a) => m.Render(a, this.renderContext));
            }
        }

        private void OnCreateDevice(object sender, EventArgs e)
        {
        }

        private void OnDestroyDevice(object sender, EventArgs e)
        {
        }

        private void OnResetDevice(object sender, EventArgs e)
        {
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChange> e)
        {
            this.LoadScreen(e.Message);
        }

        private void LoadScreen(ScreenChange change)
        {
            lock (this.rootRenderManagers)
            {
                var manager = this.renderManagerFactory.CreateRenderManager(change.Root);
                this.rootRenderManagers.Animate(change.Animation, manager);
                this.currentScreen = change.Root;

                // TODO: make it configurable if we should reset the context here
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

        /// <summary>
        /// </summary>
        private class XnaRenderContext : IXnaRenderContext
        {
            private int firstTime;

            /// <summary>
            /// Gets a value indicating whether BlinkOn.
            /// </summary>
            public bool BlinkOn { get; private set; }

            /// <summary>
            /// Gets AlternationCounter.
            /// </summary>
            public int AlternationCounter { get; private set; }

            /// <summary>
            /// Gets ScrollCounter.
            /// </summary>
            public int ScrollCounter { get; private set; }

            /// <summary>
            /// Gets MillisecondsCounter.
            /// </summary>
            public int MillisecondsCounter { get; private set; }

            /// <summary>
            /// </summary>
            public void Reset()
            {
                this.firstTime = Environment.TickCount;
            }

            /// <summary>
            /// </summary>
            public void Update()
            {
                this.MillisecondsCounter = Environment.TickCount;

                // TODO: make 3 seconds alt and 0.5 seconds blink configurable
                int diff = this.MillisecondsCounter - this.firstTime;
                this.AlternationCounter = diff / 3000;
                this.BlinkOn = ((diff / 500) % 2) == 0;
                this.ScrollCounter = diff;
            }
        }
    }

    public class MyContentManager : ContentManager
    {
        public MyContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public MyContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <returns>
        /// </returns>
        protected override Stream OpenStream(string assetName)
        {
            return base.OpenStream(assetName);
        }
    }
}