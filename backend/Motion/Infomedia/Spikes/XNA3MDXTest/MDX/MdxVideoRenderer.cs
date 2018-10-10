// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MdxVideoRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest.MDX
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.DirectX.AudioVideoPlayback;

    /// <summary>
    /// The MdxVideoRenderer.
    /// </summary>
    public class MdxVideoRenderer : Form
    {
        private readonly bool enableForm;
        private Video video;

        /// <summary>
        /// Initializes a new instance of the <see cref="MdxVideoRenderer"/> class.
        /// </summary>
        /// <param name="enableForm">True, to assign a form to the video.</param>
        public MdxVideoRenderer(bool enableForm)
        {
            this.enableForm = enableForm;
            if (this.enableForm)
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
            }
        }

        /// <summary>
        /// Event that is fired every time a data item is received.
        /// </summary>
        public event EventHandler<VideoEventArgs> VideoEventReceived;

        /// <summary>
        /// Gets a value indicating whether is playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Starts the video.
        /// </summary>
        public void Play()
        {
            if (this.IsPlaying)
            {
                // already playing the video
                // I avoid to play it twice.
                return;
            }

            this.video = new Video("video.mpg") { Fullscreen = true };
            this.video.Ending += this.OnVideoEnding;
            if (this.enableForm)
            {
                this.video.Owner = this;
                this.Show();
            }

            this.video.Play();
            this.IsPlaying = true;
        }

        /// <summary>
        /// Stops the video.
        /// </summary>
        public void Stop()
        {
            if (!this.IsPlaying)
            {
                // video already stopped.
                // I avoid to stop it twice.
                return;
            }

            this.IsPlaying = false;
            this.video.Stop();
            ThreadPool.QueueUserWorkItem(s => this.video.Dispose());
        }

        private void OnVideoEnding(object sender, EventArgs e)
        {
            // the video has (self) finished.
            // I notify this event.
            var handler = this.VideoEventReceived;
            if (handler != null)
            {
                handler(this, new VideoEventArgs(VideoEvent.VideoStopped));
            }
        }
    }
}
