namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    internal class TestRtpJpegStreamer : IUdpStreamer
    {
        private readonly Socket socket;

        private readonly ManualResetEvent waitEvent = new ManualResetEvent(true);

        private bool running;

        public TestRtpJpegStreamer()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        }

        public int LocalPort
        {
            get
            {
                return ((IPEndPoint)this.socket.LocalEndPoint).Port;
            }
        }

        public void PauseStream()
        {
            this.waitEvent.Reset();
        }

        public void ResumeStream(NormalPlayTimeRange range)
        {
            this.waitEvent.Set();
        }

        public void StartStream(IPEndPoint rtpEndPoint, NormalPlayTimeRange range)
        {
            this.waitEvent.Set();
            this.running = true;

            var thread = new Thread(this.SendThread) { IsBackground = true };
            thread.Start(rtpEndPoint);
        }

        public void StopStream()
        {
            this.running = false;
            this.waitEvent.Set();
        }

        private void SendThread(object state)
        {
            var rtpEndPoint = (IPEndPoint)state;
            var source = new JpegSource();
            while (this.running)
            {
                foreach (var packet in source.GetPackets())
                {
                    this.socket.SendTo(packet.Data, packet.Offset, packet.Size, SocketFlags.None, rtpEndPoint);
                }

                this.waitEvent.WaitOne();
            }
        }
    }
}