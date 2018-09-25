// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowedVideoSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowedVideoSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    /*
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.DirectX.AudioVideoPlayback;
    using Microsoft.DirectX.Direct3D;

    using Video = Microsoft.DirectX.AudioVideoPlayback.Video;

    /// <summary>
    /// A video sprite that uses separate windows to render the video.
    /// </summary>
    public class WindowedVideoSprite : VideoSpriteBase
    {
        private static readonly Rectangle InvalidBounds = new Rectangle(-1, -1, -1, -1);

        private readonly Control rootControl;

        private readonly Form rootForm;

        private Video video;

        private Form form;

        private Rectangle oldBounds = InvalidBounds;

        private bool isVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowedVideoSprite"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public WindowedVideoSprite(Device device)
            : base(device)
        {
            this.rootControl = device.CreationParameters.FocusWindow;

            this.rootControl.LocationChanged += this.RootControlOnLocationChanged;
            this.rootControl.SizeChanged += this.RootControlOnSizeChanged;

            this.rootForm = this.rootControl.FindForm();
            if (this.rootForm == null)
            {
                return;
            }

            this.rootForm.ResizeBegin += this.RootFormOnResizeBegin;
            this.rootForm.ResizeEnd += this.RootFormOnResizeEnd;
        }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public override Size Size
        {
            get
            {
                return this.video == null ? Size.Empty : this.video.Size;
            }
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
        public override void Render(Rectangle bounds, int alpha, IDxRenderContext context)
        {
            if (this.form == null)
            {
                return;
            }

            if (this.video.Playing)
            {
                // the video is playing. I pause a little bit
                // this thread in order to give "breath" to the one
                // that is playing.
                System.Threading.Thread.Sleep(10);
            }

            if (!bounds.Location.Equals(this.oldBounds.Location))
            {
                // update the location using the screen offset and the scaling
                var scaling = this.GetScaling();
                var screenOffset = this.GetScreenOffset();
                this.form.Location = new Point(
                    screenOffset.X + (int)(bounds.X * scaling.X),
                    screenOffset.Y + (int)(bounds.Y * scaling.Y));
            }

            if (!bounds.Size.Equals(this.oldBounds.Size))
            {
                // update the size using the scaling
                var scaling = this.GetScaling();
                this.form.Size = new Size(
                    (int)(bounds.Width * scaling.X),
                    (int)(bounds.Height * scaling.Y));
            }

            if (!this.isVisible)
            {
                this.form.Show(this.rootControl);

                this.form.BringToFront();
                this.rootControl.Focus();
                this.isVisible = true;
                this.video.Play();
            }

            this.oldBounds = bounds;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.rootControl.LocationChanged -= this.RootControlOnLocationChanged;
            this.rootControl.SizeChanged -= this.RootControlOnSizeChanged;

            if (this.rootForm == null)
            {
                return;
            }

            this.rootForm.ResizeBegin -= this.RootFormOnResizeBegin;
            this.rootForm.ResizeEnd -= this.RootFormOnResizeEnd;
        }

        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="filename">
        /// The videoUri.
        /// </param>
        protected override void CreateVideo(string filename)
        {
            this.video = new Video(filename);
            this.video.HideCursor();
            this.video.Ending += this.VideoOnEnding;

            this.form = new Form();
            this.form.ShowInTaskbar = false;
            this.form.FormBorderStyle = FormBorderStyle.None;
            this.form.GotFocus += this.FormOnGotFocus;

            this.video.Owner = this.form;

            var cursor = Cursors.No;
            cursor.Dispose();
            this.form.Cursor = cursor;
        }

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected override void ReleaseVideo()
        {
            if (this.video == null)
            {
                return;
            }

            this.form.GotFocus -= this.FormOnGotFocus;
            this.form.Hide();
            this.isVisible = false;

            this.video.Ending -= this.VideoOnEnding;
            this.video.Stop();
            this.video.Dispose();
            this.video = null;

            this.form.Dispose();
            this.form = null;
        }

        private Point GetScreenOffset()
        {
            return this.rootControl.PointToScreen(Point.Empty);
        }

        private PointF GetScaling()
        {
            var clientRect = this.rootControl.ClientRectangle;
            var displayMode = this.Device.DisplayMode;
            return new PointF(
                (float)clientRect.Width / displayMode.Width,
                (float)clientRect.Height / displayMode.Height);
        }

        private void RootControlOnLocationChanged(object sender, EventArgs e)
        {
            this.oldBounds.Location = new Point(-1, -1);
        }

        private void RootControlOnSizeChanged(object sender, EventArgs eventArgs)
        {
            this.oldBounds = InvalidBounds;
        }

        private void RootFormOnResizeEnd(object sender, EventArgs e)
        {
            if (this.form != null)
            {
                this.form.Visible = true;
            }
        }

        private void RootFormOnResizeBegin(object sender, EventArgs eventArgs)
        {
            if (this.form != null)
            {
                this.form.Visible = false;
            }
        }

        private void VideoOnEnding(object sender, EventArgs e)
        {
            this.video.SeekCurrentPosition(0, SeekPositionFlags.AbsolutePositioning);
        }

        private void FormOnGotFocus(object sender, EventArgs eventArgs)
        {
            this.rootControl.Focus();
        }
    }*/
}