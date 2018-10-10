// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXVideoSprite.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXVideoSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.DirectX.AudioVideoPlayback;

    using Video = Microsoft.DirectX.AudioVideoPlayback.Video;

    /// <summary>
    /// A video sprite that uses DirectX in separate windows to render the video.
    /// </summary>
    public class DirectXVideoSprite : WindowedVideoSpriteBase
    {
        private Video video;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXVideoSprite"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public DirectXVideoSprite(IDxDeviceRenderContext device)
            : base(device)
        {
        }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public override Size Size
        {
            get
            {
                return this.video == null ? Size.Empty : this.video.DefaultSize;
            }
        }

        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="filename">
        /// The video URI.
        /// </param>
        /// <param name="owner">
        /// The owner form on which the window will be displayed.
        /// </param>
        protected override void CreateVideo(string filename, Form owner)
        {
            this.video = new Video(filename);
            this.video.HideCursor();
            this.video.Ending += this.VideoOnEnding;

            this.video.Owner = owner;
        }

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
        protected override void Render(Rectangle bounds, int alpha, IDxRenderContext context)
        {
            if (this.video != null && this.video.Playing)
            {
                // the video is playing. I pause a little bit
                // this thread in order to give "breath" to the one
                // that is playing.
                Thread.Sleep(10);
            }

            base.Render(bounds, alpha, context);
        }

        /// <summary>
        /// Starts playing the video.
        /// </summary>
        protected override void PlayVideo()
        {
            this.video.Play();
        }

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected override void ReleaseVideo()
        {
#if __UseLuminatorTftDisplay
            HideForm();
#endif

            if (this.video != null)
            {
                this.video.Ending -= this.VideoOnEnding;
                this.video.Stop();
                this.video.Dispose();
                this.video = null;
            }

            base.ReleaseVideo();
        }

        private void VideoOnEnding(object sender, EventArgs e)
        {
            if (this.IsLooping)
            {
                this.video.SeekCurrentPosition(0, SeekPositionFlags.AbsolutePositioning);
            }
            else
            {
                this.video.Pause();
                this.RaiseVideoEnded(EventArgs.Empty);
            }
        }
    }
}