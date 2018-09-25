// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtuClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Ctu
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Timers;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ctu;
    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Requests;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;

    using NLog;

    /// <summary>
    /// Object tasked to manage all the interactions
    /// with the CU5 device.
    /// </summary>
    public class CtuClient : IManageableObject, IManageableTable
    {
        /// <summary>
        /// The local directory path where the downloaded files for CU5 shall be saved
        /// </summary>
        private const string FilesPathForCu5 = @"\ISM\CU5ExtractedFiles\";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Object tasked to serialize/deserialize CTU datagrams in
        /// buffers and vice versa, respectively.
        /// </summary>
        private readonly CtuSerializer ctuSerializer;

        /// <summary>
        /// Timer tasked to send periodically (see "periodSendCtu")
        /// CTU datagrams to the remote CU5 device.
        /// </summary>
        private readonly Timer timerCtuSender;

        /// <summary>
        /// Timer tasked to check periodically if the remote CU5 device
        /// is inactive since "maxInactivityTime" seconds.
        /// </summary>
        private readonly Timer timeoutCu5;

        private readonly Timer waitForRestart;

        private readonly Timer progressResponseTimeout;

        /// <summary>
        /// Variable that stores the periodic amount of ms to wait
        /// before sending cyclically the CTU status datagram.
        /// </summary>
        private readonly int periodSendCtu;

        /// <summary>
        /// Variable that stores the max. amount of seconds of
        /// CU5 inactivity, before setting it as inactive.
        /// </summary>
        private readonly int maxInactivityTimeSec;

        /// <summary>
        /// Queue tasked to store the telegrams received from the remote computer
        /// in a FIFO fashion.
        /// </summary>
        private readonly ProducerConsumerQueue<Triplet[]> itemsToSendQueue;

        private readonly Dictionary<string, DownloadStatusCode?> progressResponseStatus;

        private readonly Crc32 crc32;

        private readonly Dictionary<int, DisplayStatusCode> displayStatusCodes =
            new Dictionary<int, DisplayStatusCode>();

        /// <summary>
        /// The container of all the configuration parameters
        /// needed to interact with the remote CU5 device.
        /// </summary>
        private Cu5Config config;

        /// <summary>
        /// The object tasked to directly interact with the
        /// remote CU5 device using the UDP protocol.
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// The ip address and port of the remote CU5 device.
        /// </summary>
        private IPEndPoint remoteEndPoint;

        private StatusCode localStatus;

        /// <summary>
        /// The sequence number to use in the CTU datagrams to send.
        /// </summary>
        private int sequenceNumber;

        private bool udpConnected;

        private TripInfo currentTripInfo;

        private ExtendedLineInfo currentExtendedLineInfo;

        private bool isDownloading;

        private int retryCountDownloadStart;

        private int receivedDatagramCount;

        private StatusCode remoteStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="CtuClient"/> class.
        /// </summary>
        public CtuClient()
        {
            this.IsRunning = false;
            this.ctuSerializer = new CtuSerializer();
            this.maxInactivityTimeSec = 60;
            this.periodSendCtu = 20 * 1000;
            this.localStatus = StatusCode.Ok;
            this.crc32 = new Crc32();
            this.progressResponseStatus = new Dictionary<string, DownloadStatusCode?>();

            this.RemoteStatus = StatusCode.Ok;

            // now I initialize the timer that sends periodically
            // a CTU datagram as keep alive.
            this.timerCtuSender = new Timer(this.periodSendCtu);
            this.timerCtuSender.Elapsed += (s, e) => this.SendPeriodicUpdate();

            // and now I initialize the timer that check periodically
            // the CU5 activity status.
            this.timeoutCu5 = new Timer(this.maxInactivityTimeSec * 1000) { AutoReset = false };
            this.timeoutCu5.Elapsed += this.OnTimeoutCu5Elapsed;

            this.waitForRestart = new Timer(10000) { AutoReset = false };
            this.waitForRestart.Elapsed += (s, e) => this.BeginFileTransferToCu5();

            this.progressResponseTimeout = new Timer(50000) { AutoReset = false };
            this.progressResponseTimeout.Elapsed += (s, e) => this.ProgressResponseTimeout();

            this.itemsToSendQueue = new ProducerConsumerQueue<Triplet[]>(this.SendDatagram, 10);
        }

        /// <summary>
        /// Event that is fired whenever the (bidirectional) communication
        /// with the remote CU5 device is considered established
        /// (sent with success the first CTU datagram and received also its first CTU).
        /// </summary>
        public event EventHandler<EventArgs> CommunicationStarted;

        /// <summary>
        /// Event that is fired whenever occur errors on
        /// the socket with the CU5 device.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionErrorOccured;

        /// <summary>
        /// Event that is fired whenever the CU5 results inactive
        /// since to much time.
        /// </summary>
        public event EventHandler<EventArgs> InactivityStatusDetected;

        /// <summary>
        /// Event that is fired whenever the CU5 status changes.
        /// </summary>
        /// <seealso cref="RemoteStatus"/>
        public event EventHandler RemoteStatusChanged;

        /// <summary>
        /// Event that is fired whenever the CU5 display status changes.
        /// </summary>
        /// <seealso cref="DisplayStatusCodes"/>
        public event EventHandler DisplayStatusChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this object is running or not.
        /// </summary>
        [XmlIgnore]
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Gets or sets the device information.
        /// </summary>
        [XmlIgnore]
        public DeviceInfoResponse DeviceInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the local status code being sent to the remote CU5 device.
        /// </summary>
        [XmlIgnore]
        public StatusCode LocalStatus
        {
            get
            {
                return this.localStatus;
            }

            set
            {
                if (this.localStatus == value)
                {
                    return;
                }

                this.localStatus = value;
                this.EnqueueCtu(this.CreateCtuStatus());
            }
        }

        /// <summary>
        /// Gets the status of the CU5 received over CTU.
        /// </summary>
        public StatusCode RemoteStatus
        {
            get
            {
                return this.remoteStatus;
            }

            private set
            {
                if (this.remoteStatus == value)
                {
                    return;
                }

                this.remoteStatus = value;
                this.RaiseRemoteStatusChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the status codes of all displays known to the CU5.
        /// </summary>
        public IDictionary<int, DisplayStatusCode> DisplayStatusCodes
        {
            get
            {
                return this.displayStatusCodes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtuClient"/> class.
        /// </summary>
        /// <param name="cfg">The container of the ISM configurations.</param>
        public void Configure(Cu5Config cfg)
        {
            this.config = cfg;

            var address = IPAddress.Parse(this.config.IpAddress);
            this.remoteEndPoint = new IPEndPoint(address, this.config.Port);

            IPAddress localAddress;
            if (!string.IsNullOrEmpty(this.config.LocalAddress)
                && IPAddress.TryParse(this.config.LocalAddress, out localAddress))
            {
                var localEndPoint = new IPEndPoint(localAddress, this.config.Port);

                Logger.Info("Binding to local endpoint: {0}", localEndPoint);
                this.udpClient = new UdpClient(localEndPoint);
            }
            else
            {
                Logger.Info("Binding to local port: {0}", this.config.Port);
                this.udpClient = new UdpClient(this.config.Port);
            }
        }

        /// <summary>
        /// Triggers this object in order to make starting all its
        /// internal activities.
        /// </summary>
        public virtual void Start()
        {
            if (this.IsRunning)
            {
                // already running.
                // I avoid to run it twice.
                return;
            }

            this.DeviceInfo = null;

            try
            {
                if (!this.udpConnected)
                {
                    // we can only call Connect() exactly once
                    this.udpClient.Connect(this.remoteEndPoint);
                    this.udpConnected = true;
                }

                this.IsRunning = true;
                Logger.Info("CU5 connected port {0} to {1}", this.config.Port, this.remoteEndPoint);

                this.udpClient.BeginReceive(this.Cu5CtuDatagramReceived, null);

                // it's the time to start a timer that
                // sends periodically a CTU datagram.
                this.timerCtuSender.Start();

                // and the producer/consumer queue for the trip infor to send to the CU5.
                this.itemsToSendQueue.StartConsumer();

                this.SendPeriodicUpdate();

                this.ResetTimeoutTimer();
            }
            catch (Exception ex)
            {
                // an error was occured on the socket.
                Logger.Warn(ex, "Couldn't start UDP connection");
                this.NotifyConnectionError(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops all the activities with the remote CU5 device.
        /// </summary>
        public virtual void Stop()
        {
            if (!this.IsRunning)
            {
                // already stopped.
                // I avoid to stop it twice.
                return;
            }

            // I stop the timer.
            this.timerCtuSender.Stop();

            // I stop the timer.
            this.timeoutCu5.Stop();

            // I stop the producer/consumer queue for the trip infor to send to the CU5.
            this.itemsToSendQueue.StopConsumer();
            this.IsRunning = false;
            Logger.Info("CU5 stopped.");
        }

        /// <summary>
        /// Triggers the CU5 to start downloading files.
        /// </summary>
        public void BeginFileTransferToCu5()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileListForCu5 = Directory.GetFiles(path + FilesPathForCu5);
            this.SendDownloadStartDatagram(fileListForCu5);
        }

        /// <summary>
        /// Handles a given ISI message. Currently this method
        /// just checks for line number and destination.
        /// </summary>
        /// <param name="isiMessage">the message.</param>
        public void HandleIsiMessage(IsiMessageBase isiMessage)
        {
            var isiPut = isiMessage as IsiPut;
            if (isiPut == null)
            {
                // we are only interested in IsiPut
                return;
            }

            if (this.LocalStatus == StatusCode.Fallback)
            {
                // during the fallback status we don't have to send information
                // about the destination to the CU5
                return;
            }

            var countdownState = isiPut.Items.Find(item => item.Name == DataItemName.StopDepartureCountdownState);
            if (countdownState != null)
            {
                sbyte countdownNumber;
                if (!sbyte.TryParse(countdownState.Value, out countdownNumber))
                {
                    return;
                }

                var stopDepartureCountdownState = new CountdownNumber { Number = countdownNumber };
                Logger.Debug(
                    "Sending CountdownNumber to CU5: {0}",
                    stopDepartureCountdownState.Number);
                this.EnqueueCtu(stopDepartureCountdownState);
            }

            var lineNumber = isiPut.Items.Find(item => item.Name == DataItemName.LineNameForDisplay);
            var directionNumber = isiPut.Items.Find(item => item.Name == DataItemName.CurrentDirectionNo);
            var destinationNumber = isiPut.Items.Find(item => item.Name == DataItemName.DestinationNo);
            var destination = isiPut.Items.Find(item => item.Name == DataItemName.Destination);
            var destArabic = isiPut.Items.Find(item => item.Name == DataItemName.DestinationArabic);
            if (lineNumber == null || directionNumber == null || destination == null || destArabic == null
                || destinationNumber == null)
            {
                // we don't have all information required
                return;
            }

            var extendedLineInfo = new ExtendedLineInfo(
                destinationNumber.Value, directionNumber.Value, lineNumber.Value, destination.Value, destArabic.Value);
            Logger.Debug(
                    "Sending line info to CU5: {0}, {1}, {2}, {3}, {4}",
                    extendedLineInfo.DestinationNo,
                    extendedLineInfo.CurrentDirectionNo,
                    extendedLineInfo.LineNumber,
                    extendedLineInfo.Destination,
                    extendedLineInfo.DestinationArabic);
            this.currentExtendedLineInfo = extendedLineInfo;
            this.EnqueueCtu(extendedLineInfo);

            var tripInfo = new TripInfo(lineNumber.Value + directionNumber.Value, destination.Value, destArabic.Value);
            Logger.Debug(
                "Sending trip info to CU5: {0}, {1}, {2}",
                tripInfo.LineNumber,
                tripInfo.Destination,
                tripInfo.DestinationArabic);
            this.currentTripInfo = tripInfo;
            this.EnqueueCtu(tripInfo);
        }

        /// <summary>
        /// The send special input data.
        /// </summary>
        /// <param name="specialInput">
        /// The special Input.
        /// </param>
        public void SendSpecialInputState(bool specialInput)
        {
            var specialInputState = new SpecialInputInfo(specialInput);
            Logger.Debug("Sending Special Input state t CU5: {0}", specialInputState.SpecialInputState);
            this.EnqueueCtu(specialInputState);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var response in this.progressResponseStatus)
            {
                if (response.Value != null)
                {
                    yield return new List<ManagementProperty>
                                     {
                                         new ManagementProperty<string>("Filename", response.Key, true),
                                         new ManagementProperty<DownloadStatusCode>(
                                             "Download Status", (DownloadStatusCode)response.Value, true),
                                     };
                }
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Cu5 Connected", this.udpConnected, true);
            yield return new ManagementProperty<string>("Cu5 Status", this.RemoteStatus.ToString(), true);
            yield return new ManagementProperty<int>("Datagrams Sent", this.sequenceNumber, true);
            yield return new ManagementProperty<int>("Datagrams Received", this.receivedDatagramCount, true);
            yield return new ManagementProperty<int>("Cu5 Serial Number", this.DeviceInfo.SerialNumber, true);
            yield return new ManagementProperty<string>("Cu5 Data Version", this.DeviceInfo.DataVersion, true);
            yield return new ManagementProperty<string>("Cu5 Software Version", this.DeviceInfo.SoftwareVersion, true);
        }

        /// <summary>
        /// Sends a CTU datagram to the CU5.
        /// </summary>
        /// <param name="triplets">
        /// The triplets to send.
        /// </param>
        protected virtual void SendDatagram(params Triplet[] triplets)
        {
            var header = new Header { SequenceNumber = this.sequenceNumber++ };
            var payload = new Payload { Triplets = new List<Triplet>(triplets) };
            var datagram = new CtuDatagram(header, payload);
            try
            {
                byte[] buffer = this.ctuSerializer.Serialize(datagram);
                this.udpClient.Send(buffer, buffer.Length);

                Logger.Debug("Datagram sent (seq. num: {0})", datagram.Header.SequenceNumber);
                Logger.Trace(datagram.ToString);
            }
            catch (Exception ex)
            {
                // an error was occured on the socket.
                Logger.Warn(ex, "Couldn't send UDP");
                this.NotifyConnectionError(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies all the registered handlers about
        /// the establishment of the communication channel
        /// with the remote CU5 device.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCommunicationStarted(EventArgs e)
        {
            if (this.CommunicationStarted != null)
            {
                this.CommunicationStarted(this, e);
            }
        }

        /// <summary>
        /// Notifies all the registered handlers about
        /// an error occurred on the socket with the CU5 device.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void NotifyConnectionError(EventArgs e)
        {
            if (this.ConnectionErrorOccured != null)
            {
                this.ConnectionErrorOccured(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RemoteStatusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRemoteStatusChanged(EventArgs e)
        {
            var handler = this.RemoteStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DisplayStatusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDisplayStatusChanged(EventArgs e)
        {
            var handler = this.DisplayStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ResetTimeoutTimer()
        {
            this.timeoutCu5.Stop();
            this.timeoutCu5.Start();
        }

        private void EnqueueCtu(params Triplet[] triplets)
        {
            this.itemsToSendQueue.Enqueue(triplets);
        }

        /// <summary>
        /// Function called whenever a UDP datagram is received.
        /// </summary>
        /// <param name="asynchResult">The container of information about the asynchronous
        /// I/O activity just done.</param>
        private void Cu5CtuDatagramReceived(IAsyncResult asynchResult)
        {
            if (!this.IsRunning)
            {
                // UDP not started.
                return;
            }

            byte[] receiveBytes;
            try
            {
                receiveBytes = this.udpClient.EndReceive(asynchResult, ref this.remoteEndPoint);
            }
            catch (Exception ex)
            {
                // an error was occured on the socket.
                Logger.Warn(ex, "Couldn't end receive UDP");
                this.NotifyConnectionError(EventArgs.Empty);
                return;
            }

            try
            {
                var datagram = this.ctuSerializer.Deserialize(receiveBytes);
                Logger.Debug("Datagram received (seq. num: {0})", datagram.Header.SequenceNumber);
                Logger.Trace(datagram.ToString);

                this.ResetTimeoutTimer();
                this.HandleDatagram(datagram);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't handle datagram");
            }

            try
            {
                // and finally I re-enable the read for the next CTU datagram...
                this.udpClient.BeginReceive(this.Cu5CtuDatagramReceived, null);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't begin receive UDP");
                this.NotifyConnectionError(EventArgs.Empty);
            }
        }

        private void HandleDatagram(CtuDatagram datagram)
        {
            this.receivedDatagramCount++;
            var downloadFileList = new List<string>();
            foreach (var triplet in datagram.Payload.Triplets)
            {
                var deviceInfo = triplet as DeviceInfoResponse;
                if (deviceInfo != null)
                {
                    this.DeviceInfo = deviceInfo;

                    // now we have all necessary information to consider
                    // the CU5 communication started
                    this.RaiseCommunicationStarted(EventArgs.Empty);
                    continue;
                }

                var status = triplet as Status;
                if (status != null)
                {
                    this.RemoteStatus = status.Code;
                    continue;
                }

                var displayStatus = triplet as DisplayStatus;
                if (displayStatus != null)
                {
                    this.HandleDisplayStatus(displayStatus.Items);
                    continue;
                }

                var downloadProgress = triplet as DownloadProgressResponse;
                if (downloadProgress != null && this.progressResponseStatus.ContainsKey(downloadProgress.FileAbsName))
                {
                    // Stop timer waiting for download progress response
                    this.progressResponseTimeout.Stop();
                    this.progressResponseStatus[downloadProgress.FileAbsName] = downloadProgress.StatusCode;
                    string filename = this.ProcessDownloadProgressResponse(downloadProgress);
                    if (filename != string.Empty)
                    {
                        downloadFileList.Add(filename);
                    }

                    continue;
                }

                var logMessage = triplet as LogMessage;
                if (logMessage != null)
                {
                    Logger.Info("Log from CU5: {0}", logMessage.Message);
                }
            }

            if (downloadFileList.Count > 0)
            {
                this.HandleRetry(downloadFileList);
            }

            this.CheckStatusOfResponses();
        }

        private void HandleDisplayStatus(IEnumerable<DisplayStatus.Item> items)
        {
            var updated = false;
            foreach (var item in items)
            {
                DisplayStatusCode code;
                if (this.displayStatusCodes.TryGetValue(item.Id, out code) && code == item.Status)
                {
                    continue;
                }

                this.displayStatusCodes[item.Id] = item.Status;
                updated = true;
            }

            if (!updated)
            {
                return;
            }

            this.RaiseDisplayStatusChanged(EventArgs.Empty);
        }

        private void CheckStatusOfResponses()
        {
            foreach (var status in this.progressResponseStatus)
            {
                if (!status.Value.HasValue || status.Value.Value != DownloadStatusCode.Success)
                {
                    this.isDownloading = true;
                    return;
                }
            }

            this.isDownloading = false;
            this.progressResponseStatus.Clear();
        }

        private string ProcessDownloadProgressResponse(DownloadProgressResponse downloadProgress)
        {
            Logger.Info(
                "Recieved status code {0} for file {1}", downloadProgress.StatusCode, downloadProgress.FileAbsName);
            switch (downloadProgress.StatusCode)
            {
                case DownloadStatusCode.Success:
                    {
                        // Delete the file from local location as its been downloaded correctly by CU5
                        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        if (string.IsNullOrEmpty(path))
                        {
                            // invalid path.
                            Logger.Error("Cannot get the assembly location");
                            break;
                        }

                        // "path" for the moment, contains only the directory in which is running Protran.exe
                        // usually D:\\Progs\\Protran.
                        // to it, I've to add "ISM\\CU5ExtractedFiles"
                        path = string.Format("{0}{1}", path, FilesPathForCu5);

                        // now "path" is D:\\Progs\\Protran\\ISM\\CU5ExtractedFiles
                        // and for safety I remove (eventually) the last backslash
                        if (path.EndsWith("\\"))
                        {
                            path = path.Remove(path.Length - 1);
                        }

                        var file =
                            new FileInfo(
                                string.Format(
                                    "{0}{1}{2}", path, Path.DirectorySeparatorChar, downloadProgress.FileAbsName));
                        if (File.Exists(file.FullName))
                        {
                            File.Delete(file.FullName);
                            Logger.Info("Deleted file {0} after a full CU5 download.", file.FullName);
                        }
                        else
                        {
                            Logger.Info("Not deleted file {0} after a full CU5 download.", file.FullName);
                        }

                        // file.Delete(); <== if the file doesn't exist, this function doesn't throw any exception.
                        this.progressResponseStatus.Remove(downloadProgress.FileAbsName);
                        break;
                    }

                case DownloadStatusCode.Queued:
                case DownloadStatusCode.Downloading:
                    {
                        break;
                    }

                case DownloadStatusCode.GeneralError:
                case DownloadStatusCode.LowMemory:
                case DownloadStatusCode.ConnectionError:
                case DownloadStatusCode.BadCrc:
                    {
                        return downloadProgress.FileAbsName;
                    }
            }

            return string.Empty;
        }

        private void HandleRetry(IEnumerable<string> downloadFileList)
        {
            if (this.retryCountDownloadStart <= 10)
            {
                this.retryCountDownloadStart++;
                this.SendDownloadStartDatagram(downloadFileList);
            }
            else
            {
                this.retryCountDownloadStart = 0;
                this.EnqueueCtu(new DownloadAbort());
                this.progressResponseStatus.Clear();
                Logger.Info("Sent Download Abort datagram due download errors");
                this.waitForRestart.Start();
            }
        }

        private void ProgressResponseTimeout()
        {
            this.EnqueueCtu(new DownloadAbort());
            this.progressResponseStatus.Clear();
            Logger.Info("Sent Download Abort datagram due to Progress Response timeout");
            this.waitForRestart.Start();
        }

        /// <summary>
        /// Function called by the O.S. whenever is elapsed the
        /// "timerCtuSender" period.
        /// </summary>
        private void SendPeriodicUpdate()
        {
            // timeout elapsed.
            var triplets = new List<Triplet>(5);

            triplets.Add(this.CreateCtuStatus());

            if (this.LocalStatus == StatusCode.Fallback)
            {
                // don't send trip and line info if we are in fallback mode
                this.currentTripInfo = null;
                this.currentExtendedLineInfo = null;
            }
            else
            {
                if (this.currentTripInfo != null)
                {
                    // this object has collected all the information
                    // regarding the trip info.
                    // so, now it's the time to send to the remote CU5
                    // also this kind of info.
                    triplets.Add(this.currentTripInfo);
                }

                if (this.currentExtendedLineInfo != null)
                {
                    // this object has collected all the information
                    // regarding the extended line info.
                    // so, now it's the time to send to the remote CU5
                    // also this kind of info.
                    triplets.Add(this.currentExtendedLineInfo);
                }
            }

            if (this.DeviceInfo == null)
            {
                triplets.Add(new DeviceInfoRequest());
            }

            if (this.isDownloading)
            {
                triplets.Add(new DownloadProgressRequest());
                Logger.Debug("Sent Progress Request datagram");

                // Start timer to wait for download progress response
                this.progressResponseTimeout.Start();
            }

            this.EnqueueCtu(triplets.ToArray());
        }

        /// <summary>
        /// Function called by the O.S. whenever is elapsed the
        /// "timerCu5ActivityChecker" period.
        /// </summary>
        /// <param name="sender">Who invokes this function.</param>
        /// <param name="e">The event about the timer's period expiration.</param>
        private void OnTimeoutCu5Elapsed(object sender, EventArgs e)
        {
            // the CU5 device is inactive since to much time.
            // I've to notify this situation to all the registered handlers.
            if (this.InactivityStatusDetected != null)
            {
                this.InactivityStatusDetected(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates the CTU status triplet.
        /// </summary>
        /// <returns>
        /// The created CTU status triplet.
        /// </returns>
        private Status CreateCtuStatus()
        {
            return new Status(this.LocalStatus, "Protran status: " + this.LocalStatus);
        }

        private void SendDownloadStartDatagram(IEnumerable<string> fileListForCu5)
        {
            var triplets = new List<Triplet>();
            foreach (string filename in fileListForCu5)
            {
                var downloadStart = new DownloadStart();
                var fileinfo = new FileInfo(filename);
                int calculatedCrc;
                try
                {
                    // CRC 32 calculation for the file
                    using (var fs = File.Open(fileinfo.FullName, FileMode.Open))
                    {
                        var hash = this.crc32.ComputeHash(fs);

                        if (BitConverter.IsLittleEndian)
                        {
                            // big to little endian
                            Array.Reverse(hash);
                        }

                        calculatedCrc = BitConverter.ToInt32(hash, 0);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "CRC not calculated for: {0}", filename);
                    continue;
                }

                int index = fileinfo.FullName.IndexOf(FilesPathForCu5, StringComparison.InvariantCultureIgnoreCase);
                downloadStart.FileAbsPath = fileinfo.FullName.Substring(index + FilesPathForCu5.Length - 1)
                                                    .Replace('\\', '/');
                downloadStart.FileSize = (uint)fileinfo.Length;
                downloadStart.FileCrc = calculatedCrc;
                triplets.Add(downloadStart);
                this.progressResponseStatus[downloadStart.FileAbsPath] = null;
                Logger.Info("Download Start contains file {0}", downloadStart.FileAbsPath);
            }

            if (triplets.Count > 0)
            {
                this.EnqueueCtu(triplets.ToArray());
                Logger.Info("Sent Download Start datagram to CU5 with {0} triplets", triplets.Count);
                this.isDownloading = true;
            }
        }
    }
}
