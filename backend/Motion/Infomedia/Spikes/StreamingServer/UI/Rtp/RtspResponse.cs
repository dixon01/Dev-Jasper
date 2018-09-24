// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtspResponse.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// The rtsp response.
    /// </summary>
    public class RtspResponse
    {
        #region Constants and Fields

        private readonly int code;

        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        private readonly string protocol;

        private readonly string reason;

        private string body = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspResponse"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        internal RtspResponse(int code, string reason, RtspRequest request)
        {
            this.protocol = "RTSP/1.0";
            this.code = code;
            this.reason = reason;

            this["Server"] = "Gorba Streaming Server Test";
            if (request["CSeq"] != null)
            {
                this["CSeq"] = request["CSeq"];
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Body.
        /// </summary>
        public string Body
        {
            get
            {
                return this.body;
            }

            set
            {
                this.body = value;
            }
        }

        /// <summary>
        /// Gets Code.
        /// </summary>
        public int Code
        {
            get
            {
                return this.code;
            }
        }

        /// <summary>
        /// Gets Protocol.
        /// </summary>
        public string Protocol
        {
            get
            {
                return this.protocol;
            }
        }

        /// <summary>
        /// Gets Reason.
        /// </summary>
        public string Reason
        {
            get
            {
                return this.reason;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        public string this[string header]
        {
            get
            {
                string value;
                this.headers.TryGetValue(header, out value);
                return value;
            }

            set
            {
                this.headers[header] = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Protocol);
            sb.Append(' ');
            sb.Append(this.Code);
            sb.Append(' ');
            sb.Append(this.Reason);
            sb.Append("\r\n");

            if (this.body.Length > 0)
            {
                this.headers["Content-Length"] = this.body.Length.ToString(CultureInfo.InvariantCulture);
            }

            foreach (var header in this.headers)
            {
                sb.Append(header.Key);
                sb.Append(": ");
                sb.Append(header.Value);
                sb.Append("\r\n");
            }

            sb.Append("\r\n");
            sb.Append(this.body);
            return sb.ToString();
        }

        #endregion
    }
}