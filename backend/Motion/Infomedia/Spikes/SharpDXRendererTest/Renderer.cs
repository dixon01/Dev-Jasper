// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Config;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine;

    using NLog;

    using SharpDX.Direct3D9;

    /// <summary>
    /// DirectX renderer using Microsoft's sample framework.
    /// </summary>
    public class Renderer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Direct3D direct3D = new Direct3D();

        private readonly List<RenderWindow> renderWindows = new List<RenderWindow>(2);

        private readonly DxRenderContext renderContext = new DxRenderContext();

        private Timer initialRequestTimer;

        private WindowMode windowMode = WindowMode.Windowed;

        private bool debugMode;

        private bool shouldExit;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        public Renderer()
        {
            this.renderContext.Reset();

            MessageDispatcher.Instance.Subscribe<ScreenChange>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<ScreenUpdate>(this.OnScreenUpdate);

            this.initialRequestTimer = new Timer { Interval = 3 * 1000 };
            this.initialRequestTimer.Start();
            this.initialRequestTimer.Tick += (sender, args) => MessageDispatcher.Instance.Broadcast(new ScreenRequest());

            this.CreateWindows();
        }

        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        public WindowMode WindowMode
        {
            get
            {
                return this.windowMode;
            }

            set
            {
                if (this.windowMode == value)
                {
                    return;
                }

                this.windowMode = value;
                foreach (var window in this.renderWindows)
                {
                    window.WindowMode = this.windowMode;
                }
            }
        }

        /// <summary>
        /// Runs the rendering loop.
        /// </summary>
        public void Run()
        {
            foreach (var window in this.renderWindows)
            {
                window.PreviewKeyDown += this.WindowOnPreviewKeyDown;
                window.Closed += (s, e) => this.shouldExit = true;
                window.Show();
                Application.DoEvents();
            }

            // TODO: use RenderLoop.Run
            while (!this.shouldExit)
            {
                Application.DoEvents();
                this.renderContext.Update();
                foreach (var window in this.renderWindows)
                {
                    try
                    {
                        window.Render();
                    }
                    catch (Exception ex)
                    {
                        Logger.WarnException("Exception while rendering to window " + window.Adapter.Adapter, ex);
                    }
                }
            }

            foreach (var window in this.renderWindows)
            {
                window.PreviewKeyDown -= this.WindowOnPreviewKeyDown;
                window.Hide();
                window.Dispose();
            }
        }

        private void CreateWindows()
        {
            var screenConfigs = ConfigService.Instance.Config.Screens;
            int x = 0;
            if (screenConfigs == null || screenConfigs.Count == 0)
            {
                Logger.Info("Creating automatically all render windows for {0} adapters", this.direct3D.Adapters.Count);
                foreach (AdapterInformation adapter in this.direct3D.Adapters)
                {
                    var viewport = new Rectangle(
                        x, 0, adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);
                    x += viewport.Width;
                    this.AddWindow(adapter, viewport);
                }
            }
            else
            {
                Logger.Info("Creating manually configured render windows for {0} adapters", screenConfigs.Count);
                for (int i = 0; i < screenConfigs.Count; i++)
                {
                    var screen = screenConfigs[i];
                    var adapterOrdinal = screen.Adapter >= 0 ? screen.Adapter : i;
                    if (this.direct3D.Adapters.Count <= adapterOrdinal)
                    {
                        Logger.Warn(
                            "Only {0} adapters available, won't show adapter {1} ({2}x{3})",
                            this.direct3D.Adapters.Count,
                            screen.Adapter,
                            screen.Width,
                            screen.Height);
                        continue;
                    }

                    var adapter = this.direct3D.Adapters[adapterOrdinal];
                    var viewport = new Rectangle(
                        screen.X >= 0 ? screen.X : x,
                        screen.Y,
                        screen.Width > 0 ? screen.Width : adapter.CurrentDisplayMode.Width,
                        screen.Height > 0 ? screen.Height : adapter.CurrentDisplayMode.Height);
                    x += viewport.Width;
                    this.AddWindow(adapter, viewport);
                }
            }
        }

        private void AddWindow(AdapterInformation adapter, Rectangle viewport)
        {
            try
            {
                this.renderWindows.Add(new RenderWindow(this.direct3D, adapter, viewport, this.windowMode, this.renderContext));
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't create the window for adapter " + adapter.Adapter, ex);
            }
        }

        private void WindowOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.shouldExit = true;
                    break;
                case Keys.Enter:
                    if (!e.Alt)
                    {
                        break;
                    }
                    
                    switch (this.WindowMode)
                    {
                        case WindowMode.Windowed:
                            this.WindowMode = WindowMode.FullScreenWindowed;
                            break;
                        case WindowMode.FullScreenWindowed:
                            this.WindowMode = this.renderWindows.Count == 1
                                                  ? WindowMode.FullScreenExclusive
                                                  : WindowMode.Windowed;
                            break;
                        case WindowMode.FullScreenExclusive:
                            this.WindowMode = WindowMode.Windowed;
                            break;
                    }

                    break;
                case Keys.F1:
                    this.debugMode = !this.debugMode;
                    foreach (var window in this.renderWindows)
                    {
                        window.DebugMode = this.debugMode;
                    }

                    break;
            }
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChange> e)
        {
            if (this.initialRequestTimer != null)
            {
                this.initialRequestTimer.Stop();
                this.initialRequestTimer.Dispose();
                this.initialRequestTimer = null;
            }

            Logger.Info("Screen changed. New screen will be shown: #{0}", e.Message.Root.Id);

            // TODO: make it configurable if we should reset the context here
            this.renderContext.Reset();
            foreach (var window in this.renderWindows)
            {
                window.LoadScreen(e.Message);
            }
        }

        private void OnScreenUpdate(object sender, MessageEventArgs<ScreenUpdate> e)
        {
            Logger.Info("Screen #{0} updating", e.Message.ScreenId);
            foreach (var window in this.renderWindows)
            {
                window.UpdateScreen(e.Message);
            }
        }

        private class DxRenderContext : IDxRenderContext
        {
            private int firstTime;

            public bool BlinkOn { get; private set; }

            public int AlternationCounter { get; private set; }

            public int ScrollCounter { get; private set; }

            public int MillisecondsCounter { get; private set; }

            public void Reset()
            {
                this.firstTime = Environment.TickCount;
            }

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
}