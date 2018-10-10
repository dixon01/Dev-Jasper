// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.TopboxSimulator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ctu;
    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Requests;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Tools.TopboxSimulator.Properties;

    using NLog;

    /// <summary>
    /// Main form of the application.
    /// </summary>
    public partial class MainForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Timer receiveTimeout = new Timer();
        private readonly Timer statusUpdateTimer = new Timer();
        private readonly Timer tripUpdateTimer = new Timer();
        private readonly Timer lineInfoTimer = new Timer();
        private readonly Timer extendedLineInfoTimer = new Timer();
        private readonly Timer extSignTextsTimer = new Timer();

        private readonly Timer progressResponseTimeout = new Timer();
        private readonly Timer waitForRestart = new Timer();

        private readonly CtuSerializer ctuSerializer = new CtuSerializer();

        private readonly Crc32 crc32;

        private readonly Dictionary<string, DownloadStatusCode?> progressResponseStatus;

        private TransferStatus transferStatus;

        private bool started;

        private UdpClient udpServer;

        private int sequenceNumber;

        private bool gotFirstDatagram;

        private string filesPathForCu5;

        private bool isDownloading;

        private int retryCountDownloadStart;

        private bool specialInputState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.textBoxCu5IpAddress.Text = Settings.Default.Cu5IpAddress;

            this.receiveTimeout.Interval = 60000;
            this.receiveTimeout.Tick += (s, e) => this.checkBoxCu5Present.CheckState = CheckState.Indeterminate;
            this.statusUpdateTimer.Tick += (s, e) => this.SendStatusUpdate();
            this.tripUpdateTimer.Tick += (s, e) => this.SendTripUpdate();
            this.lineInfoTimer.Tick += (s, e) => this.SendLineInfoUpdate();
            this.extendedLineInfoTimer.Tick += (s, e) => this.SendExtendedLineInfoUpdate();
            this.extSignTextsTimer.Tick += (s, e) => this.SendExtSignTextsUpdate();

            this.progressResponseTimeout.Interval = 50000;
            this.progressResponseTimeout.Tick += (s, e) => this.ProgressResponseTimeout();

            this.waitForRestart.Interval  = 10000;
            this.waitForRestart.Tick += (s, e) => this.GetFilesFromFolder(this.filesPathForCu5);

            this.EnableStatusUpdate();
            this.EnableTripUpdate();
            this.EnableLineInfoUpdate();
            this.EnableExtendedLineInfoUpdate();
            this.EnableExtSignTextsUpdate();
            this.transferStatus = new TransferStatus();
            this.crc32 = new Crc32();
            this.progressResponseStatus = new Dictionary<string, DownloadStatusCode?>();
            this.specialInputState = true;
        }

        private void ProgressResponseTimeout()
        {
            this.PerformDownloadProcessAbort();
            Logger.Info("Sent Download Abort datagram due to Progress Response timeout");
            this.waitForRestart.Start();
        }

        private void Stop()
        {
            this.started = false;
            this.udpServer.Close();
            this.udpServer = null;

            this.receiveTimeout.Stop();
            this.EnableStatusUpdate();
            this.EnableTripUpdate();
            this.EnableLineInfoUpdate();
            this.EnableExtendedLineInfoUpdate();
            this.EnableExtSignTextsUpdate();
        }

        private void Start()
        {
            this.started = true;
            this.gotFirstDatagram = false;

            this.textBoxSerialNumber.Clear();
            this.textBoxDataVersion.Clear();
            this.textBoxSoftwareVersion.Clear();
            this.textBoxCu5StatusCode.Clear();
            this.textBoxCu5StatusText.Clear();
            this.textBoxCu5LineNumber.Clear();
            this.textBoxCu5Destination.Clear();
            this.textBoxCu5DestinationArabic.Clear();

            var port = (int)this.numericUpDown_port.Value;
            this.udpServer = new UdpClient(port);
            this.udpServer.Connect(IPAddress.Parse(this.textBoxCu5IpAddress.Text), port);
            this.udpServer.BeginReceive(this.Received, null);
            this.EnableStatusUpdate();
            this.EnableTripUpdate();
            this.EnableLineInfoUpdate();
            this.EnableExtendedLineInfoUpdate();
            this.EnableExtSignTextsUpdate();
        }

        private void EnableStatusUpdate()
        {
            var enabled = this.checkBoxStatusUpdate.Checked && this.started;
            this.statusUpdateTimer.Stop();
            this.statusUpdateTimer.Interval = (int)this.numericUpDown_period.Value;
            this.statusUpdateTimer.Enabled = enabled;
            this.numericUpDown_period.Enabled = !enabled;
            this.progressResponseTimeout.Enabled = enabled && this.isDownloading;

            if (enabled)
            {
                this.SendStatusUpdate();
            }
        }

        private void EnableTripUpdate()
        {
            var enabled = this.checkBoxLineNumber.Checked && !this.checkBoxCombine.Checked && this.started;
            this.tripUpdateTimer.Stop();
            this.tripUpdateTimer.Interval = (int)this.numericUpDownLineInfoPeriod.Value;
            this.tripUpdateTimer.Enabled = enabled;
            this.numericUpDownLineInfoPeriod.Enabled = !enabled;
            this.checkBoxLineNumber.Enabled = !this.checkBoxCombine.Checked;

            if (enabled)
            {
                this.SendTripUpdate();
            }
        }

        private void EnableLineInfoUpdate()
        {
            var enabled = this.checkBoxLineInfo.Checked && !this.checkBoxCombineLineInfo.Checked && this.started;
            this.lineInfoTimer.Stop();
            this.lineInfoTimer.Interval = (int)this.numericUpDownLineInfoDatagramPeriod.Value;
            this.lineInfoTimer.Enabled = enabled;
            this.numericUpDownLineInfoDatagramPeriod.Enabled = !enabled;
            this.checkBoxLineInfo.Enabled = !this.checkBoxCombineLineInfo.Checked;

            if (enabled)
            {
                this.SendLineInfoUpdate();
            }
        }

        private void EnableExtendedLineInfoUpdate()
        {
            var enabled = this.checkBoxExiLineInfo.Checked && !this.checkBoxCombineExiLineInfo.Checked && this.started;
            this.extendedLineInfoTimer.Stop();
            this.extendedLineInfoTimer.Interval = (int)this.numericUpDownExiLineInfoPeriod.Value;
            this.extendedLineInfoTimer.Enabled = enabled;
            this.numericUpDownExiLineInfoPeriod.Enabled = !enabled;
            this.checkBoxExiLineInfo.Enabled = !this.checkBoxCombineExiLineInfo.Checked;

            if (enabled)
            {
                this.SendExtendedLineInfoUpdate();
            }
        }

        private void EnableExtSignTextsUpdate()
        {
            var enabled = this.checkBoxExtSignTexts.Checked
                && !this.checkBoxCombineExtSignTexts.Checked && this.started;
            this.extSignTextsTimer.Stop();
            this.extSignTextsTimer.Interval = (int)this.numericUpDownExtSignTextsPeriod.Value;
            this.extSignTextsTimer.Enabled = enabled;
            this.numericUpDownExtSignTextsPeriod.Enabled = !enabled;
            this.checkBoxExtSignTexts.Enabled = !this.checkBoxCombineExtSignTexts.Checked;

            if (enabled)
            {
                this.SendExtSignTextsUpdate();
            }
        }

        private void SendStatusUpdate()
        {
            var triplets = new List<Triplet>(3);
            triplets.Add(new Status((StatusCode)this.numericUpDownStatus.Value, this.textBoxStatus.Text));

            if (this.checkBoxCombineExiLineInfo.Checked)
            {
                triplets.Add(this.CreateExtendedLineInfo());
            }

            if (this.checkBoxCombineExtSignTexts.Checked)
            {
                triplets.Add(this.CreateExtSignTexts());
            }

            if (this.checkBoxCombineLineInfo.Checked)
            {
                triplets.Add(this.CreateLineInfo());
            }

            if (this.checkBoxCombine.Checked)
            {
                triplets.Add(this.CreateTripInfo());
            }

            this.SendDatagram(triplets.ToArray());

            if (this.isDownloading)
            {
                this.PerformDownloadProgressRequest();

                // Start timer to wait for download progress response
                this.progressResponseTimeout.Start();
            }
        }

        private void SendTripUpdate()
        {
            this.SendDatagram(this.CreateTripInfo());
        }

        private void SendLineInfoUpdate()
        {
            this.SendDatagram(this.CreateLineInfo());
        }

        private void SendExtendedLineInfoUpdate()
        {
            this.SendDatagram(this.CreateExtendedLineInfo());
        }

        private void SendExtSignTextsUpdate()
        {
            this.SendDatagram(this.CreateExtSignTexts());
        }

        private TripInfo CreateTripInfo()
        {
            return new TripInfo(
                this.textBoxLineNumber.Text,
                this.textBoxDestination.Text,
                this.textBoxDestinationArabic.Text);
        }

        private LineInfo CreateLineInfo()
        {
            return new LineInfo(
                this.textBoxDestinationNo.Text,
                this.textBoxCurrentDirectionNo.Text,
                this.textBoxLineInfoLineNumber.Text);
        }

        private ExtendedLineInfo CreateExtendedLineInfo()
        {
            return new ExtendedLineInfo(
                this.textBoxExiDestinationNo.Text,
                this.textBoxExiCurrentDirectionNo.Text,
                this.textBoxExiLineNumber.Text,
                this.textBoxExiDestination.Text,
                this.textBoxExiDestinationArabic.Text);
        }

        private ExteriorSignTexts CreateExtSignTexts()
        {
            return new ExteriorSignTexts(
                (byte)this.numericUpDownExtSignTextsAddr.Value,
                this.textBoxText1.Text,
                this.textBoxText2.Text);
        }

        private void SendDatagram(params Triplet[] triplets)
        {
            if (!this.started)
            {
                Logger.Warn("Trying to send datagram when we are not started");
                return;
            }

            var header = new Header { SequenceNumber = this.sequenceNumber++ };
            var payload = new Payload { Triplets = new List<Triplet>(triplets) };
            var datagram = new CtuDatagram(header, payload);

            var bytes = this.ctuSerializer.Serialize(datagram);
            this.udpServer.Send(bytes, bytes.Length);
            Logger.Info("Sent datagram {0} with {1} triplets", header.SequenceNumber, payload.Triplets.Count);
        }

        private void Received(IAsyncResult ar)
        {
            if (!this.started)
            {
                // UDP not started.
                return;
            }

            IPEndPoint endPoint = null;
            byte[] receiveBytes;
            try
            {
                receiveBytes = this.udpServer.EndReceive(ar, ref endPoint);
            }
            catch (Exception e)
            {
                this.HandleException(e);
                return;
            }

            var datagram = this.ctuSerializer.Deserialize(receiveBytes);
            this.Invoke(new MethodInvoker(() => this.HandleDatagram(datagram)));

            // let's re-enable the read for the next CTU datagram...
            this.udpServer.BeginReceive(this.Received, null);
        }

        private void HandleDatagram(CtuDatagram datagram)
        {
            if (datagram.Header.VersionNumber != 1)
            {
                Logger.Error("Bad CTU version from CU5: {0}", datagram.Header.VersionNumber);
                return;
            }

            if (datagram.Header.Flags != HeaderFlags.LittleEndian)
            {
                Logger.Error("Bad header flags from CU5: {0}", datagram.Header.Flags);
                return;
            }

            if (!this.gotFirstDatagram)
            {
                this.gotFirstDatagram = true;
                this.SendDatagram(new DeviceInfoRequest());
            }

            this.checkBoxCu5Present.Checked = true;
            this.receiveTimeout.Stop();
            this.receiveTimeout.Start();

            Logger.Info("Received datagram {0}", datagram.Header.SequenceNumber);

            foreach (var triplet in datagram.Payload.Triplets)
            {
                Logger.Debug(" -> {0}: {1}", triplet.Tag, triplet);
                switch (triplet.Tag)
                {
                    case TagName.Status:
                        var status = (Status)triplet;
                        this.textBoxCu5StatusCode.Text = status.Code.ToString();
                        this.textBoxCu5StatusText.Text = status.StatusMsg;
                        break;
                    case TagName.LogMessage:
                        var log = (LogMessage)triplet;
                        this.textBoxLog.AppendText(log.Message + Environment.NewLine);
                        break;
                    case TagName.DisplayStatus:
                        this.listBoxDisplayStatus.Items.Clear();
                        var displayStatus = (DisplayStatus)triplet;
                        foreach (var item in displayStatus.Items)
                        {
                            this.listBoxDisplayStatus.Items.Add(string.Format("{0}: {1}", item.Id, item.Status));
                        }

                        break;
                    case TagName.TripInfo:
                        var trip = (TripInfo)triplet;
                        this.textBoxCu5LineNumber.Text = trip.LineNumber;
                        this.textBoxCu5Destination.Text = trip.Destination;
                        this.textBoxCu5DestinationArabic.Text = trip.DestinationArabic;
                        break;
                    case TagName.DeviceInfoResponse:
                        var info = (DeviceInfoResponse)triplet;
                        this.textBoxSerialNumber.Text = info.SerialNumber.ToString("X8");
                        this.textBoxSoftwareVersion.Text = info.SoftwareVersion;
                        this.textBoxDataVersion.Text = info.DataVersion;
                        break;
                    case TagName.DownloadProgressResponse:
                        var downloadProgress = (DownloadProgressResponse)triplet;
                        this.HandleProgressResponse(downloadProgress);
                        break;
                    default:
                        Logger.Warn("Unsupported tag: {0}", triplet.Tag);
                        break;
                }
            }
        }

        private void HandleProgressResponse(DownloadProgressResponse downloadProgress)
        {
            string filename = this.ProcessDownloadProgressResponse(downloadProgress);
            var downloadFileList = new List<string>();
            if (this.progressResponseStatus.ContainsKey(downloadProgress.FileAbsName))
            {
                // Stop timer waiting for download progress response
                this.progressResponseTimeout.Stop();
                this.progressResponseStatus[downloadProgress.FileAbsName] = downloadProgress.StatusCode;
                if (filename != string.Empty)
                {
                    downloadFileList.Add(filename);
                }
            }

            if (downloadFileList.Count > 0)
            {
                this.HandleRetry(downloadFileList);
            }

            this.CheckStatusOfResponses();
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
            this.waitForRestart.Stop();
            this.progressResponseStatus.Clear();
        }

        private void HandleRetry(IEnumerable<string> downloadFileList)
        {
            if (this.retryCountDownloadStart < 10)
            {
                this.retryCountDownloadStart++;
                this.SendDownloadStartDatagram(downloadFileList);
            }
            else
            {
                this.retryCountDownloadStart = 0;
                this.PerformDownloadProcessAbort();
                Logger.Info("Sent Download Abort datagram due download errors");
                this.waitForRestart.Start();
            }
        }

        private string ProcessDownloadProgressResponse(DownloadProgressResponse downloadProgress)
        {
            this.transferStatus.DisplayDatagramReceived(downloadProgress);
            switch (downloadProgress.StatusCode)
            {
                case DownloadStatusCode.Success:
                    {
                        // Delete the file from local location as its been downloaded correctly by CU5
                        // var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        // var file =
                        // new FileInfo(
                        // string.Format(
                        // "{0}{1}{2}", path, Path.DirectorySeparatorChar, downloadProgress.FileAbsName));
                        // file.Delete();
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

        private void HandleException(Exception ex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.HandleException(ex)));
                return;
            }

            this.textBoxLog.AppendText(ex.Message + Environment.NewLine);
            this.Stop();
        }

        private void ButtonStartStopClick(object sender, EventArgs e)
        {
            this.checkBoxCu5Present.Checked = false;

            if (this.started)
            {
                // the user wants to stop.
                this.Stop();
            }
            else
            {
                // the user want to start.
                this.Start();
            }

            this.button_startStop.Text = this.started ? "Stop" : "Start";
        }

        private void CheckBoxStatusUpdateCheckedChanged(object sender, EventArgs e)
        {
            this.EnableStatusUpdate();
        }

        private void CheckBoxCombineCheckedChanged(object sender, EventArgs e)
        {
            this.EnableTripUpdate();
        }

        private void CheckBoxLineNumberCheckedChanged(object sender, EventArgs e)
        {
            this.EnableTripUpdate();
        }

        private void CheckBoxLineInfoCheckedChanged(object sender, EventArgs e)
        {
            this.EnableLineInfoUpdate();
        }

        private void CheckBoxCombineLineInfoCheckedChanged(object sender, EventArgs e)
        {
            this.EnableLineInfoUpdate();
        }

        private void CheckBoxExiLineInfoCheckedChanged(object sender, EventArgs e)
        {
            this.EnableExtendedLineInfoUpdate();
        }

        private void CheckBoxCombineExiLineInfoCheckedChanged(object sender, EventArgs e)
        {
            this.EnableExtendedLineInfoUpdate();
        }

        private void CheckBoxExtSignTextsCheckedChanged(object sender, EventArgs e)
        {
            this.EnableExtSignTextsUpdate();
        }

        private void CheckBoxCombineExtSignTextsCheckedChanged(object sender, EventArgs e)
        {
            this.EnableExtSignTextsUpdate();
        }

        private void TextBoxCu5IpAddressTextChanged(object sender, EventArgs e)
        {
            if (Settings.Default.Cu5IpAddress == this.textBoxCu5IpAddress.Text)
            {
                return;
            }

            Settings.Default.Cu5IpAddress = this.textBoxCu5IpAddress.Text;
            Settings.Default.Save();
        }

        private void StatusTabToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.transferStatus != null)
            {
                this.transferStatus.Close();
                this.transferStatus.Dispose();
            }

            this.transferStatus = new TransferStatus();
            this.transferStatus.Show();
        }

        private void ButtonBrowseClick(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() != DialogResult.OK)
            {
                // the user doesn't select any folder.
                return;
            }

            // the user has selected a folder.
            this.textBox_folderBrowse.Text = this.folderBrowserDialog1.SelectedPath;
            this.filesPathForCu5 = this.textBox_folderBrowse.Text;
        }

        private void GetFilesFromFolder(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                // nothing to get.
                return;
            }

            var fileListForCu5 = Directory.GetFiles(filepath);
            if (fileListForCu5.Length == 0)
            {
                MessageBox.Show(
                    this,
                    @"Nothing to send.",
                    @"TopBoxSimulator",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            this.SendDownloadStartDatagram(fileListForCu5);
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
                catch (Exception e)
                {
                    Logger.ErrorException("CRC not calculated due to exception", e);
                    continue;
                }

                int index = fileinfo.FullName.IndexOf("ISM", StringComparison.InvariantCultureIgnoreCase);
                if (index == -1)
                {
                    MessageBox.Show(
                        this,
                        @"Select the directory called ""ISM"".",
                        @"TopBoxSimulator",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                downloadStart.FileAbsPath = fileinfo.FullName.Substring(index - 1).Replace('\\', '/');
                downloadStart.FileSize = (uint)fileinfo.Length;
                downloadStart.FileCrc = calculatedCrc;
                triplets.Add(downloadStart);
                this.progressResponseStatus[downloadStart.FileAbsPath] = null;
                Logger.Info("Download Start contains file {0}", downloadStart.FileAbsPath);
            }

            if (triplets.Count > 0)
            {
                this.SendDatagram(triplets.ToArray());
                this.transferStatus.Show();
                this.transferStatus.DisplayDatagramSent(triplets);
                this.isDownloading = true;
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonManualDownloadCommandClick(object sender, EventArgs e)
        {
            var buttonSender = sender as Button;
            if (buttonSender == null)
            {
                // invalid sender
                return;
            }

            if (buttonSender == this.button_ManualDownloadStart)
            {
                // the user wants to send a "Download Start" CTU datagram
                Logger.Info("The user has sent a Download Start.");
                this.PerformDownloadProcessStart();
            }
            else if (buttonSender == this.button_ManualDownloadProgressRequest)
            {
                // the user wants to send a "Download Progress Request" CTU datagram
                Logger.Info("The user has sent a Download Progress Request.");
                this.PerformDownloadProgressRequest();
            }
            else if (buttonSender == this.button_ManualDownloadAbort)
            {
                // the user wants to send a "Download Abort" CTU datagram
                Logger.Info("The user has sent a Download Abort.");
                this.PerformDownloadProcessAbort();
            }
        }

        /// <summary>
        /// Starts a complete download process based on the ISM directory
        /// selected by the user.
        /// </summary>
        private void PerformDownloadProcessStart()
        {
            string dirPath = this.textBox_folderBrowse.Text;
            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
            {
                MessageBox.Show(
                    this,
                    @"Invalid directory.",
                    @"TopBoxSimulator",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            this.filesPathForCu5 = dirPath;
            this.GetFilesFromFolder(this.filesPathForCu5);
        }

        private void PerformDownloadProcessAbort()
        {
            var triplets = new List<Triplet> { new DownloadAbort() };
            this.SendDatagram(triplets.ToArray());
            this.transferStatus.DisplayDatagramSent(triplets);
            this.progressResponseStatus.Clear();
        }

        private void PerformDownloadProgressRequest()
        {
            var triplets = new List<Triplet> { new DownloadProgressRequest() };
            this.SendDatagram(triplets.ToArray());
            this.transferStatus.DisplayDatagramSent(triplets);
        }

        private void ButtonSendCountdownNumberClick(object sender, EventArgs e)
        {
            sbyte countdownNumber;
            if (!sbyte.TryParse(this.textBoxCountdownNumber.Text, out countdownNumber))
            {
                MessageBox.Show(
                    this,
                    @"Invalid countdown number",
                    @"TopBoxSimulator",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var triplets = new List<Triplet> { new CountdownNumber { Number = countdownNumber } };
            this.SendDatagram(triplets.ToArray());
        }

        private void ButtonSendSpecialInputStateClick(object sender, EventArgs e)
        {
            if (this.specialInputState)
            {
                this.buttonSendSpecialInputState.Text = @"Enabled";
                var triplets = new List<Triplet> { new SpecialInputInfo(true) };
                this.SendDatagram(triplets.ToArray());
            }
            else
            {
                this.buttonSendSpecialInputState.Text = @"Disabled";
                var triplets = new List<Triplet> { new SpecialInputInfo(false) };
                this.SendDatagram(triplets.ToArray());
            }

            this.specialInputState = !this.specialInputState;
        }

        private void ButtonClearLogClick(object sender, EventArgs e)
        {
            this.textBoxLog.Clear();
        }
    }
}
