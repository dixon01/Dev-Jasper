// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   DirectX renderer using Microsoft's sample framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    /// <summary>
    /// DirectX renderer using Microsoft's sample framework.
    /// </summary>
    public class Renderer : IManageableObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<RenderWindow> renderWindows = new List<RenderWindow>(2);

        private readonly DxRenderContext renderContext;

        private readonly ITimer fallbackTimer;

        private readonly ITimer initialRequestTimer;

        private WindowMode windowMode = WindowMode.Windowed;

        private bool debugMode;

        private bool shouldExit;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="config">
        /// The renderer configuration.
        /// </param>
        public Renderer(RendererConfig config)
        {
            this.renderContext = new DxRenderContext(config);
            this.renderContext.Reset();

            this.RenderContextFactory = new DeviceRenderContextFactory(this.renderContext);

            if (config.FallbackTimeout > TimeSpan.Zero)
            {
                this.fallbackTimer = TimerFactory.Current.CreateTimer("Fallback");
                this.fallbackTimer.Interval = config.FallbackTimeout;
                this.fallbackTimer.AutoReset = false;
                this.fallbackTimer.Elapsed += this.FallbackTimerOnElapsed;
                this.fallbackTimer.Enabled = true;
            }

            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.OnScreenChange);

            this.initialRequestTimer = TimerFactory.Current.CreateTimer("InitialRequest");
            this.initialRequestTimer.Interval = TimeSpan.FromSeconds(3);
            this.initialRequestTimer.AutoReset = true;
            this.initialRequestTimer.Elapsed += this.InitialRequestTimerOnElapsed;
            this.initialRequestTimer.Enabled = true;
        }

        /// <summary>
        /// The started event.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired whenever a frame was successfully rendered.
        /// </summary>
        public event EventHandler FrameRendered;

        /// <summary>
        /// Event that is fired when the renderer wants to exit.
        /// Subscribers should set <see cref="ExitRequestEventArgs.Handled"/>
        /// to true if they handled this event.
        /// </summary>
        public event EventHandler<ExitRequestEventArgs> ExitRequested;

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

                Logger.Info("Switching WindowMode to {0}", value);
                this.windowMode = value;
                foreach (var window in this.renderWindows)
                {
                    window.WindowMode = this.windowMode;
                }
            }
        }

        /// <summary>
        /// Gets or sets the device render context factory.
        /// </summary>
        public IDeviceRenderContextFactory RenderContextFactory { get; set; }

        /// <summary>
        /// Gets the render context of this renderer.
        /// </summary>
        public IDxRenderContext RenderContext
        {
            get
            {
                return this.renderContext;
            }
        }

        /// <summary>
        /// Runs the rendering loop.
        /// </summary>
        public void Run()
        {
            this.CreateWindows();

            foreach (var window in this.renderWindows)
            {
                window.PreviewKeyDown += this.WindowOnPreviewKeyDown;
                window.Closed += (s, e) => this.RequestExit("User closed renderer window");
                window.Show();

                try
                {
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            if (this.renderContext.Config.Video.VideoMode == VideoMode.VlcWindow)
            {
                VlcVideoSprite.Initialize();
            }

            this.RaiseStarted(EventArgs.Empty);

            while (!this.shouldExit)
            {
                try
                {
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                this.renderContext.Update();
                var successful = true;
                foreach (var window in this.renderWindows)
                {
                    try
                    {
                        successful &= window.Render();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Exception while rendering to window " + window.Adapter.Adapter);
                        successful = false;
                    }
                }

                if (successful)
                {
                    this.RaiseFrameRendered();
                }
            }

            Logger.Debug("Run() Exiting Primary Loop, closing down");

            foreach (var window in this.renderWindows)
            {
                window.PreviewKeyDown -= this.WindowOnPreviewKeyDown;
                window.Hide();
                window.Dispose();
            }

            if (this.renderContext.Config.Video.VideoMode == VideoMode.VlcWindow)
            {
                VlcVideoSprite.Deinitialize();
            }
        }

        /// <summary>
        /// Stops this renderer by stopping the main loop.
        /// </summary>
        public void Stop()
        {
            this.shouldExit = true;
            if (this.fallbackTimer != null)
            {
                this.fallbackTimer.Enabled = false;
                this.fallbackTimer.Dispose();
            }

            this.initialRequestTimer.Enabled = false;
            this.initialRequestTimer.Dispose();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var renderWindow in this.renderWindows)
            {
                yield return
                    parent.Factory.CreateManagementProvider(
                        renderWindow.Adapter.Information.DeviceName.Replace(@"\", string.Empty), parent, renderWindow);
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Render loop running", !this.shouldExit, true);
            yield return new ManagementProperty<string>("Window mode", this.WindowMode.ToString(), true);
            yield return new ManagementProperty<int>("Number of Adaptors", Manager.Adapters.Count, true);
        }

        private void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseFrameRendered()
        {
            var handler = this.FrameRendered;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void RaiseExitRequested(ExitRequestEventArgs e)
        {
            var handler = this.ExitRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RequestExit(string reason)
        {
            if (this.shouldExit)
            {
                return;
            }

            var args = new ExitRequestEventArgs(reason);
            this.RaiseExitRequested(args);

            if (!args.Handled)
            {
                this.shouldExit = true;
            }
        }

        private void CreateWindows()
        {
            var config = this.renderContext.Config;
            this.WindowMode = config.WindowMode;
            var screenConfigs = config.Screens;
            if (screenConfigs == null || screenConfigs.Count == 0)
            {
                Logger.Info("Creating automatically all render windows for {0} adapters", Manager.Adapters.Count);
                foreach (AdapterInformation adapter in Manager.Adapters)
                {
                    var size = new Size(adapter.CurrentDisplayMode.Width, adapter.CurrentDisplayMode.Height);
                    var screen = new ScreenConfig
                    {
                        Adapter = adapter.Adapter,
                        FallbackImage = string.Empty,
                        Width = adapter.CurrentDisplayMode.Width,
                        Height = adapter.CurrentDisplayMode.Height
                    };
                    this.AddWindow(adapter, size, screen);
                }
            }
            else
            {
                Logger.Info("Creating manually configured render windows for {0} adapters", screenConfigs.Count);
                for (int i = 0; i < screenConfigs.Count; i++)
                {
                    var screen = screenConfigs[i];
                    var adapterOrdinal = screen.Adapter >= 0 ? screen.Adapter : i;
                    if (adapterOrdinal >= Manager.Adapters.Count)
                    {
                        Logger.Warn(
                            "Only {0} adapters available, won't show adapter {1} ({2}x{3})",
                            Manager.Adapters.Count,
                            screen.Adapter,
                            screen.Width,
                            screen.Height);
                        continue;
                    }

                    var adapter = Manager.Adapters[adapterOrdinal];
                    var size = new Size(
                        screen.Width > 0 ? screen.Width : adapter.CurrentDisplayMode.Width,
                        screen.Height > 0 ? screen.Height : adapter.CurrentDisplayMode.Height);
                    this.AddWindow(adapter, size, screen);
                }
            }
        }

        private void AddWindow(AdapterInformation adapter, Size size, ScreenConfig config)
        {
            try
            {
                RenderWindow rw = new RenderWindow(
                    adapter, size, this.windowMode, this.RenderContextFactory, config);
                rw.JitterStats = this.renderContext.JitterStats;
                this.renderWindows.Add(rw);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't create the window for adapter " + adapter.Adapter);
            }
        }

        private bool FullscreenExclusiveAllowed()
        {
            if (this.renderWindows.Count != 1)
            {
                return false;
            }

            // only allow exclusive mode if the (only) adapter has the right resolution
            var renderWindow = this.renderWindows[0];
            return renderWindow.ScreenSize.Width == renderWindow.Adapter.CurrentDisplayMode.Width
                   && renderWindow.ScreenSize.Height == renderWindow.Adapter.CurrentDisplayMode.Height;
        }

        private void WindowOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.RequestExit("User pressed ESC");

                    // this.renderContext.JitterStats.WriteSamples(
                    //    AppDomain.CurrentDomain.BaseDirectory + "framejitters.txt");
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
                            this.WindowMode = this.FullscreenExclusiveAllowed()
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
                    Logger.Debug("Switching debug mode (F1) to {0}", this.debugMode);
                    foreach (var window in this.renderWindows)
                    {
                        window.DebugMode = this.debugMode;
                    }

                    break;
            }
        }

        private void InitialRequestTimerOnElapsed(object sender, EventArgs args)
        {
            var request = new ScreenRequests();
            foreach (var renderWindow in this.renderWindows)
            {
                request.Screens.Add(
                    new ScreenRequest
                    {
                        ScreenId = renderWindow.ScreenId,
                        Width = renderWindow.VisibleRegion.Width,
                        Height = renderWindow.VisibleRegion.Height
                    });
            }

            MessageDispatcher.Instance.Broadcast(request);
        }

        private void FallbackTimerOnElapsed(object sender, EventArgs args)
        {
            foreach (var window in this.renderWindows)
            {
                window?.ShowFallbackScreen();
            }

            this.initialRequestTimer.Enabled = true;
        }

        /// <summary>
        /// Broadcast a reply of ScreenChanges for the given Unit
        /// </summary>
        /// <param name="screenChanges"></param>
        private void BroadcastMediFeedbackMessage(ScreenChanges screenChanges)
        {
            var unitName = MessageDispatcher.Instance?.LocalAddress.Unit;
            if (string.IsNullOrEmpty(unitName))
            {
                unitName = MessageDispatcher.Instance.LocalAddress.Unit;
            }

            var feedBackMessage = new UnitsFeedBackMessage<ScreenChanges>(screenChanges, unitName);
            MessageDispatcher.Instance.Broadcast(feedBackMessage);
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            if (this.fallbackTimer != null)
            {
                this.fallbackTimer.Enabled = false;
            }

            var enablePresentationLogging = this.renderContext.Config.EnablePresentationLogging;
            var screenLoaded = false;
            var validScreenChanges = new List<ScreenChange>();
            
            foreach (var window in this.renderWindows)
            {
                foreach (var screenChange in e.Message.Changes)
                {
                    // find a matching Screen Type and ID, intertested in only our work
                    if (screenChange.Screen.Type == window.ScreenId.Type
                        && (screenChange.Screen.Id == window.ScreenId.Id || screenChange.Screen.Id == null))
                    {
                        window?.LoadScreen(screenChange);
                        if (enablePresentationLogging == true)
                        {
                            validScreenChanges.Add(screenChange);
                        }
                        screenLoaded = true;
                    }
                }

                e.Message.Updates.Sort((a, b) => a.RootId.CompareTo(b.RootId));
                window.UpdateScreens(e.Message.Updates);
            }

            if (this.fallbackTimer != null)
            {
                this.fallbackTimer.Enabled = true;
            }

            if (validScreenChanges.Any())
            {
                // Proof of play, send Medi Feedback message with ScreenChagnes included that applied for the matching type and screen id
                this.BroadcastMediFeedbackMessage(new ScreenChanges() { Changes = validScreenChanges });
            }

            if (!screenLoaded)
            {
                return;
            }

            // TODO: make it configurable if we should reset the context here
            this.renderContext.Reset();

            this.initialRequestTimer.Enabled = false;
        }

        private class DxRenderContext : IDxRenderContext
        {
            private readonly JitterStats jitterStats = new JitterStats();

            private readonly int blinkMillis;

            private long firstTime;

            private PersistenceViewManager persistenceViewManager = new PersistenceViewManager();

            public DxRenderContext(RendererConfig config)
            {
                this.Config = config;

                this.blinkMillis = (int)this.Config.Text.BlinkInterval.TotalMilliseconds;
            }

            public PersistenceViewManager PersistenceView
            {
                get
                {
                    return this.persistenceViewManager;
                }
            }

            public JitterStats JitterStats
            {
                get { return this.jitterStats; }
            }

            public RendererConfig Config { get; private set; }

            public bool BlinkOn { get; private set; }

            public long ScrollCounter { get; private set; }

            public long MillisecondsCounter { get; private set; }

            public void Reset()
            {
                // this.firstTime = TimeProvider.Current.TickCount;
                // TODO: check the safety of this method call
                this.firstTime = Gorba.Common.Utility.Win32.Api.DLLs.Winmm.TimerGetTime();
            }

            public void Update()
            {
                // this.MillisecondsCounter = TimeProvider.Current.TickCount;
                long timerms = Gorba.Common.Utility.Win32.Api.DLLs.Winmm.TimerGetTime();
#if DEBUG
                this.jitterStats.AddSample(timerms - this.MillisecondsCounter);
#endif
                this.MillisecondsCounter = timerms;
                var diff = this.MillisecondsCounter - this.firstTime;
                this.BlinkOn = ((diff / this.blinkMillis) % 2) == 0;
                this.ScrollCounter = diff;
            }
        }

        private class DeviceRenderContextFactory : IDeviceRenderContextFactory
        {
            private readonly IDxRenderContext context;

            public DeviceRenderContextFactory(IDxRenderContext context)
            {
                this.context = context;
            }

            public DeviceConfig DeviceConfig
            {
                get
                {
                    return this.context.Config.Device;
                }
            }

            public IDxDeviceRenderContext CreateContext(Device device)
            {
                return new DeviceRenderContext(device, this.context);
            }
        }
    }
}