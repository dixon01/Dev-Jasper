// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSpriteBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoSpriteBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.RendererBase;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    /// <summary>
    /// Base class for all video sprite implementations.
    /// </summary>
    public abstract class VideoSpriteBase
    {
        private static readonly TimeSpan FallbackImageTimeout = TimeSpan.FromSeconds(15);

        private readonly Logger logger;

        private readonly IDxDeviceRenderContext renderContext;

        private readonly ITimer timer;

        private string fallbackImagePath;

        private ImageSprite fallbackImageSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSpriteBase"/> class.
        /// </summary>
        /// <param name="renderContext">The device render context.</param>
        protected VideoSpriteBase(IDxDeviceRenderContext renderContext)
        {
            this.renderContext = renderContext;
            this.logger = LogManager.GetLogger(this.GetType().FullName);
            this.Device = renderContext.Device;
            this.timer = TimerFactory.Current.CreateTimer("VideoSprite");
            this.timer.Interval = FallbackImageTimeout;
            this.timer.Elapsed += this.OnTimerElapsed;
        }

        /// <summary>
        /// Event that is fired when the video playback starts.
        /// </summary>
        public event EventHandler VideoStarted;

        /// <summary>
        /// Event that is fired when the video playback ends.
        /// This event might only be fired when the video stopped by itself, not by command from the outside.
        /// </summary>
        public event EventHandler VideoEnded;

        /// <summary>
        /// Gets the the file name of the currently displayed video.
        /// </summary>
        public string CurrentVideoUri { get; private set; }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public abstract Size Size { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the video should start
        /// from the beginning when it is done.
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Gets a value indicating whether the fallback image should be shown.
        /// </summary>
        protected bool ShowFallbackImage { get; private set; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        protected Device Device { get; private set; }

        /// <summary>
        /// Factory method to create a video sprite
        /// </summary>
        /// <param name="context">
        /// The render context.
        /// </param>
        /// <returns>
        /// A new <see cref="VideoSpriteBase"/> implementation.
        /// </returns>
        public static VideoSpriteBase Create(IDxDeviceRenderContext context)
        {
            var videoMode = context.Config.Video.VideoMode;
            switch (videoMode)
            {
                case VideoMode.DirectShow:
                    return new DirectShowVideoSprite(context);
                case VideoMode.DirectXWindow:
                    return new DirectXVideoSprite(context);
                case VideoMode.VlcWindow:
                    return new VlcVideoSprite(context);
                default:
                    throw new NotSupportedException("Video mode not supported: " + videoMode);
            }
        }

        /// <summary>
        /// Sets this sprite up for later rendering.
        /// This method should be called before every rendering.
        /// </summary>
        /// <param name="videoUri">
        /// The videoUri.
        /// </param>
        /// <param name="fallbackImage">
        /// The fallback image.
        /// </param>
        public void Setup(string videoUri, string fallbackImage)
        {
            if (videoUri == this.CurrentVideoUri)
            {
                return;
            }

            if (File.Exists(fallbackImage))
            {
                this.fallbackImagePath = fallbackImage;
            }
            else
            {
                this.logger.Warn("Fallback image not found: {0}", fallbackImage);
            }

            this.CurrentVideoUri = videoUri;

            this.ReleaseVideo();

            if (!videoUri.Contains("://") && !File.Exists(videoUri))
            {
                this.logger.Warn("Video doesn't exist: {0}", videoUri);
                this.UseFallbackImage(true);
                return;
            }

            this.logger.Debug("Creating video: {0}", videoUri);
            this.CreateVideo(videoUri);
            this.logger.Trace("Video created");
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="scaling">
        /// The scaling.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Render(Rectangle bounds, ElementScaling scaling, int alpha, IDxRenderContext context)
        {
            // in case of error, we render the fallback image sprite
            if (this.ShowFallbackImage)
            {
                if (alpha > 0)
                {
                    this.RenderFallbackImage(bounds, scaling, alpha, context);
                }

                return;
            }

            bounds = RenderHelper.ApplyScaling(bounds, this.Size, scaling);
            this.Render(bounds, alpha, context);
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public virtual void OnResetDevice()
        {
            if (this.fallbackImageSprite == null)
            {
                return;
            }

            this.fallbackImageSprite.OnResetDevice();
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public virtual void OnLostDevice()
        {
            if (this.fallbackImageSprite == null)
            {
                return;
            }

            this.fallbackImageSprite.OnLostDevice();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.DisposeFallbackImage();
            this.ReleaseVideo();
        }

        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="filename">
        /// The videoUri.
        /// </param>
        protected abstract void CreateVideo(string filename);

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected abstract void Render(Rectangle bounds, int alpha, IDxRenderContext context);

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected abstract void ReleaseVideo();

        /// <summary>
        /// Raises the <see cref="VideoStarted"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseVideoStarted(EventArgs e)
        {
            var handler = this.VideoStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Sets the visibility of the fallback image.
        /// </summary>
        /// <param name="useFallbackImage">
        /// A flag indicating whether the fallback image should be shown or not.
        /// </param>
        protected virtual void UseFallbackImage(bool useFallbackImage)
        {
            this.ShowFallbackImage = this.timer.Enabled = useFallbackImage;
        }

        /// <summary>
        /// Raises the <see cref="VideoEnded"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseVideoEnded(EventArgs e)
        {
            // we need to reset the URI, otherwise the same video wouldn't be played twice in a row
            this.CurrentVideoUri = null;

            var handler = this.VideoEnded;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        private void DisposeFallbackImage()
        {
            if (this.fallbackImageSprite == null)
            {
                return;
            }

            this.fallbackImageSprite.Dispose();
            this.fallbackImageSprite = null;
        }

        private void RenderFallbackImage(Rectangle bounds, ElementScaling scaling, int alpha, IDxRenderContext context)
        {
            if (this.fallbackImageSprite == null)
            {
                this.logger.Info("Creating fallback image sprite for video: {0}", this.fallbackImagePath);
                this.fallbackImageSprite = new ImageSprite(this.renderContext);
                this.fallbackImageSprite.Setup(this.fallbackImagePath);
            }

            this.fallbackImageSprite.Render(bounds, scaling, alpha, context);
        }

        private void OnTimerElapsed(object sender, EventArgs eventArgs)
        {
            this.logger.Info("Timeout for fallback image");
            this.RaiseVideoEnded(EventArgs.Empty);
        }
    }
}