// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectShowVideoTexture.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectShowVideoTexture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;

    using DirectShowLib;

    using Microsoft.DirectX.Direct3D;
    using Microsoft.Win32.SafeHandles;

    using NLog;

    /// <summary>
    /// Video texture using DirectShow.
    /// </summary>
    public class DirectShowVideoTexture : ISampleGrabberCB, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DsGuid mediaType;

        // Current state of the graph, which can be changed in multiple threads
        private volatile PlaybackState playbackState = PlaybackState.Stopped;

        private bool runEventThread;

        private IMediaControl mediaControl;
        private IMediaEventEx mediaEventEx;
        private IMediaSeeking mediaSeeking;

        private IntPtr graphEvent = IntPtr.Zero;
        private ManualResetEvent manualResetEvent;
        private Thread eventWatcherThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectShowVideoTexture"/> class.
        /// </summary>
        /// <param name="videoFile">
        /// An <code>.avi, .mpeg, .wmv</code> video file.
        /// </param>
        public DirectShowVideoTexture(string videoFile)
        {
            this.IsLooping = true;
            this.VideoFile = videoFile;

            // 1. Determine the type of video file.
            if (videoFile.EndsWith(".wmv", StringComparison.InvariantCultureIgnoreCase))
            {
                this.mediaType = MediaType.Video;
            }
            else
            {
                this.mediaType = null;
            }

            this.SetupGraph(this.VideoFile);
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

        private enum PlaybackState
        {
            Stopped,
            Paused,
            Running
        }

        /// <summary>
        /// Gets the video texture. This texture is continuously updated with current data from the video source.
        /// </summary>
        public Texture Texture { get; private set; }

        /// <summary>
        /// Gets the width of the source video stream (pixels).
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the source video stream (pixels).
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the size of the source video stream (pixels).
        /// </summary>
        public Size Size
        {
            get
            {
                return new Size(this.Width, this.Height);
            }
        }

        /// <summary>
        /// Gets the DirectShow Device that is providing the video.
        /// </summary>
        public DsDevice VideoDevice { get; private set; }

        /// <summary>
        /// Gets the video file to use as a source for the DirectShowVideoTexture.
        /// </summary>
        public string VideoFile { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is the video clip should
        /// loop continuously. (Default = True)
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Creates the underlying texture for the given device.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The GraphicsDevice where the video texture will be rendered.  Typically, this is the
        /// GraphicsDevice of the Game or Application that uses the DirectShowVideoTexture.
        /// </param>
        public void CreateTexture(Device graphicsDevice)
        {
            this.Texture = new Texture(
                graphicsDevice, this.Width, this.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
        }

        /// <summary>
        /// Start the Video Playback.
        /// </summary>
        public void Start()
        {
            if (this.playbackState != PlaybackState.Stopped)
            {
                return;
            }

            this.Run();

            if (this.mediaEventEx == null)
            {
                return;
            }

            int hr = this.mediaEventEx.GetEventHandle(out this.graphEvent);
            DsError.ThrowExceptionForHR(hr);
            this.manualResetEvent = new ManualResetEvent(false);
            this.manualResetEvent.SafeWaitHandle = new SafeWaitHandle(this.graphEvent, true);

            // Create a new thread to wait for events
            this.runEventThread = true;
            this.eventWatcherThread = new Thread(this.WatchForGraphEvents);
            this.eventWatcherThread.Name = "Video Event Watcher Thread";
            this.eventWatcherThread.Start();
        }

        /// <summary>
        /// Halt the Video Playback.
        /// </summary>
        public void Stop()
        {
            if (this.playbackState != PlaybackState.Running && this.playbackState != PlaybackState.Paused)
            {
                return;
            }

            if (this.mediaControl == null)
            {
                return;
            }

            int hr = this.mediaControl.Stop();
            DsError.ThrowExceptionForHR(hr);
            this.playbackState = PlaybackState.Stopped;
        }

        /// <summary>
        /// Pause the Video Playback.
        /// </summary>
        public void Pause()
        {
            if (this.playbackState != PlaybackState.Running)
            {
                return;
            }

            if (this.mediaControl == null)
            {
                return;
            }

            // Calling pause by itself does not seem to do anything.
            // DirectShowVideoTexture keeps streaming for some reason.
            // Calling pause, then stop when ready, results in an image visible and frozen on the screen.
            int hr = this.mediaControl.Pause();
            DsError.ThrowExceptionForHR(hr);
            hr = this.mediaControl.StopWhenReady();
            DsError.ThrowExceptionForHR(hr);
            this.playbackState = PlaybackState.Paused;
        }

        /// <summary>
        /// Shut down the DirectShowVideoTexture.
        /// </summary>
        public void Dispose()
        {
            this.runEventThread = false;

            if (this.mediaControl != null)
            {
                this.mediaControl.Stop();
                this.mediaControl = null;
            }

            if (this.manualResetEvent != null)
            {
                this.manualResetEvent.Set();
                this.manualResetEvent.Close();
            }

            if (this.VideoDevice != null)
            {
                this.VideoDevice.Dispose();
                this.VideoDevice = null;
            }

            if (this.Texture != null)
            {
                this.Texture.Dispose();
            }
        }

        /// <summary>
        /// Buffer Callback method from the  DirectShow.NET ISampleGrabberCB interface.  This method is called
        /// when a new frame is grabbed by the SampleGrabber.
        /// </summary>
        /// <param name="sampleTime">The sample time.</param>
        /// <param name="bufferPtr">A pointer to the image buffer that contains the grabbed sample.</param>
        /// <param name="bufferLength">The length of the image buffer containing the grabbed sample.</param>
        /// <returns>0 = success.</returns>
        unsafe int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr bufferPtr, int bufferLength)
        {
            if (this.Texture == null || this.Texture.Disposed || !this.Texture.Device.CheckCooperativeLevel())
            {
                return 0;
            }

            try
            {
                int texturePitch;
                using (var stream = this.Texture.LockRectangle(0, LockFlags.None, out texturePitch))
                {
                    try
                    {
                        var height = this.Height;
                        var width = this.Width;
                        var videoPitch = this.Width * 4;
                        byte* videoPtr = (byte*)bufferPtr.ToPointer();
                        byte* texturePtr = (byte*)stream.InternalDataPointer;

                        // start from bottom, otherwise our video is mirrored
                        int videoOffset = (height - 1) * videoPitch;
                        int textureOffset = 0;
                        for (int y = 0; y < height; y++)
                        {
                            CopyMemory(&texturePtr[textureOffset], &videoPtr[videoOffset], (uint)(width * 4));

                            videoOffset -= videoPitch;
                            textureOffset += texturePitch;
                        }
                    }
                    finally
                    {
                        this.Texture.UnlockRectangle(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not write video into texture");
            }

            return 0;
        }

        int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample sample)
        {
            // this is not supported
            return -1;
        }

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
        /// Raises the <see cref="VideoEnded"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseVideoEnded(EventArgs e)
        {
            var handler = this.VideoEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static unsafe extern void CopyMemory(void* destination, void* source, uint length);

        private void Run()
        {
            if (this.playbackState != PlaybackState.Stopped && this.playbackState != PlaybackState.Paused)
            {
                return;
            }

            if (this.mediaControl == null)
            {
                return;
            }

            int hr = this.mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            this.playbackState = PlaybackState.Running;
            this.RaiseVideoStarted(EventArgs.Empty);
        }

        private void SetupGraph(string videoFile)
        {
            // 1. Start building the graph, using FilterGraph and CaptureGraphBuilder2
            var graphBuilder = (IGraphBuilder)new FilterGraph();
            var builder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            int hr = builder.SetFiltergraph(graphBuilder);
            DsError.ThrowExceptionForHR(hr);

            // 2. Add the source filter for the video file input.
            IBaseFilter sourceFilter;
            hr = graphBuilder.AddSourceFilter(videoFile, videoFile, out sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // 3. Get the SampleGrabber interface, configure it, and add it to the graph.
            var sampGrabber = (ISampleGrabber)new SampleGrabber();
            this.ConfigureSampleGrabber(sampGrabber);
            hr = graphBuilder.AddFilter((IBaseFilter)sampGrabber, "SampleGrabber");
            DsError.ThrowExceptionForHR(hr);

            // 4. Add the null renderer (since we don't want to render in a seperate window.)
            var nullRenderer = (IBaseFilter)new NullRenderer();
            hr = graphBuilder.AddFilter(nullRenderer, "Null Renderer");
            DsError.ThrowExceptionForHR(hr);

            // 5. Render the stream.  The way the stream is rendered depends on its type.
            hr = builder.RenderStream(null, this.mediaType, sourceFilter, (IBaseFilter)sampGrabber, nullRenderer);
            DsError.ThrowExceptionForHR(hr);

            // 6. Now that everthing is configured and set up, save the width, height, stride information for use later.
            this.SaveSizeInfo(sampGrabber);

            // 7. Obtain the interfaces that we will use to control the execution of the filter graph.
            this.mediaControl = graphBuilder as IMediaControl;
            this.mediaEventEx = graphBuilder as IMediaEventEx;
            this.mediaSeeking = graphBuilder as IMediaSeeking;
        }

        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            // Set the media type to Video/RBG24
            var media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.ARGB32;
            media.formatType = FormatType.VideoInfo;

            int hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            // Get the media type from the SampleGrabber
            var media = new AMMediaType();
            int hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            var videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            this.Width = videoInfoHeader.BmiHeader.Width;
            this.Height = videoInfoHeader.BmiHeader.Height;

            DsUtils.FreeAMMediaType(media);
        }

        private void WatchForGraphEvents()
        {
            while (this.runEventThread)
            {
                this.manualResetEvent.WaitOne(-1, true);
                lock (this)
                {
                    this.HandleGraphEvents();
                }
            }
        }

        private void HandleGraphEvents()
        {
            if (this.mediaEventEx == null)
            {
                return;
            }

            IntPtr eventParam1, eventParam2;
            EventCode eventCode;

            // Print a console message for debugging purposes
            Logger.Trace("Calling MediaEventEx.GetEvent()");

            // Process all queued events
            while (this.mediaEventEx.GetEvent(out eventCode, out eventParam1, out eventParam2, 0) == 0)
            {
                int hr;
                if (eventCode == EventCode.Complete)
                {
                    if (this.IsLooping)
                    {
                        var pos = new DsLong(0);

                        // Reset to first frame of the clip
                        hr = this.mediaSeeking.SetPositions(
                            pos, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);

                        if (hr < 0)
                        {
                            // SetPositions has failed.  Just try stopping and starting again.
                            this.Stop();
                            this.Run();
                        }
                    }
                    else
                    {
                        this.Pause();
                        this.RaiseVideoEnded(EventArgs.Empty);
                    }
                }

                // Free event parameter memory.
                hr = this.mediaEventEx.FreeEventParams(eventCode, eventParam1, eventParam2);
                DsError.ThrowExceptionForHR(hr);
            }
        }
    }
}
