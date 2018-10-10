namespace ManagedDirectXTest
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using DirectShowLib;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// This class exposes a Texture property for use by XNA components and applications that 
    /// consume Texture2D objects.  This class uses the 3rd Party DirectShow.NET library.
    /// </summary>
    public class VideoTexture : ISampleGrabberCB, IDisposable
    {
        /// <summary>
        /// This event is raised whenever the current frame of the VideoTexture changes.  Use this event to push
        /// new data to subscribers, as an alternative to polling by calling the Texture property.
        /// </summary>
        public event EventHandler<NewVideoTextureEventArgs> NewVideoTextureReady;

        private void RaiseNewVideoTextureEvent()
        {
            var handler = this.NewVideoTextureReady;
            if (handler != null)
            {
                handler(this, new NewVideoTextureEventArgs(this.Texture));
            }
        }

        /// <summary>
        /// Gets the video texture. This texture is continously updated with current data from the video source.
        /// </summary>
        public Texture Texture { get; private set; }

        private VideoType vidType;

        /// <summary>
        /// The width of the source video stream (pixels).
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the source video stream (pixels).
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The index of the DirectX Video Device source.  Default is 0, for the first device found, or this value can
        /// be set in the constructor.
        /// </summary>
        public int Source { get; private set; }

        /// <summary>
        /// The graphics device where the texture will be rendered.  This is set in the constructor.
        /// </summary>
        public Device GraphicsDevice { get; private set; }

        /// <summary>
        /// The DirectShow Device that is providing the video.
        /// </summary>
        public DsDevice VideoDevice { get; private set; }

        /// <summary>
        /// The video file to use as a source for the VideoTexture.
        /// </summary>
        public string VideoFile { get; set; }

        /// <summary>
        /// Should the video clip loop continuously? (Default = True)
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Timeout for Graph Events, in milliseconds. (Default = 500)
        /// </summary>
        public uint Timeout { get; set; }

        /// <summary>
        /// The playback rate for video clips. (Default = 30)
        /// </summary>
        public int FramesPerSecond { get; set; }

        // Current state of the graph, which can be changed in multiple threads
        private volatile PlaybackState playbackState = PlaybackState.Stopped;

        /// <summary>
        /// Graphics Device based constructor. This constructor assumes that the first DirectShow video device that is found 
        /// will be used as the source of the video.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice where the video texture will be rendered.  Typically, this is the
        /// GraphicsDevice of the Game or Application that uses the VideoTexture.</param>
        public VideoTexture(Device graphicsDevice)
            : this(0, graphicsDevice)
        {
        }

        /// <summary>
        /// A constructor that specifies a particular DirectShow Video Device to use as the source of the video.
        /// </summary>
        /// <param name="videoSource">The DirectShow video device to use.  This integer represents the index that is
        /// assigned to the device as the available video devices are enumerated.</param>
        /// <param name="graphicsDevice">The GraphicsDevice where the video texture will be rendered.  Typically, this is the
        /// GraphicsDevice of the Game or Application that uses the VideoTexture.</param>
        public VideoTexture(int videoSource, Device graphicsDevice)
        {
            this.FramesPerSecond = 30;
            this.Timeout = 500;
            this.IsLooping = true;
            this.GraphicsDevice = graphicsDevice;
            this.Source = videoSource;
            this.vidType = VideoType.Live;

            // 1. Obtain the video device.
            this.VideoDevice = this.SelectWebcam(this.Source);

            // 2. Setup the Capture Graph.
            this.SetupGraph(this.VideoDevice);

            // 3. Setup the video texture.  This will use the width and height data of the actual video stream that is now attached.
            this.Texture = new Texture(this.GraphicsDevice, this.Width, this.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            
            // 4. Start the video streaming process.
            this.Start();
        }

        /// <summary>
        /// A constructor that specifies a particular video file to use as the source of the video.
        /// </summary>
        /// <param name="videoFile">An .avi, .mpeg, .wmv video file.</param>
        /// <param name="graphicsDevice">The GraphicsDevice where the video texture will be rendered.  Typically, this is the
        /// GraphicsDevice of the Game or Application that uses the VideoTexture.</param>
        public VideoTexture(string videoFile, Device graphicsDevice)
        {
            this.FramesPerSecond = 30;
            this.Timeout = 500;
            this.IsLooping = true;
            this.Source = 0;
            this.GraphicsDevice = graphicsDevice;
            this.VideoFile = videoFile;

            // 1. Determine the type of video file.
            if (videoFile.EndsWith(".avi", StringComparison.InvariantCultureIgnoreCase))
            {
                this.vidType = VideoType.AVI;
            }
            else if (videoFile.EndsWith(".mpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                this.vidType = VideoType.MPEG;
            }
            else if (videoFile.EndsWith(".mpg", StringComparison.InvariantCultureIgnoreCase))
            {
                this.vidType = VideoType.MPEG;
            }
            else if (videoFile.EndsWith(".wmv", StringComparison.InvariantCultureIgnoreCase))
            {
                this.vidType = VideoType.WMV;
            }
            else
            {
                throw new Exception("Unsupported Video Type: " + videoFile);
            }

            // 2. Setup the Capture Graph.
            this.SetupGraph(this.VideoFile); // Works for .avi, .mpeg,.wmv files. 

            // 3. Setup the video texture.  This will use the width and height data of the actual video stream that is now attached.
            this.Texture = new Texture(this.GraphicsDevice, this.Width, this.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            
            // 4. Start the video streaming process.
            this.Start();
        }

        /// <summary>
        /// Shut down the VideoTexture.
        /// </summary>
        public void Dispose()
        {
            if (this.mediaControl != null)
            {
                this.mediaControl.Stop();
                this.mediaControl = null;
            }

            if (this.manualResetEvent != null)
            {
                this.manualResetEvent.Close();
            }

            if (this.eventWatcherThread != null)
            {
                this.eventWatcherThread.Abort();
                this.eventWatcherThread = null;
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

        private IMediaControl mediaControl = null;
        private IMediaEventEx mediaEventEx = null;
        private IMediaSeeking mediaSeeking = null;

        private DsDevice SelectWebcam(int deviceToUse)
        {
            DsDevice dsDevice = null;
            try
            {
                Console.WriteLine("Trying to select video input device " + deviceToUse.ToString());

                dsDevice = null;

                DsDevice[] videoInputDevices = DirectShowLib.DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                // See if there are any Video Input Devices attached and active, and if so, put them into the array...
                if (videoInputDevices.Length == 0)
                {
                    throw new Exception("No video input devices found!");
                }

                if (videoInputDevices.Length <= (deviceToUse + 1))	// last available deviceToUse
                {
                    dsDevice = videoInputDevices[deviceToUse] as DsDevice;	// cast using as					
                }

                // and perform a final check to make sure the device is selected.
                if (dsDevice == null)
                {
                    throw new Exception("No video input devices selected!");
                }

                Console.WriteLine("Success!");
                return dsDevice;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure...");
                Console.WriteLine("Video Input Selection Exception: " + ex.Message);
                throw new Exception("Video Input Selection Exception", ex);
            }
        }

        private void SetupGraph(DsDevice dev)
        {
            try
            {
                int hr;

                // 1. Start building the graph, using FilterGraph and CaptureGraphBuilder2
                IFilterGraph2 graphBuilder = (IFilterGraph2)new FilterGraph();
                ICaptureGraphBuilder2 builder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = builder.SetFiltergraph(graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                // 2. Add the source filter for video device input.
                IBaseFilter sourceFilter = null;
                hr = graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, "Video Input Filter", out sourceFilter);
                DsError.ThrowExceptionForHR(hr);

                // 3. Get the SampleGrabber interface, configure it, and add it to the graph.
                ISampleGrabber sampGrabber = (ISampleGrabber)new SampleGrabber();
                ConfigureSampleGrabber(sampGrabber);
                hr = graphBuilder.AddFilter((IBaseFilter)sampGrabber, "SampleGrabber");
                DsError.ThrowExceptionForHR(hr);

                // 4. Add the null renderer (since we don't want to render in a seperate window.)
                IBaseFilter nullRenderer = (IBaseFilter)new NullRenderer();
                hr = graphBuilder.AddFilter(nullRenderer, "Null Renderer");
                DsError.ThrowExceptionForHR(hr);

                // 5. Configure the render stream.
                hr = builder.RenderStream(PinCategory.Capture, MediaType.Video, sourceFilter, (IBaseFilter)sampGrabber, nullRenderer);
                DsError.ThrowExceptionForHR(hr);

                // 6. Now that everthing is configured and set up, save the width, height, stride information for use later.
                SaveSizeInfo(sampGrabber);

                // 7. Obtain the interfaces that we will use to control the execution of the filter graph.
                this.mediaControl = graphBuilder as IMediaControl;
                this.mediaEventEx = graphBuilder as IMediaEventEx;
                this.mediaSeeking = graphBuilder as IMediaSeeking;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// This method sets up the DirectShow filter graph and obtains the interfaces necessary to control playback
        /// for VideoTextures created from video files.  This method works for .avi, .mpeg, and .wmv files.  
        /// </summary>
        /// <param name="videoFile">The .avi, .mpeg, or .wmv video file.</param>
        private void SetupGraph(string videoFile)
        {
            try
            {
                int hr;

                // 1. Start building the graph, using FilterGraph and CaptureGraphBuilder2
                IGraphBuilder graphBuilder = (IGraphBuilder)new FilterGraph();
                ICaptureGraphBuilder2 builder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = builder.SetFiltergraph(graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                // 2. Add the source filter for the video file input.
                IBaseFilter sourceFilter = null;
                hr = graphBuilder.AddSourceFilter(videoFile, videoFile, out sourceFilter);
                DsError.ThrowExceptionForHR(hr);

                // 3. Get the SampleGrabber interface, configure it, and add it to the graph.
                ISampleGrabber sampGrabber = (ISampleGrabber)new SampleGrabber();
                ConfigureSampleGrabber(sampGrabber);
                hr = graphBuilder.AddFilter((IBaseFilter)sampGrabber, "SampleGrabber");
                DsError.ThrowExceptionForHR(hr);

                // 4. Add the null renderer (since we don't want to render in a seperate window.)
                IBaseFilter nullRenderer = (IBaseFilter)new NullRenderer();
                hr = graphBuilder.AddFilter(nullRenderer, "Null Renderer");
                DsError.ThrowExceptionForHR(hr);

                // 5. Render the stream.  The way the stream is rendered depends on its type.
                switch (this.vidType)
                {
                    case VideoType.AVI:
                        hr = builder.RenderStream(null, null, sourceFilter, (IBaseFilter)sampGrabber, nullRenderer);
                        break;
                    case VideoType.MPEG:
                        hr = builder.RenderStream(null, null, sourceFilter, (IBaseFilter)sampGrabber, nullRenderer);
                        break;
                    case VideoType.WMV:
                        hr = builder.RenderStream(null, MediaType.Video, sourceFilter, (IBaseFilter)sampGrabber, nullRenderer);
                        break;
                    default:
                        throw new Exception("Unsupported Video type: " + this.vidType.ToString());
                }
                DsError.ThrowExceptionForHR(hr);

                // 6. Now that everthing is configured and set up, save the width, height, stride information for use later.
                SaveSizeInfo(sampGrabber);

                // 7. Obtain the interfaces that we will use to control the execution of the filter graph.
                this.mediaControl = graphBuilder as IMediaControl;
                this.mediaEventEx = graphBuilder as IMediaEventEx;
                this.mediaSeeking = graphBuilder as IMediaSeeking;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }

        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            this.Width = videoInfoHeader.BmiHeader.Width;
            this.Height = videoInfoHeader.BmiHeader.Height;

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// Start the Video Playback.
        /// </summary>
        public void Start()
        {
            if (this.playbackState == PlaybackState.Stopped)
            {
                Run();
                // Use one or the other of the following techniques (ManualEventWatcher is prefered.)
                ManualEventWatcher(); // Watches for graph events on a seperate thread, and calls HandleGraphEvents() when triggered.
                //TimerBasedEventPolling(); // Calls the HandleGraphEvents() on a timer.
            }
        }

        /// <summary>
        /// Run the Video Playback.
        /// </summary>
        public void Run()
        {
            if (this.playbackState != PlaybackState.Stopped && this.playbackState != PlaybackState.Paused)
            {
                return;
            }

            if (this.mediaControl == null)
            {
                return;
            }


            this.videoPitch = ((this.Width * 3) + 3) & ~3;
            int hr = this.mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            this.playbackState = PlaybackState.Running;
        }

        /// <summary>
        /// Pause the Video Playback.
        /// </summary>
        public void Pause()
        {
            if (this.playbackState == PlaybackState.Running)
            {
                if (this.mediaControl != null)
                {
                    // Calling pause by itself does not seem to do anything. (VideoTexture keeps streaming for some reason).
                    // Calling pause, then stop when ready, results in an image visible and frozen on the screen. (Which is what we want...)
                    int hr = this.mediaControl.Pause();
                    DsError.ThrowExceptionForHR(hr);
                    hr = this.mediaControl.StopWhenReady();
                    DsError.ThrowExceptionForHR(hr);
                    this.playbackState = PlaybackState.Paused;
                }
            }
        }

        /// <summary>
        /// Halt the Video Playback.  
        /// </summary>
        public void Stop()
        {
            if (this.playbackState == PlaybackState.Running || this.playbackState == PlaybackState.Paused)
            {
                if (this.mediaControl != null)
                {
                    int hr = this.mediaControl.Stop();
                    DsError.ThrowExceptionForHR(hr);
                    this.playbackState = PlaybackState.Stopped;
                }
            }
        }

        private IntPtr graphEvent = IntPtr.Zero;
        private ManualResetEvent manualResetEvent;
        private Thread eventWatcherThread;

        private void ManualEventWatcher()
        {
            if (this.mediaEventEx != null)
            {
                int hr = this.mediaEventEx.GetEventHandle(out this.graphEvent);
                DsError.ThrowExceptionForHR(hr);
                this.manualResetEvent = new ManualResetEvent(false);
                this.manualResetEvent.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(this.graphEvent, true);

                // Create a new thread to wait for events
                this.eventWatcherThread = new Thread(new ThreadStart(this.WatchForGraphEvents));
                this.eventWatcherThread.Name = "Event Watcher Thread";
                this.eventWatcherThread.Start();
            }
        }

        private void WatchForGraphEvents()
        {
            while (true)
            {
                this.manualResetEvent.WaitOne(-1, true);
                lock (this)
                {
                    HandleGraphEvents();
                }

            }
        }

        private System.Timers.Timer eventTimer;

        private int videoPitch;

        private void TimerBasedEventPolling()
        {
            this.eventTimer = new System.Timers.Timer(100);
            this.eventTimer.Elapsed += new System.Timers.ElapsedEventHandler(eventTimer_Elapsed);
            this.eventTimer.Enabled = true;
        }

        private void eventTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HandleGraphEvents();
        }

        private void HandleGraphEvents()
        {
            if (this.mediaEventEx != null)
            {
                int hr = 0;
                IntPtr eventParam1, eventParam2;
                EventCode eventCode;

                // Print a console message for debugging purposes
                Console.WriteLine("Calling MediaEventEx.GetEvent()");

                // Process all queued events
                while (this.mediaEventEx.GetEvent(out eventCode, out eventParam1, out eventParam2, 0) == 0)
                {
                    switch (eventCode)
                    {
                        case EventCode.Complete:
                            if (this.IsLooping)
                            {
                                DsLong pos = new DsLong(0);
                                // Reset to first frame of the clip
                                hr = this.mediaSeeking.SetPositions(pos, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
                                if (hr < 0) // SetPositions has failed.  Just try stopping and starting again.
                                {
                                    Stop();
                                    Run();
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    // Print a console message for debugging purposes.
                    Console.WriteLine(eventCode.ToString());

                    // Free event parameter memory.
                    hr = this.mediaEventEx.FreeEventParams(eventCode, eventParam1, eventParam2);
                    DsError.ThrowExceptionForHR(hr);
                }
            }
        }

        /// <summary>
        /// Buffer Callback method from the  DirectShow.NET ISampleGrabberCB interface.  This method is called
        /// when a new frame is grabbed by the SampleGrabber.
        /// </summary>
        /// <param name="SampleTime">The sample time.</param>
        /// <param name="pBuffer">A pointer to the image buffer that contains the grabbed sample.</param>
        /// <param name="BufferLen">The length of the image buffer containing the grabbed sample.</param>
        /// <returns>0 = success.</returns>
        public unsafe int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (this.Texture.Disposed)
            {
                return 0;
            }

            lock (this.Texture)
            {
                int texturePitch;
                var stream = this.Texture.LockRectangle(0, LockFlags.None, out texturePitch);
                try
                {
                    var height = this.Height;
                    var width = this.Width;
                    byte* videoPtr = (byte*)pBuffer.ToPointer();
                    byte* texturePtr = (byte*)stream.InternalDataPointer;
                    int videoOffset = 0;
                    int textureOffset = 0;
                    for (int y = 0; y < height; y++)
                    {
                        int videoRowStart = videoOffset;
                        int textureRowStart = textureOffset;

                        for (int x = 0; x < width; x++)
                        {
                            texturePtr[textureOffset++] = videoPtr[videoOffset++];
                            texturePtr[textureOffset++] = videoPtr[videoOffset++];
                            texturePtr[textureOffset++] = videoPtr[videoOffset++];
                            texturePtr[textureOffset++] = 0xFF;
                        }

                        videoOffset = videoRowStart + this.videoPitch;
                        textureOffset = textureRowStart + texturePitch;
                    }
                }
                finally
                {
                    this.Texture.UnlockRectangle(0);
                }
            }

            this.RaiseNewVideoTextureEvent();    // notify subscribers, if any.

            return 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static unsafe extern void CopyMemory(void* destination, void* source, uint length);

        /// <summary>
        /// Sample CallBack method from the ISampleGrabberCB interface (DirectShow.NET).  Not used.
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="pSample"></param>
        /// <returns></returns>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private enum PlaybackState
        {
            Stopped,
            Paused,
            Running,
            Exiting
        }

        private enum VideoType
        {
            AVI,
            MPEG,
            WMV,
            Live,
            Unknown
        }
    }
}
