// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MdxVideoRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Engine
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using DxRenderVideoTest.Event;

    using Microsoft.DirectX.AudioVideoPlayback;

    /// <summary>
    /// A simple wrapper around the object "Video" of Managed DirectX.
    /// </summary>
    public class MdxVideoRenderer : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MdxVideoRenderer"/> class.
        /// </summary>
        public MdxVideoRenderer()
        {
        }

        /// <summary>
        /// Event that is fired every time a data item is received.
        /// </summary>
        public event EventHandler<VideoEventArgs> VideoEventReceived;

        /// <summary>
        /// Event that is fired every time a data item is received.
        /// </summary>
        public event EventHandler<VideoEventArgs> SecondVideoEventReceived;

        /// <summary>
        /// Gets the video.
        /// </summary>
        public Video Video { get; private set; }

        /// <summary>
        /// Gets the second video.
        /// </summary>
        public Video Video2 { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is playing the first video.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is playing the second video.
        /// </summary>
        public bool IsPlayingSecond { get; private set; }

        /// <summary>
        /// Starts the video without associating it to a control owner.
        /// </summary>
        public void Play()
        {
            this.Play(null);
        }

        /// <summary>
        /// Starts the video associating it to a specific control owner.
        /// </summary>
        /// <param name="videoOwner">
        /// The video owner.
        /// </param>
        public void Play(Control videoOwner)
        {
            if (this.IsPlaying)
            {
                // already playing the video
                // I avoid to play it twice.
                return;
            }

            this.Video = new Video("video.mpg");
            this.Video.Ending += this.OnVideoEnding;
            this.Video.Owner = videoOwner;
            this.Video.Fullscreen = videoOwner == null;
            this.Video.Play();
            this.IsPlaying = true;
        }

        /// <summary>
        /// The play second video.
        /// </summary>
        /// <param name="videoOwner">
        /// The video owner.
        /// </param>
        public void PlaySecondVideo(Control videoOwner)
        {
            if (this.IsPlayingSecond)
            {
                // already playing the video
                // I avoid to play it twice.
                return;
            }

            this.Video2 = new Video("video.mpg");
            this.Video2.Ending += this.OnSecondVideoEnding;
            this.Video2.Owner = videoOwner;
            this.Video2.Fullscreen = videoOwner == null;
            this.Video2.Play();
            this.IsPlayingSecond = true;
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
            this.Video.Stop();
            ThreadPool.QueueUserWorkItem(s => this.Video.Dispose());
        }

        /// <summary>
        /// Stops the second video
        /// </summary>
        public void StopSecondVideo()
        {
            if (!this.IsPlayingSecond)
            {
                // video already stopped.
                // I avoid to stop it twice.
                return;
            }

            this.IsPlayingSecond = false;
            this.Video2.Stop();
            ThreadPool.QueueUserWorkItem(s => this.Video2.Dispose());
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

        private void OnSecondVideoEnding(object sender, EventArgs e)
        {
            // the second video has (self) finished.
            // I notify this event.
            var handler = this.SecondVideoEventReceived;
            if (handler != null)
            {
                handler(this, new VideoEventArgs(VideoEvent.VideoStopped));
            }
        }

        /*
        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "MdxVideoRenderer";
            this.ResumeLayout(false);
        }
        */
    }
}
