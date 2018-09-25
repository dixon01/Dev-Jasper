namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EmptyRtcpStreamer : IUdpStreamer
    {
        private readonly Socket socket;

        public EmptyRtcpStreamer()
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
        }

        public void ResumeStream(NormalPlayTimeRange range)
        {
        }

        public void StartStream(IPEndPoint rtpEndPoint, NormalPlayTimeRange range)
        {
        }

        public void StopStream()
        {
        }
    }
}
