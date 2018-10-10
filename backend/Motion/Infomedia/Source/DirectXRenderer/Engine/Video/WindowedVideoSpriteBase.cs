// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowedVideoSpriteBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowedVideoSpriteBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using NLog;

    /// <summary>
    /// Base class for video sprites that use separate windows to render the video.
    /// </summary>
    public abstract class WindowedVideoSpriteBase : VideoSpriteBase
    {
        private static readonly Rectangle InvalidBounds = new Rectangle(-1, -1, -1, -1);

#if __UseLuminatorTftDisplay
        private static readonly Color DefaultBackgroundColor = Color.Black;
#endif

        private readonly Control rootControl;

        private readonly Form rootForm;

        private Form form;

        private Rectangle oldBounds = InvalidBounds;

        private bool isPlaying;

        private bool isShowing;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowedVideoSpriteBase"/> class.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        protected WindowedVideoSpriteBase(IDxDeviceRenderContext renderContext)
            : base(renderContext)
        {
            this.rootControl = this.Device.CreationParameters.FocusWindow;


#if __UseLuminatorTftDisplay
            this.rootControl.BackColor = DefaultBackgroundColor;
#endif

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
        /// The video URI.
        /// </param>
        /// <param name="owner">
        /// The owner form on which the window will be displayed.
        /// </param>
        protected abstract void CreateVideo(string filename, Form owner);

        /// <summary>
        /// Starts playing the video.
        /// </summary>
        protected abstract void PlayVideo();

        /// <summary>
        /// Creates the video.
        /// </summary>
        /// <param name="filename">
        /// The videoUri.
        /// </param>
        protected override void CreateVideo(string filename)
        {
            this.form = new Form();
            this.form.ShowInTaskbar = false;
            this.form.FormBorderStyle = FormBorderStyle.None;
            this.form.StartPosition = FormStartPosition.Manual;
            this.form.GotFocus += this.FormOnGotFocus;

            this.oldBounds = InvalidBounds;

            this.CreateVideo(filename, this.form);

            var cursor = Cursors.No;
            cursor.Dispose();
            this.form.Cursor = cursor;
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
            if (this.form == null)
            {
                return;
            }

            if (!bounds.Location.Equals(this.oldBounds.Location))
            {
                // update the location using the screen offset and the scaling
                var screenScaling = this.GetScreenScaling();
                var screenOffset = this.GetScreenOffset();
                this.form.Location = new Point(
                    screenOffset.X + (int)(bounds.X * screenScaling.X),
                    screenOffset.Y + (int)(bounds.Y * screenScaling.Y));
            }

            if (!bounds.Size.Equals(this.oldBounds.Size))
            {
                // update the size using the scaling
                var screenScaling = this.GetScreenScaling();
                this.form.Size = new Size(
                    (int)(bounds.Width * screenScaling.X),
                    (int)(bounds.Height * screenScaling.Y));
            }

            if (!this.isPlaying)
            {
                if (alpha > 0)
                {
                    if (this.rootForm != null)
                    {
#if __UseLuminatorTftDisplay
                        if (this.form.IsHandleCreated)
                        {
                            if (this.form.InvokeRequired)
                            {
                                this.form.Invoke(
                                    new Action(
                                        () =>
                                            {
                                                this.form.TopMost = this.rootForm.TopMost;
                                            }));
                            }
                            else
                            {
                                this.form.TopMost = this.rootForm.TopMost;
                            }
                        }
                        else
                        {  
                            this.form.TopMost = this.rootForm.TopMost;
                        }
#else
                        this.form.TopMost = this.rootForm.TopMost;
#endif
                    }

                    this.ShowForm();
                    this.isPlaying = true;
                    this.PlayVideo();
                    this.RaiseVideoStarted(EventArgs.Empty);
                }
            }
            else if (alpha > 0)
            {
                this.ShowForm();
            }
            else
            {
                this.HideForm();
            }

            this.oldBounds = bounds;
        }

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected override void ReleaseVideo()
        {
            if (this.form == null)
            {
                return;
            }

            this.form.GotFocus -= this.FormOnGotFocus;
            this.HideForm();
            this.isPlaying = false;

            this.form.Dispose();
            this.form = null;
        }

        private void ShowForm()
        {
            if (this.isShowing)
            {
                return;
            }
            try
            {
                if (this.form != null)
                {
                    if (this.form.IsHandleCreated)
                    {
                        if (this.form.InvokeRequired)
                        {
                            this.form.BeginInvoke(
                                new Action(
                                    () =>
                                        {
                                            this.form.Show(this.rootControl);
                                            this.form.BringToFront();
                                        }));
                        }
                        else
                        {
                            this.form.Show(this.rootControl);
                            this.form.BringToFront();
                        }
                    }
                    else
                    {
                        this.form.Show(this.rootControl);
                        this.form.BringToFront();
                    }
                }

                if (this.rootControl != null)
                {
                    if (this.rootControl.InvokeRequired)
                    {
                        this.rootControl.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        this.rootControl.Focus();
                                    }));
                    }
                    else
                    {
                        this.rootControl.Focus();
                    }
                }
            }
            finally
            {
                this.isShowing = true;
            }
        }


        protected void HideForm()
        {
            if (!this.isShowing || this.form == null)
            {
                return;
            }

            try
            {
#if __UseLuminatorTftDisplay
                if (this.form.IsHandleCreated)
                {
                    if (this.form.InvokeRequired)
                    {
                        this.form.Invoke(
                            new Action(
                                () =>
                                {
                                    this.form.Hide();
                                }));
                    }
                    else
                    {
                        this.form.Hide();
                    }
                }
                else
                {
                    this.form.Hide();
                }

#else
            this.form.Hide();
#endif
            }
            finally
            {
                this.isShowing = false;
            }
        }

        private Point GetScreenOffset()
        {
            return this.rootControl.PointToScreen(Point.Empty);
        }

        private PointF GetScreenScaling()
        {
            var clientRect = this.rootControl.ClientRectangle;
            var presParams = this.Device.PresentationParameters;
            return new PointF(
                (float)clientRect.Width / presParams.BackBufferWidth,
                (float)clientRect.Height / presParams.BackBufferHeight);
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
            if (this.form != null && !this.ShowFallbackImage)
            {
                if (this.form.IsHandleCreated)
                {
                    if (this.form.InvokeRequired)
                    {
                        this.form.BeginInvoke(new Action(() =>
                        {
                            this.form.Visible = true;
                        }));
                    }
                    else
                    {
                        this.form.Visible = true;
                    }
                }
            }
        }

        private void RootFormOnResizeBegin(object sender, EventArgs eventArgs)
        {
            if (this.form != null && this.form.IsHandleCreated)
            {
                if (this.form.InvokeRequired)
                {
                    this.form.BeginInvoke(new Action(() =>
                    {
                        this.form.Visible = false;
                    }));
                }
                else
                {
                    this.form.Visible = false;
                }
            }
        }

        private void FormOnGotFocus(object sender, EventArgs eventArgs)
        {
            if (this.rootControl != null && this.rootControl.IsHandleCreated)
            {
                if (this.rootControl.InvokeRequired)
                {
                    this.rootControl.BeginInvoke(
                        new Action(
                            () =>
                                {
                                    this.rootControl.Focus();
                                }));
                }
                else
                {
                    this.rootControl.Focus();
                }
            }
        }
    }
}