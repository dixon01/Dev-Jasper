// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A class for retrieving data from a NTP/SNTP server.
//   See http://www.faqs.org/rfcs/rfc2030.html for full details of protocol.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// A class for retrieving data from a NTP/SNTP server.
    /// See <see cref="http://www.faqs.org/rfcs/rfc2030.html"/> for full details of protocol.
    /// </summary>
    public partial class SntpClient
    {
        /// <summary>
        /// The default NTP/SNTP version number.
        /// </summary>
        public const VersionNumber DefaultVersionNumber = VersionNumber.Version3;

        /// <summary>
        /// The default number of milliseconds used for send and receive.
        /// </summary>
        public const int DefaultTimeout = 5000;

        /// <summary>
        /// The server that is used by default.
        /// </summary>
        public static readonly RemoteSntpServer DefaultServer = Sntp.RemoteSntpServer.Default;

        private int timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpClient"/> class.
        /// </summary>
        public SntpClient()
        {
            this.Initialize();
            this.Timeout = DefaultTimeout;
            this.VersionNumber = DefaultVersionNumber;
        }

        /// <summary>
        /// Raised when a query to the server completes successfully.
        /// </summary>
        public event EventHandler<QueryServerCompletedEventArgs> QueryServerCompleted;

        /// <summary>
        /// Gets the real local date and time using the default server and a total timeout of 1 second.
        /// If there is an error or exception, DateTime.MinValue is returned.
        /// (NB: This property getter is blocking)
        /// </summary>
        public static DateTime Now
        {
            get { return GetNow(); }
        }

        /// <summary>
        /// Gets a value indicating whether the SNTP client is busy.
        /// </summary>
        public bool IsBusy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the server to use.
        /// </summary>
        public RemoteSntpServer RemoteSntpServer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds used for sending and receiving.
        /// </summary>
        [DefaultValue(DefaultTimeout)]
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                if (value < -1)
                {
                    value = DefaultTimeout;
                }

                this.timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the NTP/SNTP version to use.
        /// </summary>
        [DefaultValue(DefaultVersionNumber)]
        public VersionNumber VersionNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates the current local time zone offset from UTC.
        /// </summary>
        /// <returns>A TimeSpan that is the current local time zone offset from UTC.</returns>
        public static TimeSpan GetCurrentLocalTimeZoneOffset()
        {
            return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        }

        /// <summary>
        /// Gets the real local date and time using the default server and a total timeout of 1 second.
        /// If there is an error or exception, DateTime.MinValue is returned.
        /// </summary>
        /// <returns>The real local date and time.</returns>
        public static DateTime GetNow()
        {
            return GetNow(RemoteSntpServer.Default, 500);
        }

        /// <summary>
        /// Gets the real local date and time using the specified server and a total timeout of 1 second.
        /// If there is an error or exception, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="remoteSntpServer">The server to use.</param>
        /// <returns>The real local date and time.</returns>
        public static DateTime GetNow(RemoteSntpServer remoteSntpServer)
        {
            return GetNow(remoteSntpServer, 500);
        }

        /// <summary>
        /// Gets the real local date and time using the default server and the specified timeout.
        /// If there is an error or exception, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds used for sending and receiving.</param>
        /// <returns>The real local date and time.</returns>
        public static DateTime GetNow(int timeout)
        {
            return GetNow(RemoteSntpServer.Default, timeout);
        }

        /// <summary>
        /// Gets the real local date and time using the default server and the specified timeout.
        /// If there is an error or exception, DateTime.MinValue is returned.
        /// </summary>
        /// <param name="remoteSntpServer">The server to use.</param>
        /// <param name="timeout">The timeout in milliseconds used for sending and receiving.</param>
        /// <returns>The real local date and time.</returns>
        public static DateTime GetNow(RemoteSntpServer remoteSntpServer, int timeout)
        {
            var sntpClient = new SntpClient();
            sntpClient.RemoteSntpServer = remoteSntpServer;
            sntpClient.Timeout = timeout;
            var args = sntpClient.QueryServer();
            return args.Succeeded ? DateTime.Now.AddSeconds(args.Data.LocalClockOffset) : DateTime.MinValue;
        }

        /// <summary>
        /// Queries the specified server on a separate thread.
        /// </summary>
        /// <returns>true if the SNTP client wasn't busy, otherwise false.</returns>
        public bool QueryServerAsync()
        {
            bool result = false;
            if (!this.IsBusy)
            {
                this.IsBusy = true;
                ThreadPool.QueueUserWorkItem(s =>
                    {
                        var arg = this.QueryServer();
                        this.IsBusy = false;
                        this.OnQueryServerCompleted(arg);
                    });
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Raises the QueryServerCompleted event.
        /// </summary>
        /// <param name="e">A QueryServerCompletedEventArgs instance.</param>
        protected virtual void OnQueryServerCompleted(QueryServerCompletedEventArgs e)
        {
            var eh = this.QueryServerCompleted;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        private void Initialize()
        {
            if (this.RemoteSntpServer == null)
            {
                this.RemoteSntpServer = DefaultServer;
            }
        }

        /// <summary>
        /// This is the 'nuts and bolts' method that queries the server.
        /// </summary>
        /// <returns>A QueryServerResults instance that holds the results of the query.</returns>
        private QueryServerCompletedEventArgs QueryServer()
        {
            var result = new QueryServerCompletedEventArgs();
            this.Initialize();
            UdpClient client = null;
            try
            {
                // Configure and connect the socket.
                var endPoint = this.RemoteSntpServer.GetIPEndPoint();
                client = this.CreateUdpClient();
                client.Connect(endPoint);

                // Send and receive the data, and save the completion DateTime.
                SntpData request = SntpData.GetClientRequestPacket(this.VersionNumber);
                client.Send(request, request.Length);
                result.Data = client.Receive(ref endPoint);
                result.Data.DestinationDateTime = DateTime.Now.ToUniversalTime();

                // Check the data
                if (result.Data.Mode == Mode.Server)
                {
                    result.Succeeded = true;
                }
                else
                {
                    result.ErrorData = new ErrorData("The response from the server was invalid.");
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorData = new ErrorData(ex);
                return result;
            }
            finally
            {
                // Close the socket
                if (client != null)
                {
                    client.Close();
                }
            }
        }
    }
}
