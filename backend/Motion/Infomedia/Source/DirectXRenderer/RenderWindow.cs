// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderWindow.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Core;
    /// Referenced for Acapela branding #4180 VSTS.
    using Gorba.Common.Configuration.Infomedia.AudioRenderer;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Wrapper;
    using Gorba.Motion.Infomedia.DirectXRenderer.DxExtensions;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.DirectXRenderer.Properties;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.RendererBase.Manager.Animation;

    using Microsoft.DirectX.Direct3D;
    using NLog;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;
    using HorizontalAlignment = Gorba.Common.Configuration.Infomedia.Layout.HorizontalAlignment;
    using Math = System.Math;
    using Thread = System.Threading.Thread;

    /// <summary>
    /// The render window.
    /// </summary>
    public sealed partial class RenderWindow : Form, IManageableObject
    {
        private static readonly TimeSpan MinSplashTimeout = TimeSpan.FromSeconds(5);

        private static readonly Color DefaultBackgroundColor = Color.Black;
        private static readonly Color SplashBackgroundColor = Color.FromArgb(230, 236, 240);

        private readonly Logger logger;

        private readonly PortListener backlightPort;

        private readonly DeviceConfig deviceConfig;

        private readonly IDxDeviceRenderContext renderContext;

        private readonly Screen screen;

        private readonly RenderManagerFactory renderManagerFactory;

        private readonly AlphaAnimator<ScreenRootRenderManager<IDxDeviceRenderContext>> screenRenderManagers =
            new AlphaAnimator<ScreenRootRenderManager<IDxDeviceRenderContext>>(null);

        private readonly Timer splashTimer = new Timer();

        private readonly List<string> loadedFonts = new List<string>();

        private readonly ScreenConfig config;

        private WindowMode windowMode;
        private Device device;

        private Sprite fpsSprite;
        private IFontInfo fpsFont;

        private long frameCounter;
        private long logFrameCounter;
        private int lastTickCount;
        private int lastLogTickCount;
        private string lastFrameRate = string.Empty;

        private bool showingSplashScreen;
        private ScreenRootRenderManager<IDxDeviceRenderContext> firstManager;

        private TimeSpan splashTimeout;

        private float currentFrameRate;

        private ScreenRoot currentRoot;

        private bool? lastBacklightValue;

        private IntPtr focusThief;

        private bool firstTime;

        private Viewport viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderWindow"/> class.
        /// </summary>
        /// <param name="adapter">
        ///     The adapter.
        /// </param>
        /// <param name="size">
        ///     The size of the rendering area.
        /// </param>
        /// <param name="windowMode">
        ///     The render mode.
        /// </param>
        /// <param name="contextFactory">
        /// The render context factory.
        /// </param>
        /// <param name="config">
        /// The screen configuration
        /// </param>
        public RenderWindow(
            AdapterInformation adapter,
            Size size,
            WindowMode windowMode,
            IDeviceRenderContextFactory contextFactory,
            ScreenConfig config)
        {
            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
            this.logger = LogManager.GetLogger(string.Format("{0}<{1}>", this.GetType().FullName, adapter.Adapter));
            this.Adapter = adapter;
            this.ScreenSize = size;
            this.config = config;
            this.LostFocus += this.OnLostFocus;

            this.deviceConfig = contextFactory.DeviceConfig;

            if (windowMode == WindowMode.FullScreenExclusive
                && (size.Width != adapter.CurrentDisplayMode.Width || size.Height != adapter.CurrentDisplayMode.Height))
            {
                this.logger.Warn("FullScreenExclusive not supported with this resolution, using FullScreenWindowed");
                windowMode = WindowMode.FullScreenWindowed;
            }

            this.windowMode = windowMode;

            this.ScreenId = new ScreenId
            {
                Type = PhysicalScreenType.TFT,
                Id = this.config.Id ?? adapter.Adapter.ToString(CultureInfo.InvariantCulture)
            };

            this.logger.Info(
                "Creating render window for adapter {0}: {1}x{2}", adapter.Adapter, size.Width, size.Height);

            this.backlightPort = new PortListener(
                new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*"), "Backlight");
            this.backlightPort.Start(TimeSpan.FromSeconds(5));

            this.InitializeComponent();
            var fileVersion = FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location);

            try
            {
                var version = new Version(fileVersion.FileVersion);
                this.Text = string.Format("{0} {1}", this.Text, version.ToString());
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed getting assembly version {0}, Cause:{1}", fileVersion.FileVersion, ex.Message);
                this.Text = "N/A";
            }

            var visibleRect = config.VisibleRegion;
            this.viewport.X = visibleRect != null ? visibleRect.X : 0;
            this.viewport.Y = visibleRect != null ? visibleRect.Y : 0;
            this.viewport.Width = visibleRect != null && visibleRect.Width > 0 ? visibleRect.Width : size.Width;
            this.viewport.Height = visibleRect != null && visibleRect.Height > 0 ? visibleRect.Height : size.Height;

            this.VisibleRegion = new Rectangle(
                this.viewport.X, this.viewport.Y, this.viewport.Width, this.viewport.Height);
            this.screen = Array.Find(
                Screen.AllScreens,
                s =>
                s.DeviceName.StartsWith(adapter.Information.DeviceName, StringComparison.InvariantCultureIgnoreCase));

            this.ResetWindow(true);
            this.renderContext = contextFactory.CreateContext(this.device);

            this.renderManagerFactory = new RenderManagerFactory(
                this.renderContext,
                new Rectangle(this.viewport.X, this.viewport.Y, this.viewport.Width, this.viewport.Height));
            this.splashTimer.Tick += this.SplashTimerOnTick;
            this.SplashTimeout = MinSplashTimeout;

            var splashManager = this.renderManagerFactory.CreateRenderManager(this.CreateSplashScreen());
            this.ChangeRenderManagers(splashManager, null);
        }

        /// <summary>
        /// Gets or sets the jitter stats.
        /// </summary>
        public JitterStats JitterStats { get; set; }

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
                this.ResetWindow(true);
            }
        }

        /// <summary>
        /// Gets or sets the splash screen timeout (minimum time the splash screen is shown).
        /// </summary>
        public TimeSpan SplashTimeout
        {
            get
            {
                return this.splashTimeout;
            }

            set
            {
                if (this.splashTimeout == value)
                {
                    return;
                }

                var oldTimeout = this.splashTimeout;
                this.splashTimer.Enabled = false;

                if (value < TimeSpan.Zero)
                {
                    this.splashTimeout = TimeSpan.Zero;
                    this.HideSplashScreen();
                    return;
                }

                this.splashTimeout = value < MinSplashTimeout ? MinSplashTimeout : value;
                var interval = this.splashTimeout - oldTimeout;
                if (interval <= TimeSpan.Zero)
                {
                    this.HideSplashScreen();
                    return;
                }

                this.showingSplashScreen = true;
                this.splashTimer.Interval = (int)interval.TotalMilliseconds;
                this.splashTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the debug mode should be shown.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Gets the screen id.
        /// </summary>
        public ScreenId ScreenId { get; private set; }

        /// <summary>
        /// Gets the screen size.
        /// </summary>
        public Size ScreenSize { get; private set; }

        /// <summary>
        /// Gets the adapter information.
        /// </summary>
        public AdapterInformation Adapter { get; private set; }

        /// <summary>
        /// Gets the visible region of the screen.
        /// </summary>
        public Rectangle VisibleRegion { get; private set; }

        /// <summary>
        /// Renders everything in this window.
        /// This method has to be called regularly by the renderer.
        /// </summary>
        /// <returns>
        /// True if the renderer could successfully render the screen.
        /// </returns>
        public bool Render()
        {
            if (this.device == null || this.device.Disposed)
            {
                return false;
            }

            if (!this.firstTime)
            {
                this.BringToFront();
                this.Focus();
                var focusWindow = User32.GetForegroundWindow();
                if (focusWindow != IntPtr.Zero && focusWindow != this.Handle)
                {
                    this.logger.Debug("Minimizing focus window: {0:X8}", focusWindow.ToInt32());
                    User32.ShowWindow(focusWindow, ShowWindow.Minimize);
                }

                this.firstTime = true;
            }

            if (!this.CheckCooperativeLevel())
            {
                // don't loop at 100% CPU if we have an issue with the render window
                Thread.Sleep(100);

                // it is ok if we weren't rendered if the focus was stolen by another window
                return this.focusThief != IntPtr.Zero;
            }

            this.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, DefaultBackgroundColor, 1.0f, 0);
            this.device.Viewport = this.viewport;

            if (this.showingSplashScreen)
            {
                // clear the "visible" area if we are showing the splash screen
                this.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, SplashBackgroundColor, 1.0f, 0);
            }

            this.device.BeginScene();
            this.device.SetRenderState(RenderStates.AlphaBlendEnable, true);

            try
            {
                this.RenderContent();

                if (this.DebugMode)
                {
                    this.device.Viewport = new Viewport
                    {
                        X = 0,
                        Y = 0,
                        Width = this.ScreenSize.Width,
                        Height = this.ScreenSize.Height
                    };
                    this.UpdateFramerate();
                    this.DrawFramerate();
                }
            }
            finally
            {
                this.device.EndScene();
                this.device.Present();
            }

            return true;
        }

        /// <summary>
        /// Loads a new screen.
        /// </summary>
        /// <param name="change">
        /// The screen change.
        /// </param>
        public void LoadScreen(ScreenChange change)
        {
            this.logger.Info("Screen changed. New screen will be shown: #{0}", change.ScreenRoot.Id);
            var root = (ScreenRoot)change.ScreenRoot.Clone();
            this.currentRoot = root;
            
            this.logger.Trace("New screen: {0}", root);

            this.logger.Debug("Loading additional fonts");
            foreach (var fontFile in change.FontFiles)
            {
                this.LoadFont(fontFile);
            }

            var manager = this.renderManagerFactory.CreateRenderManager(root);

            this.logger.Trace("Created new root render manager");

            if (this.splashTimer.Enabled)
            {
                ScreenRootRenderManager<IDxDeviceRenderContext> oldManager = null;
                lock (this.screenRenderManagers)
                {
                    if (this.splashTimer.Enabled)
                    {
                        oldManager = this.firstManager;
                        this.firstManager = manager;
                        if (oldManager == null)
                        {
                            return;
                        }
                    }
                }

                if (oldManager != null)
                {
                    oldManager.Dispose();
                    return;
                }
            }

            this.showingSplashScreen = false;
            this.ChangeRenderManagers(manager, change.Animation);

            this.logger.Trace("Finished switching to new screen");
        }

        /// <summary>
        /// Updates the current screen.
        /// </summary>
        /// <param name="updates">
        /// The screen updates
        /// </param>
        public void UpdateScreens(IList<ScreenUpdate> updates)
        {
            this.logger.Trace("Updating render managers with {0} updates", updates.Count);

            var initial = this.firstManager;
            lock (this.screenRenderManagers)
            {
                if (initial != null)
                {
                    initial.Update(updates);
                }

                this.screenRenderManagers.DoWithValues((screenRootRenderManager, alpha) =>
                    {
                        bool hadUpdates = screenRootRenderManager.Update(updates);
                        // TODO
                        if (hadUpdates)
                        {
                            foreach (var screenUpdate in updates)
                            {
                                foreach (var itemUpdate in screenUpdate.Updates)
                                {
                                    var rootItem = itemUpdate.Value as RootItem;
                                    var imageItems = rootItem.Items.Where(m => m.GetType() == typeof(ImageItem)).ToList();
                                    foreach (ImageItem imageItem in imageItems)
                                    {
                                        this.logger.Trace("Had Updates file:{0}", imageItem.Filename);
                                    }

                                    var videoItems = rootItem.Items.Where(m => m.GetType() == typeof(VideoItem)).ToList();
                                    foreach (VideoItem videoItem in videoItems)
                                    {
                                        this.logger.Trace($"Had Video Updates file: {videoItem.VideoUri}");
                                    }
                                }
                            }
                            // TODO


                        }
                    });
            }

            this.logger.Trace("Finished updating render managers");
        }

        /// <summary>
        /// Shows the fallback screen if available else the splash screen.
        /// </summary>
        public void ShowFallbackScreen()
        {
            lock (this.screenRenderManagers)
            {
                ScreenRoot root;
                if (!string.IsNullOrEmpty(this.config.FallbackImage) && File.Exists(this.config.FallbackImage))
                {
                    this.logger.Info("Showing fallback image {0}", this.config.FallbackImage);
                    root = this.CreateFallbackScreen();
                }
                else
                {
                    this.logger.Info("Showing fallback splash screen");
                    root = this.CreateSplashScreen();
                    this.showingSplashScreen = true;
                }

                var fallbackManager = this.renderManagerFactory.CreateRenderManager(root);
                this.ChangeRenderManagers(fallbackManager, null);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            if (this.currentRoot == null)
            {
                yield break;
            }

            yield return parent.Factory.CreateManagementProvider("Screen items", parent, this.currentRoot.Root);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<float>("FPS", this.currentFrameRate, true);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.backlightPort.Dispose();
                this.screenRenderManagers.Release();
                this.device.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            if (this.focusThief == IntPtr.Zero && this.WindowMode == WindowMode.FullScreenWindowed)
            {
                this.focusThief = User32.GetForegroundWindow();
                this.logger.Warn(
                    "Focus to fullscreen window was stolen by {0:X8}", this.focusThief.ToInt32());
            }
        }

        private ScreenRoot CreateSplashScreen()
        {
            var root = new RootItem { Id = -1, Width = this.viewport.Width, Height = this.viewport.Height };
#if __UseLuminatorTftDisplay
            var logo = Resources.Luminator;
#else
            var logo = Resources.GorbaLogo;
#endif

            var logoHeight = Math.Min((root.Height / 4) - 50, logo.Height);

            root.Items.Add(
                new ImageItem
                {
#if __UseLuminatorTftDisplay
                    Filename = ImageSprite.ResourcePrefix + "Luminator",
#else
           Filename = ImageSprite.ResourcePrefix + "GorbaLogo",
#endif
                   // Filename = ImageSprite.ResourcePrefix + "GorbaLogo",
                    Scaling = ElementScaling.Scale,
                    X = 0,
                    Y = (root.Height / 4) - logoHeight,
                    Width = root.Width,
                    Height = logoHeight
                });

            root.Items.Add(
                new TextItem
                {
#if __UseLuminatorTftDisplay
                    Text = "INFOTransit",
#else
                    Text = "Infomedia",
#endif
                    Font =
                            new Font
                            {
                                Height = root.Height / 5,
                                Weight = 900,
                                Face = "Arial",
                                Color = "Black"
                            },
                    Align = HorizontalAlignment.Center,
                    VAlign = VerticalAlignment.Middle,
                    X = 0,
                    Y = 0,
                    Width = root.Width,
                    Height = root.Height
                });

            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                root.Items.Add(
                    new TextItem
                    {
                        Text = versionInfo.FileVersion,
                        Font = new Font { Height = root.Height / 15, Weight = 400, Face = "Arial", Color = "Black" },
                        Align = HorizontalAlignment.Center,
                        X = 0,
                        Y = (int)(root.Height / 1.3),
                        Width = root.Width
                    });
            }
            catch (Exception ex)
            {
                this.logger.Warn(ex, "Couldn't load player version");
            }

            this.ShowAcapelaBranding(root);
            
            return new ScreenRoot { Root = root };
        }

        /// <summary>
        /// If we are using Acapela, show brand recognition
        /// </summary>
        /// <param name="root">The graphical element containing this text item.</param>
        private void ShowAcapelaBranding(RootItem root)
        {
            try
            {
                if (!this.IsUsingAcapela())
                {
                    return;
                }
            
                // Determine if we are using Acapela
                root.Items.Add(
                    new TextItem
                        {
                            Text = "TTS Audio powered by Acapela",
                            Font = new Font { Height = root.Height / 15, Weight = 400, Face = "Arial", Color = "Black" },
                            Align = HorizontalAlignment.Left,
                            X = root.Height / 30,
                            Y = (int)(root.Height - (root.Height / 15)),
                            Width = root.Height / 2
                        });
            }
            catch (Exception e)
            {
                this.logger.Warn($"Unable to retrieve Audio configuration. {e.Message}");
            }
        }

        private bool IsUsingAcapela()
        {
            var configMgr = new ConfigManager<AudioRendererConfig>
                                {
                                    FileName = PathManager.Instance.GetPath(FileType.Config, "AudioRenderer.xml"),
                                    EnableCaching = true,
                                    XmlSchema = AudioRendererConfig.Schema
                                };
            var audioConfig = configMgr.Config;

            if (audioConfig.TextToSpeech != null && audioConfig.TextToSpeech.Api == TextToSpeechApi.Acapela)
            {
                return true;
            }

            return false;
        }

        private void ChangeRenderManagers(
            ScreenRootRenderManager<IDxDeviceRenderContext> manager, PropertyChangeAnimation animation)
        {
            lock (this.screenRenderManagers)
            {
#if __UseLuminatorTftDisplay
                this.Invoke(new Action(() =>
                {
                    this.screenRenderManagers.Animate(animation, manager);
                }));

#else
                this.screenRenderManagers.Animate(animation, manager);
#endif
            }
        }

        private ScreenRoot CreateFallbackScreen()
        {
            var root = new RootItem { Id = -1, Width = this.viewport.Width, Height = this.viewport.Height };
            root.Items.Add(
                 new ImageItem
                 {
                     Filename = this.config.FallbackImage,
                     Scaling = ElementScaling.Stretch,
                     X = 0,
                     Y = 0,
                     Width = root.Width,
                     Height = root.Height
                 });

            return new ScreenRoot { Root = root };
        }

        /// <summary>
        /// Resets the device using the given render mode.
        /// </summary>
        /// <param name="shouldResetDevice">
        /// Flag indicating if the device should be reset.
        /// </param>
        /// <returns>
        /// True if the device was successfully reset.
        /// </returns>
        private bool ResetWindow(bool shouldResetDevice)
        {
            var presentParams = this.CreatePresentParams();

            Rectangle bounds;

#if __UseLuminatorTftDisplay
            this.BackColor = DefaultBackgroundColor;
#endif

            if (this.windowMode == WindowMode.Windowed)
            {
                var screenSize = this.screen.Bounds.Size;
                bounds = new Rectangle(
                    this.screen.Bounds.X, this.screen.Bounds.Y, this.ScreenSize.Width, this.ScreenSize.Height);
                while (bounds.Width >= screenSize.Width || bounds.Height >= screenSize.Height)
                {
                    bounds.Width /= 2;
                    bounds.Height /= 2;
                }

                // add window frame
                bounds.Width += this.Size.Width - this.ClientSize.Width;
                bounds.Height += this.Size.Height - this.ClientSize.Height;

                bounds.X += (screenSize.Width - bounds.Width) / 2;
                bounds.Y += (screenSize.Height - bounds.Height) / 2;

                this.Cursor = Cursors.Default;

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Bounds = bounds;
            }
            else
            {
                bounds = this.screen.Bounds;
                var cursor = Cursors.No;
                cursor.Dispose();
                this.Cursor = cursor;

                // Attention: the order of setting these properties is crucial!
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.Bounds = bounds;

                // this is needed so an initial fullscreen window is shown on the correct screen:
                this.StartPosition = FormStartPosition.Manual;
            }

            var success = true;
            if (this.device == null)
            {
                this.logger.Info("Creating device");
                this.device = this.CreateDevice(presentParams);
            }
            else
            {
                this.logger.Debug("Resetting device");
                try
                {
                    if (shouldResetDevice)
                    {
                        this.device.Reset(presentParams);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    this.logger.Error(ex, "Couldn't reset the device");
                }
            }

            this.Bounds = bounds;

#if __UseLuminatorTftDisplay
            this.Invoke(new Action(() =>
            {
#endif
                this.TopMost = this.windowMode != WindowMode.Windowed;

                if (success)
                {
                    this.focusThief = IntPtr.Zero;
                    this.BringToFront();
                    this.Focus();
                }
#if __UseLuminatorTftDisplay
            }));
#endif

            return success;
        }

        private Device CreateDevice(PresentParameters presentParams)
        {
            var caps = Manager.GetDeviceCaps(this.Adapter.Adapter, DeviceType.Hardware);

            CreateFlags flags = 0;
            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
            {
                flags |= CreateFlags.HardwareVertexProcessing;
            }
            else
            {
                flags |= CreateFlags.SoftwareVertexProcessing;
            }

            var dev = new Device(this.Adapter.Adapter, DeviceType.Hardware, this, flags, presentParams);
            dev.DeviceLost += this.DeviceOnDeviceLost;
            dev.DeviceReset += this.DeviceOnDeviceReset;
            dev.Disposing += this.DeviceOnDisposing;

            // prevent resizing (used in windowed mode to prevent changing the size of the device)
            dev.DeviceResizing += (s, e) => e.Cancel = true;

            // from http://www.vbforums.com/showthread.php?664787-RESOLVED-DirectX-Rotated-Text
            dev.SetRenderState(RenderStates.ZEnable, true);
            dev.SetRenderState(RenderStates.Lighting, 0);
            dev.SetRenderState(RenderStates.CullMode, (int)Cull.CounterClockwise);
            dev.SetRenderState(RenderStates.BlendOperation, (int)BlendOperation.Add);
            dev.SetSamplerState(0, SamplerStageStates.MagFilter, (int)Filter.Linear);
            dev.SetSamplerState(0, SamplerStageStates.MinFilter, (int)Filter.Linear);

            return dev;
        }

        private PresentParameters CreatePresentParams()
        {
            var presentParams = new PresentParameters
            {
                Windowed = this.windowMode != WindowMode.FullScreenExclusive,
                ForceNoMultiThreadedFlag = !this.deviceConfig.MultiThreaded,
                MultiSampleQuality = this.deviceConfig.MultiSampleQuality,
                BackBufferCount = 1,
                BackBufferWidth = this.ScreenSize.Width,
                BackBufferHeight = this.ScreenSize.Height,
                BackBufferFormat = this.Adapter.CurrentDisplayMode.Format,
                AutoDepthStencilFormat = DepthFormat.D16,
                EnableAutoDepthStencil = true,
                PresentFlag = (PresentFlag)this.deviceConfig.PresentFlag
            };
            this.SetMultiSampleType(presentParams);

            switch (this.deviceConfig.PresentationInterval)
            {
                case PresentIntervals.Immediate:
                    presentParams.PresentationInterval = PresentInterval.Immediate;
                    break;
                case PresentIntervals.Four:
                    presentParams.PresentationInterval = PresentInterval.Four;
                    break;
                case PresentIntervals.Three:
                    presentParams.PresentationInterval = PresentInterval.Three;
                    break;
                case PresentIntervals.Two:
                    presentParams.PresentationInterval = PresentInterval.Two;
                    break;
                case PresentIntervals.One:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (this.deviceConfig.SwapEffect)
            {
                case SwapEffects.Copy:
                    presentParams.SwapEffect = SwapEffect.Copy;
                    break;
                case SwapEffects.Flip:
                    presentParams.SwapEffect = SwapEffect.Flip;
                    break;
                default:
                    presentParams.SwapEffect = SwapEffect.Discard;
                    break;
            }

            return presentParams;
        }

        private void SetMultiSampleType(PresentParameters presentParams)
        {
            switch (this.deviceConfig.MultiSample)
            {
                case MultiSampleTypes.SixteenSamples:
                    presentParams.MultiSample = MultiSampleType.SixteenSamples;
                    break;
                case MultiSampleTypes.FifteenSamples:
                    presentParams.MultiSample = MultiSampleType.FifteenSamples;
                    break;
                case MultiSampleTypes.FourteenSamples:
                    presentParams.MultiSample = MultiSampleType.FourteenSamples;
                    break;
                case MultiSampleTypes.ThirteenSamples:
                    presentParams.MultiSample = MultiSampleType.ThirteenSamples;
                    break;
                case MultiSampleTypes.TwelveSamples:
                    presentParams.MultiSample = MultiSampleType.TwelveSamples;
                    break;
                case MultiSampleTypes.ElevenSamples:
                    presentParams.MultiSample = MultiSampleType.ElevenSamples;
                    break;
                case MultiSampleTypes.TenSamples:
                    presentParams.MultiSample = MultiSampleType.TenSamples;
                    break;
                case MultiSampleTypes.NineSamples:
                    presentParams.MultiSample = MultiSampleType.NineSamples;
                    break;
                case MultiSampleTypes.EightSamples:
                    presentParams.MultiSample = MultiSampleType.EightSamples;
                    break;
                case MultiSampleTypes.SevenSamples:
                    presentParams.MultiSample = MultiSampleType.SevenSamples;
                    break;
                case MultiSampleTypes.SixSamples:
                    presentParams.MultiSample = MultiSampleType.SixSamples;
                    break;
                case MultiSampleTypes.FiveSamples:
                    presentParams.MultiSample = MultiSampleType.FiveSamples;
                    break;
                case MultiSampleTypes.FourSamples:
                    presentParams.MultiSample = MultiSampleType.FourSamples;
                    break;
                case MultiSampleTypes.ThreeSamples:
                    presentParams.MultiSample = MultiSampleType.ThreeSamples;
                    break;
                case MultiSampleTypes.TwoSamples:
                    presentParams.MultiSample = MultiSampleType.TwoSamples;
                    break;
                case MultiSampleTypes.NonMaskable:
                    presentParams.MultiSample = MultiSampleType.NonMaskable;
                    break;
                default:
                    presentParams.MultiSample = MultiSampleType.None;
                    break;
            }
        }

        private void RenderContent()
        {
            lock (this.screenRenderManagers)
            {
                this.screenRenderManagers.Update(this.renderContext);

                var backlight = this.GetBacklightValue();
                if (this.backlightPort.Port != null && backlight != this.lastBacklightValue)
                {
                    this.logger.Info("Changing backlight to {0}", backlight);
                    this.lastBacklightValue = backlight;
                    this.backlightPort.Value = FlagValues.GetValue(backlight);
                }

                this.screenRenderManagers.DoWithValues(
                    (m, a) =>
                    {
                        m.Update(this.renderContext);
                        m.Render(a, this.renderContext);
                    });
            }
        }

        private bool CheckCooperativeLevel()
        {
            int levelResult;
            if (this.device.CheckCooperativeLevel(out levelResult))
            {
                // everything is OK, let's render the content
                var foregroundWindow = User32.GetForegroundWindow();
                if (this.focusThief != IntPtr.Zero && foregroundWindow == this.Handle)
                {
                    var resetDevice = this.ResetWindow(false);
                    this.logger.Info("Render Window given focus back was {0}", resetDevice);
                }

                return true;
            }

            if (levelResult == -2005530519)
            {
                // -2005530519 = 0x88760869 = D3DERR_DEVICENOTRESET
                this.logger.Info("Resetting device because of D3DERR_DEVICENOTRESET");
                this.device.Reset(this.CreatePresentParams());
                this.focusThief = IntPtr.Zero;
                return true;
            }

            if (levelResult != -2005530520)
            {
                // -2005530520 = 0x88760868 = D3DERR_DEVICELOST
                return false;
            }

            if (this.focusThief == IntPtr.Zero)
            {
                this.focusThief = User32.GetForegroundWindow();
                this.logger.Warn(
                    "Focus to fullscreen exclusive window was stolen by {0:X8}", this.focusThief.ToInt32());
                return false;
            }

            var style = NativeMethods.GetWindowLong(this.focusThief, NativeMethods.GWL_STYLE);
            this.logger.Trace("Focus thief {0:X8} style is: {1:X8}", this.focusThief.ToInt32(), style);
            if ((style & NativeMethods.WS_VISIBLE) == NativeMethods.WS_VISIBLE)
            {
                return false;
            }

            return this.ResetWindow(true);
        }

        private void LoadFont(string fontFile)
        {
            // convert from Infomedia format (delimiter: ";") to GDI format (delimiter: "|")
            fontFile = string.Join("|", fontFile.Split(';'));

            this.logger.Trace("Loading font {0}", fontFile);
            if (this.loadedFonts.Find(f => f.Equals(fontFile, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                this.logger.Trace("Font already exists");
                return;
            }

            try
            {
                var loaded = NativeMethods.AddFontResourceEx(fontFile, NativeMethods.FR_PRIVATE, IntPtr.Zero);
                this.logger.Info("Loaded {0} font(s) from {1}", loaded, fontFile);
                this.loadedFonts.Add(fontFile);
            }
            catch (Exception ex)
            {
                this.logger.Warn(ex, "Couldn't load font File " + fontFile);
            }
        }

        private bool GetBacklightValue()
        {
            var oldVisible = this.screenRenderManagers.OldValue != null && this.screenRenderManagers.OldValue.Visible;
            var newVisible = this.screenRenderManagers.NewValue == null || this.screenRenderManagers.NewValue.Visible;
            return oldVisible || newVisible;
        }

        private void UpdateFramerate()
        {
            this.frameCounter++;
            this.logFrameCounter++;

            // int tickCount = Environment.TickCount;
            int tickCount = Gorba.Common.Utility.Win32.Api.DLLs.Winmm.TimerGetTime();
            if (tickCount - this.lastTickCount <= 200)
            {
                return;
            }

            var frameRate = (float)this.frameCounter * 1000 / Math.Abs(tickCount - this.lastTickCount);
            this.lastTickCount = tickCount;
            this.frameCounter = 0;

            if (frameRate < 1)
            {
                return;
            }

            this.lastFrameRate = string.Format(
                "Screen {0} ({1}x{2}) - [{3},{4},{5},{6}] - {7} - {8:0} fps",
                this.Adapter.Adapter,
                this.ScreenSize.Width,
                this.ScreenSize.Height,
                this.VisibleRegion.X,
                this.VisibleRegion.Y,
                this.VisibleRegion.Width,
                this.VisibleRegion.Height,
                this.windowMode,
                frameRate);

#if DEBUG
            this.lastFrameRate += string.Format(
                "\n{0}\n{1}",
                this.Adapter.CurrentDisplayMode.ToString(),
                this.JitterStats);
#endif

            if (tickCount - this.lastLogTickCount <= 1000)
            {
                return;
            }

            frameRate = (float)this.logFrameCounter * 1000 / Math.Abs(tickCount - this.lastLogTickCount);
            this.currentFrameRate = frameRate;
            this.lastLogTickCount = tickCount;
            this.logFrameCounter = 0;

            this.logger.Trace("Frame rate is: {0:0.00} fps", frameRate);
        }

        private void DrawFramerate()
        {
            const int X = 10;
            const int Y = 10;

            if (this.fpsSprite == null)
            {
                this.fpsSprite = new Sprite(this.device);

                this.fpsFont = this.renderContext.GetFontInfo(
                    24,
                    0,
                    FontWeight.Bold,
                    10,
                    false,
                    CharacterSet.Default,
                    Precision.Default,
                    FontQuality.AntiAliased,
                    PitchAndFamily.DefaultPitch,
                    "Arial");
            }

            this.fpsSprite.Begin(SpriteFlags.SortTexture | SpriteFlags.AlphaBlend);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X - 1, Y, Color.FromArgb(192, 0, 0, 0));
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X, Y - 1, Color.FromArgb(192, 0, 0, 0));
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X + 1, 10, Color.FromArgb(192, 0, 0, 0));
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X, Y + 1, Color.FromArgb(192, 0, 0, 0));
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, 10, 10, Color.FromArgb(192, 255, 255, 255));
            this.fpsSprite.End();
        }

        private void HideSplashScreen()
        {
            lock (this.screenRenderManagers)
            {
                this.splashTimer.Enabled = false;

                if (this.firstManager != null)
                {
                    this.showingSplashScreen = false;
                    this.ChangeRenderManagers(this.firstManager, null);
                    this.firstManager = null;
                }
            }
        }

        private void SplashTimerOnTick(object sender, EventArgs eventArgs)
        {
            this.HideSplashScreen();
        }

        private void DeviceOnDeviceLost(object s, EventArgs e)
        {
            if (this.fpsSprite == null || this.fpsSprite.Disposed)
            {
                return;
            }

            this.fpsSprite.OnLostDevice();
        }

        private void DeviceOnDeviceReset(object s, EventArgs e)
        {
            if (this.fpsSprite == null)
            {
                return;
            }

            this.fpsSprite.OnResetDevice();
        }

        private void DeviceOnDisposing(object s, EventArgs e)
        {
            if (this.fpsSprite == null)
            {
                return;
            }

            this.fpsSprite.Dispose();
        }
    }
}
