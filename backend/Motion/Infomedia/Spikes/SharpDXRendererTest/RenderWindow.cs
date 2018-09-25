// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderWindow.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Config;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine;

    using NLog;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// The render window.
    /// </summary>
    public sealed partial class RenderWindow : Form
    {
        private readonly Logger logger;

        private readonly IDxRenderContext renderContext;

        private readonly Direct3D direct3D;

        private readonly Rectangle viewport;
        private readonly Screen screen;

        private readonly RenderManagerFactory renderManagerFactory;

        private readonly AlphaAnimator<RootRenderManager<IDxRenderContext>> rootRenderManagers =
            new AlphaAnimator<RootRenderManager<IDxRenderContext>>(null);

        private WindowMode windowMode;
        private Device device;

        private Sprite fpsSprite;
        private FontEx fpsFont;

        private long frameCounter;
        private long logFrameCounter;
        private int lastTickCount;
        private int lastLogTickCount;
        private string lastFrameRate = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderWindow"/> class.
        /// </summary>
        /// <param name="direct3D"></param>
        /// <param name="adapter">
        ///     The adapter.
        /// </param>
        /// <param name="viewport">
        ///     The viewport of this window into the entire render area.
        /// </param>
        /// <param name="windowMode">
        ///     The render mode.
        /// </param>
        /// <param name="renderContext">
        ///     The render context.
        /// </param>
        public RenderWindow(Direct3D direct3D, AdapterInformation adapter, Rectangle viewport, WindowMode windowMode, IDxRenderContext renderContext)
        {
            this.logger = LogManager.GetLogger(string.Format("{0}<{1}>", this.GetType().FullName, adapter.Adapter));
            this.direct3D = direct3D;
            this.Adapter = adapter;
            this.viewport = viewport;
            this.windowMode = windowMode;
            this.renderContext = renderContext;

            this.logger.Info(
                "Creating render window for adapter {0}: {1}x{2}", adapter.Adapter, viewport.Width, viewport.Height);

            this.InitializeComponent();

            var fileVersion = FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location);
            var version = new Version(fileVersion.FileVersion);
            this.Text = string.Format("{0} {1}", this.Text, version.ToString(3));

            this.screen = Array.Find(
                Screen.AllScreens,
                s =>
                s.DeviceName.StartsWith(adapter.Details.DeviceName, StringComparison.InvariantCultureIgnoreCase));

            this.ResetDevice();

            this.renderManagerFactory = new RenderManagerFactory(this.device, this.viewport);
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
                this.ResetDevice();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the debug mode should be shown.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Gets the adapter information.
        /// </summary>
        public AdapterInformation Adapter { get; private set; }

        /// <summary>
        /// Renders everything in this window.
        /// This method has to be called regularly by the renderer.
        /// </summary>
        public void Render()
        {
            if (this.device == null || this.device.IsDisposed)
            {
                return;
            }

            var result = this.device.TestCooperativeLevel();
            if (!result.Success)
            {
                if (result.Code == ResultCode.DeviceNotReset.Result) 
                {
                    this.ResetDevice();
                }

                Thread.Sleep(1);
                return;
            }

            this.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, SharpDX.Color.Black, 1.0f, 0);
            this.device.BeginScene();

            try
            {
                lock (this.rootRenderManagers)
                {
                    this.rootRenderManagers.Update(this.renderContext);

                    this.rootRenderManagers.DoWithValues((m, a) => m.Render(a, this.renderContext));
                }

                if (this.DebugMode)
                {
                    this.UpdateFramerate();
                    this.DrawFramerate();
                }
            }
            finally
            {
                this.device.EndScene();
                this.device.Present();
            }
        }

        /// <summary>
        /// Loads a new screen.
        /// </summary>
        /// <param name="change">
        /// The screen change.
        /// </param>
        public void LoadScreen(ScreenChange change)
        {
            var manager = this.renderManagerFactory.CreateRenderManager(change.Root);
            lock (this.rootRenderManagers)
            {
                this.rootRenderManagers.Animate(change.Animation, manager);
            }
        }

        /// <summary>
        /// Updates the current screen.
        /// </summary>
        /// <param name="update">
        /// The screen update.
        /// </param>
        public void UpdateScreen(ScreenUpdate update)
        {
            lock (this.rootRenderManagers)
            {
                this.rootRenderManagers.DoWithValues((m, a) => m.Update(update));
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.rootRenderManagers.Release();
                this.device.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Resets the device using the given render mode.
        /// </summary>
        private void ResetDevice()
        {
            var config = ConfigService.Instance.Config.Device;

            var presentParams = new PresentParameters
            {
                Windowed = this.windowMode != WindowMode.FullScreenExclusive,
                SwapEffect = config.SwapEffect,

                //ForceNoMultiThreadedFlag = !config.MultiThreaded,

                PresentationInterval = config.PresentationInterval,
                PresentFlags = config.PresentFlag,

                MultiSampleType = config.MultiSample,
                MultiSampleQuality = config.MultiSampleQuality,
                
                BackBufferCount = 1,
                BackBufferWidth = this.viewport.Width,
                BackBufferHeight = this.viewport.Height,
                BackBufferFormat = this.Adapter.CurrentDisplayMode.Format
            };

            presentParams.AutoDepthStencilFormat = Format.D16;
            presentParams.EnableAutoDepthStencil = true;

            var bounds = this.screen.Bounds;
            if (this.windowMode == WindowMode.Windowed)
            {
                bounds.X += bounds.Width / 4;
                bounds.Y += bounds.Height / 4;
                bounds.Width /= 2;
                bounds.Height /= 2;
                bounds.Width += this.Size.Width - this.ClientSize.Width;
                bounds.Height += this.Size.Height - this.ClientSize.Height;

                this.Cursor = Cursors.Default;

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Bounds = bounds;
            }
            else
            {
                var cursor = Cursors.No;
                cursor.Dispose();
                this.Cursor = cursor;

                // Attention: the order of setting these properties is crucial!
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.Bounds = bounds;
            }

            if (this.device == null)
            {
                var caps = this.Adapter.GetCaps(DeviceType.Hardware);

                CreateFlags flags = 0;
                /*if ((caps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0)
                {
                    flags |= CreateFlags.HardwareVertexProcessing;
                }
                else*/
                {
                    flags |= CreateFlags.SoftwareVertexProcessing;
                }

                // TODO: do we need more than one Direct3D object?
                this.device = new Device(
                    this.direct3D, this.Adapter.Adapter, DeviceType.Hardware, this.Handle, flags, presentParams);
                //this.device.DeviceLost += this.DeviceOnDeviceLost;
                //this.device.DeviceReset += this.DeviceOnDeviceReset;
                this.device.Disposing += this.DeviceOnDisposing;

                // prevent resizing (used in windowed mode to prevent changing the size of the device)
                // TODO: we need to disable resizing somehow!
                //this.device.DeviceResizing += (s, e) => e.Cancel = true;

                this.PrepareProjectionAndLights();
            }
            else
            {
                try
                {
                    this.device.Reset(presentParams);
                }
                catch (Exception ex)
                {
                    this.logger.ErrorException("Couldn't reset the device", ex);
                }
            }

            this.device.ShowCursor = this.windowMode == WindowMode.Windowed;

            this.Bounds = bounds;
        }

        private void UpdateFramerate()
        {
            this.frameCounter++;
            this.logFrameCounter++;

            int tickCount = Environment.TickCount;
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
                "Screen {0} @[{1},{2}] ({3}x{4}) - {5} - {6:0} fps",
                this.Adapter.Adapter,
                this.viewport.X,
                this.viewport.Y,
                this.viewport.Width,
                this.viewport.Height,
                this.windowMode,
                frameRate);

            if (tickCount - this.lastLogTickCount <= 1000)
            {
                return;
            }

            frameRate = (float)this.logFrameCounter * 1000 / Math.Abs(tickCount - this.lastTickCount);
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

                this.fpsFont = FontCache.Instance.CreateFont(
                    this.device, 
                    24, 
                    0, 
                    FontWeight.Bold, 
                    10, 
                    false, 
                    FontCharacterSet.Default,
                    FontPrecision.Default, 
                    FontQuality.Antialiased,
                    FontPitchAndFamily.Default, 
                    "Arial");
            }

            if (string.IsNullOrEmpty(this.lastFrameRate))
            {
                return;
            }

            this.fpsSprite.Begin(SpriteFlags.SortTexture | SpriteFlags.AlphaBlend);
            var shadowColor = new ColorBGRA(0, 0, 0, 192);
            var mainColor = new ColorBGRA(255, 255, 255, 192);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X - 1, Y, shadowColor);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X, Y - 1, shadowColor);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X + 1, 10, shadowColor);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, X, Y + 1, shadowColor);
            this.fpsFont.Font.DrawText(this.fpsSprite, this.lastFrameRate, 10, 10, mainColor);
            this.fpsSprite.End();
        }

        private void PrepareProjectionAndLights()
        {
            /*
            this.device.Transform.Projection = Matrix.PerspectiveFovLH(
                (float)Math.PI / 4, (float)this.viewport.Width / this.viewport.Height, 0, 1);
            this.device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, -100.0f), new Vector3(), new Vector3(0, 1, 0));

            this.device.Lights[0].Type = LightType.Directional;
            this.device.Lights[0].Diffuse = Color.White;
            this.device.Lights[0].Direction = new Vector3(0, 0, 1);
            this.device.Lights[0].Update();
            this.device.Lights[0].Enabled = true;*/
        }

        private void DeviceOnDeviceLost(object s, EventArgs e)
        {
            if (this.fpsSprite == null || this.fpsSprite.IsDisposed)
            {
                return;
            }

            this.fpsSprite.OnLostDevice();
        }

        private void DeviceOnDeviceReset(object s, EventArgs e)
        {
            this.PrepareProjectionAndLights();

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
