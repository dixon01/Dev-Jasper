// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXSimulator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXSimulator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Options;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;

    using Microsoft.DirectX.Direct3D;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualBasic.Devices;

    /// <summary>
    /// Simulator that uses a DirectX window to show the contents of a layout.
    /// </summary>
    public class DirectXSimulator : ISimulator
    {
        private Thread thread;

        private Renderer renderer;

        /// <summary>
        /// Event that is fired when this simulator is stopped, either by calling
        /// <see cref="Stop"/> or if the simulator stops by itself (e.g. trough user interaction).
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets the width of the area that is simulated.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the area that is simulated.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Configures this simulator with the given size.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public void Configure(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Starts this simulator by showing the UI.
        /// </summary>
        public void Start()
        {
            if (this.thread != null)
            {
                return;
            }

            this.thread = new Thread(this.Run) { IsBackground = true };
            if (Environment.ProcessorCount < 2)
            {
                // don't use up all CPU on single-core systems
                this.thread.Priority = ThreadPriority.BelowNormal;
            }

            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.Start();
        }

        /// <summary>
        /// Stops this simulator by hiding the UI.
        /// </summary>
        public void Stop()
        {
            if (this.thread == null)
            {
                return;
            }

            this.renderer.Stop();
            this.renderer = null;

            this.thread.Join(TimeSpan.FromMilliseconds(500));
            this.thread = null;
        }

        /// <summary>
        /// Raises the <see cref="Stopped"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStopped(EventArgs e)
        {
            var handler = this.Stopped;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Run()
        {
            var config = this.GetRendererConfig();

            this.renderer = new Renderer(config);
            this.renderer.RenderContextFactory = new DeviceRenderContextFactory(this.renderer.RenderContext);
            this.renderer.Run();
            this.RaiseStopped(EventArgs.Empty);
        }

        private RendererConfig GetRendererConfig()
        {
            var mediaConfiguration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            var maxRamUsage = new ComputerInfo().AvailablePhysicalMemory * 2 / 3;
            var config = new RendererConfig();
            config.Screens.Add(new ScreenConfig { Width = this.Width, Height = this.Height });
            config.WindowMode = WindowMode.Windowed;
            config.FallbackTimeout = TimeSpan.Zero;
            config.Image = new ImageConfig
                               {
                                   BitmapCacheTimeout = TimeSpan.FromMinutes(5),
                                   MaxBitmapCacheBytes = (long)maxRamUsage,
                                   MaxCacheBytesPerBitmap = this.Width * this.Height * 8 // 2x size of screen
                               };
            var rendererOptionsCategory = state.Options.Categories.FirstOrDefault(c => c is RendererOptionCategory);
            var videoMode = VideoMode.DirectXWindow;
            var textConfig = mediaConfiguration.DirectXRendererConfig == null
                                 ? new TextConfig()
                                 : mediaConfiguration.DirectXRendererConfig.Text;
            if (rendererOptionsCategory != null)
            {
                var group = rendererOptionsCategory.Groups.First(g => g is RendererOptionGroup);
                if (@group != null)
                {
                    var rendererGroup = (RendererOptionGroup)@group;
                    if (!Enum.TryParse(rendererGroup.VideoMode, out videoMode))
                    {
                        videoMode = mediaConfiguration.DirectXRendererConfig == null
                                        ? VideoMode.DirectXWindow
                                        : mediaConfiguration.DirectXRendererConfig.Video.VideoMode;
                    }

                    TextMode textMode;
                    if (!Enum.TryParse(rendererGroup.TextMode, out textMode))
                    {
                        textMode = mediaConfiguration.DirectXRendererConfig == null
                                       ? TextMode.FontSprite
                                       : mediaConfiguration.DirectXRendererConfig.Text.TextMode;
                    }

                    FontQualities fontQuality;
                    if (!Enum.TryParse(rendererGroup.FontQuality, out fontQuality))
                    {
                        fontQuality = mediaConfiguration.DirectXRendererConfig == null
                                          ? FontQualities.ClearTypeNatural
                                          : mediaConfiguration.DirectXRendererConfig.Text.FontQuality;
                    }

                    textConfig.TextMode = textMode;
                    textConfig.FontQuality = fontQuality;
                }
            }

            config.Video.VideoMode = videoMode;
            config.Text = textConfig;
            return config;
        }

        private class DeviceRenderContextFactory : IDeviceRenderContextFactory
        {
            private readonly IDxRenderContext renderContext;

            public DeviceRenderContextFactory(IDxRenderContext renderContext)
            {
                this.renderContext = renderContext;
            }

            public DeviceConfig DeviceConfig
            {
                get
                {
                    return this.renderContext.Config.Device;
                }
            }

            public IDxDeviceRenderContext CreateContext(Device device)
            {
                return new ExtendedDeviceRenderContext(device, this.renderContext);
            }
        }

        private class ExtendedDeviceRenderContext : DeviceRenderContext
        {
            public ExtendedDeviceRenderContext(Device device, IDxRenderContext parentContext)
                : base(device, parentContext)
            {
            }

            public override IImageTexture GetImageTexture(string filename)
            {
                if (filename.Length <= 1)
                {
                    return null;
                }

                if (filename[0] == '#')
                {
                    return this.GetImageTexture(
                        new ImageKey(filename),
                        filename,
                        () => new BitmapImageTexture(LoadBitmap(filename.Substring(1)), this.Device));
                }

                return base.GetImageTexture(filename);
            }

            private static Bitmap LoadBitmap(string hash)
            {
                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                var resource = state.ProjectManager.GetResource(hash);
                using (var input = resource.OpenRead())
                {
                    return new Bitmap(input);
                }
            }
        }
    }
}