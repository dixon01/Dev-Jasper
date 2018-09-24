// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="MulticastDnsServer.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ARSoft.Tools.Net.Dns
{
    #if WindowsCE
    using OpenNETCF.Net.NetworkInformation;
#else
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    using ARSoft.Tools.Net.Socket;

    using NLog;

#endif

    /// <summary>
    ///     mDNS (RFC 6762) server implementation.
    ///     The source code is largely based on <see cref="DnsServer" />, but support
    ///     for TCP was stripped out and mDNS features like not responding to a received record
    ///     are implemented here.
    /// </summary>
    public class MulticastDnsServer : IDisposable
    {
        #region Static Fields

        public static readonly int MdnsPort = 5353;

        public static readonly IPAddress MulticastAddress = IPAddress.Parse("224.0.0.251");

        private static readonly Logger Logger = LogManager.GetLogger("ARSoft.Tools.Net.Dns.MulticastDnsServer");

        #endregion

        #region Fields

        /// <summary>
        ///     Method that will be called to get the keydata for processing a tsig signed message
        /// </summary>
        public DnsServer.SelectTsigKey TsigKeySelector;

        private readonly IPEndPoint bindEndPoint;

        private readonly ProcessMessage processMessageDelegate;

        private readonly int udpListenerCount;

        private int availableUdpListener;

        private bool hasActiveUdpListener;

        private bool running;

        private UdpListener udpListener;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Creates a new dns server instance which will listen on all available interfaces
        /// </summary>
        /// <param name="udpListenerCount"> The count of threads listings on udp, 0 to deactivate udp </param>
        /// <param name="processMessage"> Method, which process the message and returns the response </param>
        public MulticastDnsServer(int udpListenerCount, ProcessMessage processMessage)
            : this(IPAddress.Any, udpListenerCount, processMessage)
        {
        }

        /// <summary>
        ///     Creates a new dns server instance
        /// </summary>
        /// <param name="bindAddress"> The address, on which should be listend </param>
        /// <param name="udpListenerCount"> The count of threads listings on udp, 0 to deactivate udp </param>
        /// <param name="processMessage"> Method, which process the message and returns the response </param>
        public MulticastDnsServer(IPAddress bindAddress, int udpListenerCount, ProcessMessage processMessage)
            : this(new IPEndPoint(bindAddress, MdnsPort), udpListenerCount, processMessage)
        {
        }

        /// <summary>
        ///     Creates a new dns server instance
        /// </summary>
        /// <param name="bindEndPoint"> The endpoint, on which should be listend </param>
        /// <param name="udpListenerCount"> The count of threads listings on udp, 0 to deactivate udp </param>
        /// <param name="processMessage"> Method, which process the message and returns the response </param>
        public MulticastDnsServer(IPEndPoint bindEndPoint, int udpListenerCount, ProcessMessage processMessage)
        {
            this.bindEndPoint = bindEndPoint;
            this.processMessageDelegate = processMessage;

            this.udpListenerCount = udpListenerCount;

            Timeout = 120000;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Represents the method, that will be called to get the response for a specific dns query
        /// </summary>
        /// <param name="query"> The query, for that a response should be returned </param>
        /// <param name="remoteEndPoint">
        ///     The remote end point where the message comes from (in) and where to send the response to (out)
        /// </param>
        /// <returns> A DnsMessage with the response to the query or null if nothing should be sent back. </returns>
        public delegate DnsMessageBase ProcessMessage(DnsMessageBase query, ref IPEndPoint remoteEndPoint);

        #endregion

        #region Public Events

        /// <summary>
        ///     This event is fired on exceptions of the listeners. You can use it for custom logging.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionThrown;

        /// <summary>
        ///     This event is fired whenever a message is received, that is not correct signed
        /// </summary>
        public event EventHandler<InvalidSignedMessageEventArgs> InvalidSignedMessageReceived;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the timeout for sending and receiving data
        /// </summary>
        public int Timeout { get; set; }

        #endregion

        #region Public Methods and Operators

        public IPAddress[] GetLocalIPAddresses()
        {
            if (!this.bindEndPoint.Address.Equals(IPAddress.Any))
            {
                return new[] { this.bindEndPoint.Address };
            }

            return
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(
                        n =>
                        n.SupportsMulticast() && (n.OperationalStatus == OperationalStatus.Up)
                        && (n.NetworkInterfaceType != NetworkInterfaceType.Loopback) && (n.NetworkInterfaceType != NetworkInterfaceType.Tunnel))
                    .SelectMany(n => n.GetIPProperties().UnicastAddresses.Select(a => a.Address))
                    .Where(a => !IPAddress.IsLoopback(a) && (a.AddressFamily == AddressFamily.InterNetwork))
                    .ToArray();
        }

        public void SendMessage(DnsMessageBase message, IPEndPoint endpoint)
        {
            this.DoSendMessage(message, null, false, endpoint, false);
        }

        /// <summary>
        ///     Starts the server
        /// </summary>
        public void Start()
        {
            this.Start(MulticastAddress);
        }

        /// <summary>
        ///     Starts the server
        /// </summary>
        public void Start(IPAddress multicastAddress)
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            if (this.udpListenerCount > 0)
            {
                this.availableUdpListener = this.udpListenerCount;
                this.udpListener = new UdpListener(this.bindEndPoint, multicastAddress);
                StartUdpListen();
            }
        }

        /// <summary>
        ///     Stops the server
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            if (this.udpListenerCount > 0)
            {
                this.udpListener.Dispose();
            }
        }

        #endregion

        #region Explicit Interface Methods

        void IDisposable.Dispose()
        {
            Stop();
        }

        #endregion

        #region Methods

        private DnsMessageBase DoProcessMessage(DnsMessageBase query, ref IPEndPoint remoteEndPoint)
        {
            if (query.TSigOptions != null)
            {
                switch (query.TSigOptions.ValidationResult)
                {
                    case ReturnCode.BadKey:
                    case ReturnCode.BadSig:
                        query.IsQuery = false;
                        query.ReturnCode = ReturnCode.NotAuthoritive;
                        query.TSigOptions.Error = query.TSigOptions.ValidationResult;
                        query.TSigOptions.KeyData = null;

                        if (InvalidSignedMessageReceived != null)
                        {
                            InvalidSignedMessageReceived(this, new InvalidSignedMessageEventArgs(query));
                        }

                        return query;

                    case ReturnCode.BadTime:
                        query.IsQuery = false;
                        query.ReturnCode = ReturnCode.NotAuthoritive;
                        query.TSigOptions.Error = query.TSigOptions.ValidationResult;
                        query.TSigOptions.OtherData = new byte[6];
                        int tmp = 0;
                        TSigRecord.EncodeDateTime(query.TSigOptions.OtherData, ref tmp, DateTime.Now);

                        if (InvalidSignedMessageReceived != null)
                        {
                            InvalidSignedMessageReceived(this, new InvalidSignedMessageEventArgs(query));
                        }

                        return query;
                }
            }

            return this.processMessageDelegate(query, ref remoteEndPoint);
        }

        private void DoSendMessage(DnsMessageBase response, byte[] originalMac, bool isEDnsEnabled, IPEndPoint endpoint, bool restartReceive)
        {
            byte[] buffer;
            int length = response.Encode(false, originalMac, out buffer);

            #region Truncating

            DnsMessage message = response as DnsMessage;

            if (message != null)
            {
                int maxLength = 512;
                if (isEDnsEnabled && message.IsEDnsEnabled)
                {
                    maxLength = Math.Max(512, (int)message.EDnsOptions.UpdPayloadSize);
                }

                while (length > maxLength)
                {
                    // First step: remove data from additional records except the opt record
                    if ((message.IsEDnsEnabled && (message.AdditionalRecords.Count > 1))
                        || (!message.IsEDnsEnabled && (message.AdditionalRecords.Count > 0)))
                    {
                        for (int i = message.AdditionalRecords.Count - 1; i >= 0; i--)
                        {
                            if (message.AdditionalRecords[i].RecordType != RecordType.Opt)
                            {
                                message.AdditionalRecords.RemoveAt(i);
                            }
                        }

                        length = message.Encode(false, originalMac, out buffer);
                        continue;
                    }

                    int savedLength = 0;
                    if (message.AuthorityRecords.Count > 0)
                    {
                        for (int i = message.AuthorityRecords.Count - 1; i >= 0; i--)
                        {
                            savedLength += message.AuthorityRecords[i].MaximumLength;
                            message.AuthorityRecords.RemoveAt(i);

                            if ((length - savedLength) < maxLength)
                            {
                                break;
                            }
                        }

                        message.IsTruncated = true;

                        length = message.Encode(false, originalMac, out buffer);
                        continue;
                    }

                    if (message.AnswerRecords.Count > 0)
                    {
                        for (int i = message.AnswerRecords.Count - 1; i >= 0; i--)
                        {
                            savedLength += message.AnswerRecords[i].MaximumLength;
                            message.AnswerRecords.RemoveAt(i);

                            if ((length - savedLength) < maxLength)
                            {
                                break;
                            }
                        }

                        message.IsTruncated = true;

                        length = message.Encode(false, originalMac, out buffer);
                        continue;
                    }

                    if (message.Questions.Count > 0)
                    {
                        for (int i = message.Questions.Count - 1; i >= 0; i--)
                        {
                            savedLength += message.Questions[i].MaximumLength;
                            message.Questions.RemoveAt(i);

                            if ((length - savedLength) < maxLength)
                            {
                                break;
                            }
                        }

                        message.IsTruncated = true;

                        length = message.Encode(false, originalMac, out buffer);
                    }
                }
            }

            #endregion

            this.udpListener.BeginSend(buffer, 0, length, endpoint, this.EndUdpSend, restartReceive);
        }

        private void EndUdpReceive(IAsyncResult ar)
        {
            try
            {
                lock (this.udpListener)
                {
                    this.hasActiveUdpListener = false;
                }
                StartUdpListen();

                IPEndPoint endpoint;

                byte[] buffer = this.udpListener.EndReceive(ar, out endpoint);

                if (endpoint == null || (this.GetLocalIPAddresses().Any(a => a.Equals(endpoint.Address)) && this.bindEndPoint.Port == endpoint.Port))
                {
                    // this is our own message, let's ignore it
                    this.RestartUdpListen();
                    return;
                }

                DnsMessageBase query;
                byte[] originalMac;
                try
                {
                    query = DnsMessageBase.Create(buffer, true, TsigKeySelector, null);
                    originalMac = (query.TSigOptions == null) ? null : query.TSigOptions.Mac;
                }
                catch (Exception)
                {
                    throw new Exception("Error parsing dns query");
                }

                DnsMessageBase response;
                try
                {
                    response = this.DoProcessMessage(query, ref endpoint);
                }
                catch (Exception ex)
                {
                    OnExceptionThrown(ex);
                    response = null;
                }

                if (response == null || endpoint == null)
                {
                    // mDNS messages don't always mean there is a response
                    // (since we are also getting responses from others)
                    this.RestartUdpListen();
                    return;
                }

                this.DoSendMessage(response, originalMac, query.IsEDnsEnabled, endpoint, true);
            }
            catch (Exception ex)
            {
                HandleUdpException(ex);
            }
        }

        private void EndUdpSend(IAsyncResult ar)
        {
            try
            {
                this.udpListener.EndSend(ar);
            }
            catch (Exception ex)
            {
                HandleUdpException(ex);
            }

            if (!(ar.AsyncState is bool) || (bool)ar.AsyncState)
            {
                this.RestartUdpListen();
            }
        }

        private void HandleUdpException(Exception e)
        {
            this.RestartUdpListen();

            OnExceptionThrown(e);
        }

        private void OnExceptionThrown(Exception ex)
        {
            if (ex is ObjectDisposedException)
            {
                return;
            }

            if (ExceptionThrown != null)
            {
                this.ExceptionThrown(this, new ExceptionEventArgs(ex));
            }
            else
            {
                Logger.Warn(ex, "Error in multi-cast DNS server");
            }
        }

        private void RestartUdpListen()
        {
            if (!this.running)
            {
                return;
            }

            lock (this.udpListener)
            {
                this.availableUdpListener++;
            }

            this.StartUdpListen();
        }

        private void StartUdpListen()
        {
            try
            {
                lock (this.udpListener)
                {
                    if ((this.availableUdpListener > 0) && !this.hasActiveUdpListener)
                    {
                        this.availableUdpListener--;
                        this.hasActiveUdpListener = true;
                        this.udpListener.BeginReceive(EndUdpReceive, null);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (this.udpListener)
                {
                    this.hasActiveUdpListener = false;
                }
                HandleUdpException(ex);
            }
        }

        #endregion
    }
}