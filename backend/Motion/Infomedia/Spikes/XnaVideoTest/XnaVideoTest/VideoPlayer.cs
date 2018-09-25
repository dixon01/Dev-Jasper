// -----------------------------------------------------------------------
// <copyright file="VideoPlayer.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace XnaVideoTest
{
    using System;
    using System.Runtime.InteropServices;

    using DirectShowLib;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VideoPlayer : ISampleGrabberCB, IDisposable
    {
        private Guid MEDIATYPE_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        private FilterGraph fg;
        private IGraphBuilder gb;
        private IMediaControl mc;
        private IMediaEventEx me;
        private IMediaSeeking ms;
        public bool IsVideoPlaying;
        private IVideoWindow vm;
        private readonly long videoDuration;

        public VideoPlayer()
        {
            this.InitializeInterfaces();

            var sg = new SampleGrabber();
            var sampleGrabber = (ISampleGrabber)sg;
            gb.AddFilter((IBaseFilter)sg, "Grabber");

            var mt = new AMMediaType();
            mt.majorType = MEDIATYPE_Video;
            sampleGrabber.SetMediaType(mt);
            gb.RenderFile(@"Wildlife.wmv", null);
            ms.GetDuration(out videoDuration);
        }
        public event EventHandler OnVideoComplete;

        private void InitializeInterfaces()
        {
            fg = new FilterGraph();
            gb = (IGraphBuilder)fg;
            mc = (IMediaControl)fg;
            me = (IMediaEventEx)fg;
            vm = (IVideoWindow)gb;
            ms = (IMediaSeeking)fg;
        }

        public void Play()
        {
            vm.put_FullScreenMode(OABool.True);
            mc.Run();
            IsVideoPlaying = true;
            this.WaitTillVideoComplete();
        }

        private void WaitTillVideoComplete()
        {
            EventCode pEvCode;

            do
            {
                me.WaitForCompletion((int)videoDuration, out pEvCode);
            }
            while (pEvCode != EventCode.Complete);
           
            OnVideoComplete.Invoke(this, EventArgs.Empty);

        }

        public void Stop()
        {
            vm.put_FullScreenMode(OABool.False);
            mc.Stop();
            IsVideoPlaying = false;
            ms.SetPositions(0, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning);
        }

        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            return 0;
        }

        public void Dispose()
        {
            Stop();
            CloseInterfaces();
        }

        private void CloseInterfaces()
        {
            if (me != null)
            {
                mc.Stop();
                me.SetNotifyWindow(IntPtr.Zero, 0x00008001, IntPtr.Zero);
            }
            mc = null;
            me = null;
            gb = null;
            if (fg != null)
                Marshal.ReleaseComObject(fg);
            fg = null;
        }
    }
}
