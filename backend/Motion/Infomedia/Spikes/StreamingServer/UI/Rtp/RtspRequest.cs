// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtspRequest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// The rtsp request.
    /// </summary>
    public class RtspRequest
    {
        #region Constants and Fields

        private readonly string command;

        private readonly IPEndPoint endPoint;

        private readonly Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

        private readonly string protocol;

        private readonly string url;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspRequest"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="endPoint">
        /// The end point.
        /// </param>
        internal RtspRequest(string request, IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            string[] lines = request.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 1)
            {
                throw new IOException("Bad Request");
            }

            string prevHeader = null;
            foreach (string line in lines)
            {
                if (this.url == null)
                {
                    string[] parts = line.Split(new[] { ' ' }, 3);
                    if (parts.Length != 3)
                    {
                        throw new IOException("Bad Request");
                    }

                    this.command = parts[0];
                    this.url = parts[1];
                    this.protocol = parts[2];
                }
                else
                {
                    int pos = line.IndexOf(": ", StringComparison.CurrentCultureIgnoreCase);
                    if (pos >= 0)
                    {
                        string headerName = line.Substring(0, pos);

                        List<string> headerValues;
                        if (!this.headers.TryGetValue(headerName, out headerValues))
                        {
                            headerValues = new List<string>(1);
                            this.headers.Add(headerName, headerValues);
                        }

                        headerValues.Add(line.Substring(pos + 2));
                        prevHeader = headerName;
                    }
                    else if (line.StartsWith("  ") && prevHeader != null)
                    {
                        List<string> headerValues = this.headers[prevHeader];
                        headerValues[headerValues.Count - 1] = headerValues[headerValues.Count - 1] + line.Substring(2);
                    }
                    else
                    {
                        throw new IOException("Bad Request");
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Command.
        /// </summary>
        public string Command
        {
            get
            {
                return this.command;
            }
        }

        /// <summary>
        /// Gets EndPoint.
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                return this.endPoint;
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
        /// Gets Uri.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return new Uri(this.url);
            }
        }

        /// <summary>
        /// Gets Url.
        /// </summary>
        public string Url
        {
            get
            {
                return this.url;
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
                List<string> values;
                if (this.headers.TryGetValue(header, out values) && values.Count > 0)
                {
                    return values[0];
                }
                
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Command);
            sb.Append(' ');
            sb.Append(this.Url);
            sb.Append(' ');
            sb.Append(this.Protocol);
            sb.Append("\r\n");

            foreach (var header in this.headers)
            {
                foreach (string value in header.Value)
                {
                    sb.Append(header.Key);
                    sb.Append(": ");
                    sb.Append(value);
                    sb.Append("\r\n");
                }
            }

            sb.Append("\r\n");
            return sb.ToString();
        }

        #endregion
    }
}