// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RTPStreamer_.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System.Net;

    internal interface IUdpStreamer
    {
        int LocalPort { get; }

        void PauseStream();

        void ResumeStream(NormalPlayTimeRange range);

        void StartStream(IPEndPoint rtpEndPoint, NormalPlayTimeRange range);

        void StopStream();
    }

    /// <summary>
    /// The rtp streamer.
    /// </summary>
    internal class RtpStreamer
    {
        #region Constants and Fields

        private readonly IPEndPoint rtcpEndPoint;

        private readonly IPEndPoint rtpEndPoint;

        private readonly string sessionId;

        private bool paused;

        private bool started;

        private IUdpStreamer rtcpStream;
        private IUdpStreamer rtpStream;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtpStreamer"/> class.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="streamName">
        /// The stream name.
        /// </param>
        /// <param name="rtpEndPoint">
        /// The rtp end point.
        /// </param>
        /// <param name="rtcpEndPoint">
        /// The rtcp end point.
        /// </param>
        internal RtpStreamer(
            string sessionId, string streamName, IPEndPoint rtpEndPoint, IPEndPoint rtcpEndPoint)
        {
            this.sessionId = sessionId;
            this.rtpEndPoint = rtpEndPoint;
            this.rtcpEndPoint = rtcpEndPoint;

            this.rtpStream = new TestRtpJpegStreamer();
            this.rtcpStream = new EmptyRtcpStreamer();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether Paused.
        /// </summary>
        public bool Paused
        {
            get
            {
                return this.paused;
            }
        }

        /// <summary>
        /// Gets RtcpPort.
        /// </summary>
        public int RtcpPort
        {
            get
            {
                return this.rtcpStream.LocalPort;
            }
        }

        /// <summary>
        /// Gets RtpPort.
        /// </summary>
        public int RtpPort
        {
            get
            {
                return this.rtpStream.LocalPort;
            }
        }

        /// <summary>
        /// Gets SessionId.
        /// </summary>
        public string SessionId
        {
            get
            {
                return this.sessionId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Started.
        /// </summary>
        public bool Started
        {
            get
            {
                return this.started;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            if (this.started && !this.paused)
            {
                this.paused = true;
                this.rtpStream.PauseStream();
                this.rtcpStream.PauseStream();
            }
        }

        /// <summary>
        /// The resume.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        public void Resume(NormalPlayTimeRange range)
        {
            if (this.started && this.paused)
            {
                this.paused = false;
                this.rtpStream.ResumeStream(range);
                this.rtcpStream.ResumeStream(range);
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="range">
        /// The range.
        /// </param>
        public void Start(NormalPlayTimeRange range)
        {
            if (!this.started)
            {
                this.started = true;
                this.rtpStream.StartStream(this.rtpEndPoint, range);
                this.rtcpStream.StartStream(this.rtcpEndPoint, range);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (this.started)
            {
                this.started = false;
                this.paused = false;
                this.rtpStream.StopStream();
                this.rtcpStream.StopStream();
            }
        }

        #endregion

        #region Methods

        /*private void rtpStream_StreamFinished(object sender, EventArgs e)
        {
            if (this.rtpStream.Ssrc != 0)
            {
                // todo: bye packet does not yet work with QuickTime...
                var bye = new GoodbyeRtcpPacket(new[] { this.rtpStream.Ssrc });
                Console.WriteLine("{0}: BYE: {1}", this.mediaInfo.Name, Hex.ToString(bye.GetData()));
                this.rtcpStream.SendPacket(bye);
            }
        }*/

        #endregion
    }
}