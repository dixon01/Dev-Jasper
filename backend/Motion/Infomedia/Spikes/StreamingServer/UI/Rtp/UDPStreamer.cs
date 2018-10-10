namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Net.Sockets;
    using System.Net;

    public class UdpStreamer
    {/*
        private RtpFile file;

        private Thread streamThread;
        private Socket socket;

        private IPacketFilter filter;
        private IEnumerator<StreamPacket> stream;

        private int timeStampFrequency = 0;
        private int startTimeStamp = -1;

        private int ssrc;

        public UdpStreamer(RtpFile file) : this(file, null)
        {
        }

        public UdpStreamer(RtpFile file, IPacketFilter filter)
        {
            this.file = file;
            this.filter = filter;

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        }

        public event EventHandler StreamFinished;

        public int LocalPort
        {
            get
            {
                return (this.socket.LocalEndPoint as IPEndPoint).Port;
            }
        }

        public int TimeStampFrequency
        {
            get
            {
                return this.timeStampFrequency;
            }
            set
            {
                this.timeStampFrequency = value;
            }
        }

        public int Ssrc
        {
            get
            {
                return this.ssrc;
            }
        }

        public void StartStream(EndPoint endPoint, NormalPlayTimeRange range)
        {
            if (this.streamThread == null)
            {
                this.stream = this.GetFilteredStream(range);

                this.streamThread = new Thread(new ParameterizedThreadStart(this.StreamThread));
                this.streamThread.Start(endPoint);
            }
        }

        public void PauseStream()
        {
            if (this.streamThread != null)
            {
                Monitor.Enter(this);
            }
        }

        public void ResumeStream(NormalPlayTimeRange range)
        {
            if (this.streamThread != null)
            {
                if (range.From != null)
                {
                    this.stream = this.GetFilteredStream(range);
                }
                try
                {
                    Monitor.Exit(this);
                }
                catch (Exception)
                {
                }
            }
        }

        public void StopStream()
        {
            if (this.streamThread != null)
            {
                //streamThread.Abort();
                this.streamThread = null;
                try
                {
                    // just to be sure, release the lock
                    Monitor.Exit(this);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }

        public void SendPacket(RtcpPacket rtcpPacket)
        {
            byte[] data = rtcpPacket.GetData();
            this.socket.Send(data, 0, data.Length, SocketFlags.None);
        }

        private void StreamThread(object userData)
        {
            try
            {
                EndPoint endPoint = userData as EndPoint;

                this.socket.Connect(endPoint);

                long startTime = DateTime.Now.Ticks; // in 100 ns

                // we do not use a foreach here because we might
                // change the stream variable from another thread
                // e.g. by resuming at a specific position
                while (this.stream.MoveNext())
                {
                    StreamPacket packet = this.stream.Current;
                    this.ssrc = packet.Ssrc;

                    if (this.streamThread == null)
                    {
                        return;
                    }
                    if (this.timeStampFrequency > 0)
                    {
                        int timeStamp = packet.TimeStamp;
                        int tsDelta = timeStamp - this.startTimeStamp;
                        long timeDelta = this.TimeStampTo100Nano(tsDelta);
                        long now = DateTime.Now.Ticks;

                        long waitTime = timeDelta - (now - startTime) - 2000000L;
                        if (waitTime > 0)
                        {
                            Console.WriteLine("{2}: Sleep {0} ms (delta={1})", waitTime / 10000L, timeDelta / 10000L, this.filter);
                            Thread.Sleep(new TimeSpan(waitTime));
                        }
                    }
                    if (Monitor.TryEnter(this))
                    {
                        this.socket.Send(packet.Data, 1, packet.Data.Length - 1, SocketFlags.None);
                        Console.WriteLine("{0}: Send", this.filter);
                        //Console.WriteLine(Hex.ToDisplayString(packet.Data, 1, packet.Data.Length - 1, 4));
                    }
                    else // in case of a pause
                    {
                        Monitor.Enter(this);
                    }
                    Monitor.Exit(this);
                }

                if (this.StreamFinished != null)
                {
                    Console.WriteLine("{0}: Stream Finished", this.filter);
                    this.StreamFinished(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private IEnumerator<StreamPacket> GetFilteredStream(NormalPlayTimeRange range)
        {
            long startTime = range.From != null ? range.From.Get100NanoSeconds() : 0;
            long endTime = range.To != null ? range.To.Get100NanoSeconds() : Int64.MaxValue;
            this.startTimeStamp = -1;
            foreach (MasterBlock block in this.file.MasterBlocks)
            {
                foreach (StreamPacket packet in block)
                {
                    if (this.filter == null || this.filter.Accept(packet))
                    {
                        if (this.startTimeStamp == -1)
                        {
                            this.startTimeStamp = packet.TimeStamp;
                        }
                        if (this.timeStampFrequency > 0)
                        {
                            long timeStamp100Nano = this.TimeStampTo100Nano(packet.TimeStamp - this.startTimeStamp);
                            if (timeStamp100Nano >= startTime)
                            {
                                if (timeStamp100Nano > endTime)
                                {
                                    // stop if we are behind the endTime
                                    yield break;
                                }
                                else
                                {
                                    yield return packet;
                                }
                            }
                        }
                        else
                        {
                            yield return packet;
                        }
                    }
                }
            }
            yield break;
        }

        private long TimeStampTo100Nano(int timeStamp)
        {
            return timeStamp * 10000000L / this.timeStampFrequency;
        }*/
    }
}
