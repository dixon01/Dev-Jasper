// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VlcVideoSprite.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VlcVideoSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Core;

    using NLog;

    using Vlc.DotNet.Core;
    using Vlc.DotNet.Core.Interops.Signatures.LibVlc.MediaListPlayer;
    using Vlc.DotNet.Core.Medias;
    using Vlc.DotNet.Forms;

    /// <summary>
    /// Video sprite that uses VLC media player to play a video in a separate window.
    /// </summary>
    public class VlcVideoSprite : WindowedVideoSpriteBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<VlcVideoSprite>();

        // VLC correctly initialized when true
        private static bool initialized;

        private VlcControl videoControl;
        
        private Form ownerForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="VlcVideoSprite"/> class.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public VlcVideoSprite(IDxDeviceRenderContext renderContext)
            : base(renderContext)
        {
        }

        /// <summary>
        /// Gets the size of the video.
        /// </summary>
        public override Size Size
        {
            get
            {
                return this.videoControl == null ? Size.Empty : this.videoControl.VideoProperties.Size;
            }
        }

        /// <summary>
        /// Initializes VLC.
        /// This should only be done exactly once in an application before using this sprite class.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                if (initialized)
                {
                    return;
                }

                if (Directory.Exists(CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64))
                {
                    // Set libvlc.dll and libvlccore.dll directory path
                    VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64;

                    // Set the vlc plugins directory path
                    VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_AMD64;
                }
                else
                {
                    // Set libvlc.dll and libvlccore.dll directory path
                    VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_X86;

                    // Set the vlc plugins directory path
                    VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_X86;
                }

                // Set the startup options
#if __UseLuminatorTftDisplay
                VlcContext.StartupOptions.IgnoreConfig = false;
                Logger.Error("VLC don't ignore configuration.");
#else
                VlcContext.StartupOptions.IgnoreConfig = true;
#endif

                VlcContext.StartupOptions.LogOptions.LogInFile = false;
                VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
                VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.None;

                // Initialize the VlcContext
                VlcContext.Initialize();
                initialized = true;
            }
            catch (Exception ex)
            {
                Logger.Error("VLC Initialization Failed {0}", ex.Message);
            }
        }

        /// <summary>
        /// Destroys everything related to VLC.
        /// This should only be done exactly once in an application just before exiting.
        /// </summary>
        public static void Deinitialize()
        {
            // Close the VlcContext
            VlcContext.CloseAll();
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
            this.ownerForm = owner;
            this.videoControl = new VlcControl { Dock = DockStyle.Fill };
            this.videoControl.EncounteredError += this.VideoControlOnEncounteredError;
            this.videoControl.EndReached += this.VideoControlOnEndReached;

            if (filename.Contains("://"))
            {
                this.videoControl.Media = new LocationMedia(filename);
            }
            else
            {
                this.videoControl.Media = new PathMedia(filename);
            }

            if (this.IsLooping)
            {
                this.videoControl.PlaybackMode = PlaybackModes.Loop;
            }

            owner.Controls.Add(this.videoControl);
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
            if (this.videoControl != null && this.videoControl.IsPlaying)
            {
                // the video is playing. I pause a little bit
                // this thread in order to give "breath" to the one
                // that is playing.

                // Hack for thread priority and management for Gorba 29" CPU
#if __UseLuminatorTftDisplay
                Thread.Sleep(75);
#else
                Thread.Sleep(10);
#endif
            }

            base.Render(bounds, alpha, context);
        }

        /// <summary>
        /// Starts playing the video.
        /// </summary>
        protected override void PlayVideo()
        {
            this.videoControl.Play();
        }

        /// <summary>
        /// Releases the video.
        /// </summary>
        protected override void ReleaseVideo()
        {
#if __UseLuminatorTftDisplay
            HideForm();
#endif

            if (this.videoControl != null)
            {
                this.videoControl.EndReached -= this.VideoControlOnEndReached;
                this.videoControl.Stop();
                this.videoControl.Dispose();
                this.videoControl = null;
            }

            base.ReleaseVideo();
        }

        private void VideoControlOnEncounteredError(VlcControl sender, VlcEventArgs<EventArgs> vlcEventArgs)
        {
            Logger.Error("VideoControlOnEncounteredError()");
            if (this.ownerForm != null)
            {
                if (this.ownerForm.IsHandleCreated)
                {
                    if (this.ownerForm.InvokeRequired)
                    {
                        this.ownerForm.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        this.ownerForm.Hide();
                                    }));
                    }
                    else
                    {
                        this.ownerForm.Hide();
                    }
                }
            }
            
            this.UseFallbackImage(true);
        }

        private void VideoControlOnEndReached(VlcControl sender, VlcEventArgs<EventArgs> vlcEventArgs)
        {
            if (!this.IsLooping)
            {
                this.RaiseVideoEnded(EventArgs.Empty);
            }
        }
    }
}