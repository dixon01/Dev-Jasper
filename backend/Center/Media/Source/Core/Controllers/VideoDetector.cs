// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoDetector.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using DirectShowLib;
    using DirectShowLib.DES;

    using Gorba.Common.Utility.Win32.Api.DLLs;

    using NLog;

    using PixelFormat = System.Drawing.Imaging.PixelFormat;
    using Size = System.Windows.Size;

    /// <summary>
    /// This class allows to get video file meta data and create a snapshot of a specific frame.
    /// </summary>
    /// <remarks>
    /// This class doesn't support MP4 file format.
    /// </remarks>
    public class VideoDetector : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IMediaDet mediaDet;
        private int streamCount;
        private double videoStreamLength;
        private bool hasVideo;
        private Bitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDetector"/> class.
        /// </summary>
        /// <param name="fullFileName">
        /// The full path of the file.
        /// </param>
        public VideoDetector(string fullFileName)
        {
            this.FullFileName = fullFileName;
            this.LoadMedia();
        }

        /// <summary>
        /// Gets the file name including its path.
        /// </summary>
        public string FullFileName { get; private set; }

        /// <summary>
        /// Gets the video resolution.
        /// </summary>
        public Size VideoResolution { get; private set; }

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Disposes the object, freeing also unmanaged objects.
        /// </summary>
        public void Dispose()
        {
            this.FreeResources();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the <see cref="BitmapSource"/> of a frame in the middle of a video.
        /// </summary>
        /// <returns>
        /// The <see cref="BitmapSource"/> of the frame that can be saved.
        /// </returns>
        public unsafe BitmapSource GetMiddleFrame()
        {
            if (!this.hasVideo)
            {
                return null;
            }

            var secondsPos = this.Duration.TotalSeconds / 2;
            var bufferPointer = IntPtr.Zero;
            BitmapSource bitmapSource = null;
            try
            {
                int bufferSize;
                var hr = this.mediaDet.GetBitmapBits(
                    secondsPos,
                    out bufferSize,
                    IntPtr.Zero,
                    (int)this.VideoResolution.Width,
                    (int)this.VideoResolution.Height);
                if (hr == 0)
                {
                    bufferPointer = Marshal.AllocCoTaskMem(bufferSize);
                    hr = this.mediaDet.GetBitmapBits(
                        secondsPos,
                        out bufferSize,
                        bufferPointer,
                        (int)this.VideoResolution.Width,
                        (int)this.VideoResolution.Height);
                    DsError.ThrowExceptionForHR(hr);
                    var bitmapHeader = (BitmapInfoHeader)Marshal.PtrToStructure(
                        bufferPointer, typeof(BitmapInfoHeader));
                    var bitmapDataPointer = (byte*)bufferPointer.ToPointer();

                    bitmapDataPointer += bitmapHeader.Size;
                    var bitmapData = new IntPtr(bitmapDataPointer);
                    this.bitmap = new Bitmap(bitmapHeader.Width, bitmapHeader.Height, PixelFormat.Format24bppRgb);

                    var bitmapdata = this.bitmap.LockBits(
                        new Rectangle(0, 0, bitmapHeader.Width, bitmapHeader.Height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format24bppRgb);

                    Kernel32.MoveMemory(
                        bitmapdata.Scan0,
                        bitmapData,
                        (int)this.VideoResolution.Width * (int)this.VideoResolution.Height * 3);
                    this.bitmap.UnlockBits(bitmapdata);

                    this.bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                    bitmapdata = this.bitmap.LockBits(
                        new Rectangle(0, 0, bitmapHeader.Width, bitmapHeader.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format24bppRgb);
                    bitmapSource = BitmapSource.Create(
                        (int)this.VideoResolution.Width,
                        (int)this.VideoResolution.Height,
                        96,
                        96,
                        PixelFormats.Bgr24,
                        null,
                        bitmapdata.Scan0,
                        bitmapdata.Stride * (int)this.VideoResolution.Height,
                        bitmapdata.Stride);
                    this.bitmap.UnlockBits(bitmapdata);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting a frame from a video.", exception);
            }
            finally
            {
                if (bufferPointer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(bufferPointer);
                }
            }

            return bitmapSource;
        }

        private void LoadMedia()
        {
            this.mediaDet = new MediaDet() as IMediaDet;

            if (this.mediaDet == null)
            {
                throw new NullReferenceException("Could not create an instance of MediaDet COM");
            }

            var hr = this.mediaDet.put_Filename(this.FullFileName);
            DsError.ThrowExceptionForHR(hr);

            hr = this.mediaDet.get_OutputStreams(out this.streamCount);
            DsError.ThrowExceptionForHR(hr);

            for (int i = 0; i < this.streamCount; i++)
            {
                hr = this.mediaDet.put_CurrentStream(i);
                DsError.ThrowExceptionForHR(hr);

                Guid majorType;

                hr = this.mediaDet.get_StreamType(out majorType);
                DsError.ThrowExceptionForHR(hr);

                var mediaType = new AMMediaType();

                hr = this.mediaDet.get_StreamMediaType(mediaType);
                DsError.ThrowExceptionForHR(hr);

                if (majorType == MediaType.Video)
                {
                    this.ReadVideoProperties(mediaType);
                }

                DsUtils.FreeAMMediaType(mediaType);
            }
        }

        private void ReadVideoProperties(AMMediaType mediaType)
        {
            var hr = this.mediaDet.get_StreamLength(out this.videoStreamLength);
            double frameRate;
            this.mediaDet.get_FrameRate(out frameRate);
            var frameCount = (int)(frameRate * this.videoStreamLength);
            this.Duration = TimeSpan.FromSeconds(frameCount / frameRate);
            DsError.ThrowExceptionForHR(hr);

            if (mediaType.formatType == FormatType.VideoInfo)
            {
                this.hasVideo = true;

                var header = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
                this.VideoResolution = new Size(header.BmiHeader.Width, header.BmiHeader.Height);
            }
            else if (mediaType.formatType == FormatType.VideoInfo2)
            {
                this.hasVideo = true;

                var header = (VideoInfoHeader2)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader2));
                this.VideoResolution = new Size(header.BmiHeader.Width, header.BmiHeader.Height);
            }
        }

        private void FreeResources()
        {
            this.VideoResolution = Size.Empty;

            if (this.bitmap != null)
            {
                this.bitmap.Dispose();
            }

            this.bitmap = null;

            if (this.mediaDet == null)
            {
                return;
            }

            Marshal.ReleaseComObject(this.mediaDet);
            this.mediaDet = null;
        }
    }
}
