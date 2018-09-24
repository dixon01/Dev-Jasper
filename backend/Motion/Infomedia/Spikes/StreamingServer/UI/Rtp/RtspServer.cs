// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtspServer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The rtsp server.
    /// </summary>
    public class RtspServer
    {
        #region Constants and Fields

        private static readonly Random random = new Random();

        private static readonly Dictionary<string, List<RtpStreamer>> streamers =
            new Dictionary<string, List<RtpStreamer>>();

        private Socket serverSocket;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspServer"/> class.
        /// </summary>
        public RtspServer()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public void Start(int port)
        {
            this.Stop();

            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Blocking = true;
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

            var serverThread = new Thread(this.ServerThread);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (this.serverSocket != null)
            {
                foreach (var streamList in streamers.Values)
                {
                    foreach (RtpStreamer stream in streamList)
                    {
                        stream.Stop();
                    }
                }

                this.serverSocket.Close();
                this.serverSocket = null;
            }
        }

        #endregion

        #region Methods

        private void ConnectionHandler(object userObject)
        {
            using (var socket = userObject as Socket)
            {
                var endPoint = socket.RemoteEndPoint as IPEndPoint;
                var data = new byte[1024];

                while (true)
                {
                    try
                    {
                        RtspRequest req = this.ReceiveRequest(socket);
                        if (req == null)
                        {
                            break;
                        }

                        Console.WriteLine(
                            "== RCV == {2} == {0}:{1} ==", 
                            endPoint.Address, 
                            endPoint.Port, 
                            DateTime.Now.ToLongTimeString());
                        Console.WriteLine(req.ToString().Trim());
                        Console.WriteLine("=======================================");
                        RtspResponse resp = null;

                        switch (req.Command.ToUpper())
                        {
                            case "OPTIONS":
                                resp = this.HandleOptions(req);
                                break;
                            case "DESCRIBE":
                                resp = this.HandleDescribe(req);
                                break;
                            case "SETUP":
                                resp = this.HandleSetup(req);
                                break;
                            case "PLAY":
                                resp = this.HandlePlay(req);
                                break;
                            case "PAUSE":
                                resp = this.HandlePause(req);
                                break;
                            case "TEARDOWN":
                                resp = this.HandleTeardown(req);
                                break;
                            default:

                                // hmmm, we don't know this command
                                resp = new RtspResponse(501, "Not Implemented", req);
                                break;
                        }

                        if (resp != null)
                        {
                            Console.WriteLine(
                                "== SND == {2} == {0}:{1} ==", 
                                endPoint.Address, 
                                endPoint.Port, 
                                DateTime.Now.ToLongTimeString());
                            Console.WriteLine(resp.ToString().Trim());
                            Console.WriteLine("=======================================");
                            this.SendResponse(resp, socket);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        if (ex is SocketException)
                        {
                            socket.Close();
                            break;
                        }
                    }
                }
            }
        }

        private RtspResponse HandleDescribe(RtspRequest req)
        {
            if (req["Accept"].IndexOf("application/sdp", StringComparison.InvariantCulture) < 0)
            {
                throw new NotSupportedException("Not Supported");
            }

            var sb = new StringBuilder();
            sb.AppendLine("v=0");
            sb.AppendLine("m=video 0 RTP/AVP 26");

            // todo: add more
            /*
                sb.Append(this.file.SdpPacket.Session.ToString());
                foreach (SdpMediaInfo info in this.file.SdpPacket)
                {
                    // do not send data stream, QuickTime is confused by this!
                    if (info.Name != "data")
                    {
                        sb.Append("\r\n");
                        sb.Append(info.ToString());
                    }
                }*/

            var resp = new RtspResponse(200, "OK", req);
            resp["Cache-Control"] = "must-revalidate";
            resp["Content-Type"] = "application/sdp";
            resp.Body = sb.ToString();
            return resp;
        }

        private RtspResponse HandleOptions(RtspRequest req)
        {
            var resp = new RtspResponse(200, "OK", req);
            ////resp["Public"] = "DESCRIBE, SETUP, TEARDOWN, PLAY, PAUSE";
            resp["Public"] = "DESCRIBE, SETUP, TEARDOWN, PLAY"; // we don't support pause/resume for now
            return resp;
        }

        private RtspResponse HandlePause(RtspRequest req)
        {
            var resp = new RtspResponse(200, "OK", req);
            foreach (RtpStreamer streamer in streamers[req["Session"]])
            {
                streamer.Pause();
            }

            return resp;
        }

        private RtspResponse HandlePlay(RtspRequest req)
        {
            var resp = new RtspResponse(200, "OK", req);
            NormalPlayTimeRange nptRange = NormalPlayTimeRange.ALL;
            string range = req["Range"];
            if (range != null)
            {
                if (range.StartsWith("npt="))
                {
                    try
                    {
                        nptRange = new NormalPlayTimeRange(range.Substring(4));
                    }
                    catch (Exception ex)
                    {
                        // ignore if we can't parse the NPT range
                        Console.WriteLine(ex);
                    }
                }

                resp["Range"] = range;
            }

            foreach (RtpStreamer streamer in streamers[req["Session"]])
            {
                if (streamer.Paused)
                {
                    streamer.Resume(nptRange);
                }
                else
                {
                    streamer.Start(nptRange);
                }
            }

            /*RTP-Info: url=rtsp://foo.com/bar.avi/streamid=0;seq=45102,
               url=rtsp://foo.com/bar.avi/streamid=1;seq=30211*/
            return resp;
        }

        private RtspResponse HandleSetup(RtspRequest req)
        {
            string path = req.Uri.LocalPath;
            string streamName = path.Substring(path.LastIndexOf('/') + 1);
            string sessionId = req["Session"];
            if (sessionId == null)
            {
                do
                {
                    sessionId = random.Next(10000000, 99999999).ToString();
                }
                while (streamers.ContainsKey(sessionId));
                streamers.Add(sessionId, new List<RtpStreamer>(2));
            }

            string[] ports = req["Transport"].Split('=', '-');
            var endPoint1 = new IPEndPoint(req.EndPoint.Address, int.Parse(ports[ports.Length - 2]));
            var endPoint2 = new IPEndPoint(req.EndPoint.Address, int.Parse(ports[ports.Length - 1]));
            var streamer = new RtpStreamer(sessionId, streamName, endPoint1, endPoint2);
            streamers[sessionId].Add(streamer);

            var resp = new RtspResponse(200, "OK", req);
            resp["Session"] = streamer.SessionId;
            resp["Transport"] = string.Format(
                "{0};server_port={1}-{2}", req["Transport"], streamer.RtpPort, streamer.RtcpPort);

            return resp;
        }

        private RtspResponse HandleTeardown(RtspRequest req)
        {
            var resp = new RtspResponse(200, "OK", req);
            string sessionId = req["Session"];
            foreach (RtpStreamer streamer in streamers[sessionId])
            {
                streamer.Stop();
            }

            return resp;
        }

        private RtspRequest ReceiveRequest(Socket socket)
        {
            var data = new byte[1024];
            var memory = new MemoryStream(1024);
            string dataStr;

            int len;
            do
            {
                len = socket.Receive(data);
                memory.Write(data, 0, len);
                dataStr = Encoding.ASCII.GetString(memory.GetBuffer(), 0, (int)memory.Length);
            }
            while (len > 0 && dataStr.IndexOf("\r\n\r\n") < 0);
            return dataStr.Length > 0 ? new RtspRequest(dataStr, socket.RemoteEndPoint as IPEndPoint) : null;
        }

        private void SendResponse(RtspResponse resp, Socket socket)
        {
            socket.Send(Encoding.ASCII.GetBytes(resp.ToString()));
        }

        private void ServerThread()
        {
            try
            {
                this.serverSocket.Listen(2);
                while (true)
                {
                    Socket socket = this.serverSocket.Accept();
                    var handler = new Thread(this.ConnectionHandler);
                    handler.IsBackground = true;
                    handler.Start(socket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}