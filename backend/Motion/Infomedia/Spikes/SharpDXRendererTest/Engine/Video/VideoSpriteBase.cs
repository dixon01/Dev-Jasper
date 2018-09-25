// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSpriteBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoSpriteBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Video
{
    using System;
    using System.Drawing;
    using System.IO;

    using Gorba.Motion.Infomedia.SharpDXRendererTest.Config;

    using NLog;

    using SharpDX.Direct3D9;

    /// <summary>
    /// Base class for all video sprite implementations.
    /// </summary>
    public abstract class VideoSpriteBase
    {
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSpriteBase"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected VideoSpriteBase(Device device)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName);
            this.Device = device;
        }

        /// <summary>
        /// Gets the file name of the currently displayed image.
        /// </summary>
        public string CurrentVideoUri { get; private set; }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public abstract Size Size { get; }

        /// <summary>
        /// Gets the device.
        /// </summary>
        protected Device Device { get; private set; }

        /// <summary>
        /// Factory method to create a video sprite
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <returns>
        /// A new <see cref="VideoSpriteBase"/> implementation.
        /// </returns>
        public static VideoSpriteBase Create(Device device)
        {
            var videoMode = ConfigService.Instance.Config.Video.VideoMode;
            switch (videoMode)
            {
                case VideoMode.DirectShow:
                    return new DirectShowVideoSprite(device);
                case VideoMode.DirectXWindow:
                    //return new WindowedVideoSprite(device);
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
        public void Setup(string videoUri)
        {
            if (videoUri == this.CurrentVideoUri)
            {
                return;
            }

            this.CurrentVideoUri = videoUri;

            this.ReleaseVideo();

            if (!File.Exists(videoUri))
            {
                this.logger.Warn("Video doesn't exist: {0}", videoUri);
                return;
            }

            this.logger.Debug("Creating video: {0}", videoUri);
            this.CreateVideo(videoUri);
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="alpha">
        ///     The alpha.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        public abstract void Render(Rectangle bounds, int alpha, IDxRenderContext context);

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public virtual void OnResetDevice()
        {
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public virtual void OnLostDevice()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
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
        /// Releases the video.
        /// </summary>
        protected abstract void ReleaseVideo();
    }
}