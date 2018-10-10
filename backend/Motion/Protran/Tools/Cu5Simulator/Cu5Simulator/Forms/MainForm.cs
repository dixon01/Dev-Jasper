// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ctu;
    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Main form for this application.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly Context stateMachineContext;
        private readonly CtuSerializer ctuSerializer;
        private readonly Timer timerCtuSender;
        private StateMachineUi stateMachineUi;

        private bool started;

        private UdpClient udpServer;

        private bool isFirstCtuArrived;

        private ushort seqNum;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.comboBoxStatus.Items.Add(StatusCode.Ok);
            this.comboBoxStatus.Items.Add(StatusCode.MissingData);
            this.comboBoxStatus.SelectedIndex = 0;

            this.InitDisplayStateComboBox(this.comboBoxDisplayStateFront);
            this.InitDisplayStateComboBox(this.comboBoxDisplayStateRear);
            this.InitDisplayStateComboBox(this.comboBoxDisplayStateLeft);
            this.InitDisplayStateComboBox(this.comboBoxDisplayStateRight);

            this.ctuSerializer = new CtuSerializer();
            this.timerCtuSender = new Timer((int)this.numericUpDown_period.Value);
            this.timerCtuSender.Elapsed += this.TimerSendCtuElapsed;

            this.stateMachineContext = new Context();
            this.stateMachineContext.TripletsProduced += this.StateMachineTripletsProduced;
        }

        private void InitDisplayStateComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            comboBox.Items.Add(DisplayStatusCode.Ok);
            comboBox.Items.Add(DisplayStatusCode.Initializing);
            comboBox.Items.Add(DisplayStatusCode.NoConnection);
            comboBox.Items.Add(DisplayStatusCode.DisplayError);
            comboBox.Items.Add(DisplayStatusCode.BacklightError);

            comboBox.SelectedIndex = 0;
        }

        private void ButtonStartStopClick(object sender, EventArgs e)
        {
            if (this.started)
            {
                // the user wants to stop.
                this.Stop();
            }
            else
            {
                // the user wants to start.
                this.Start();
            }

            if (this.started)
            {
                this.button_startStop.Text = "Stop";
                this.label_listening.Text = "(listening)";
                this.numericUpDown_period.Enabled = false;
            }
            else
            {
                this.button_startStop.Text = "Start";
                this.label_listening.Text = "(not listening)";
                this.numericUpDown_period.Enabled = true;
            }
        }

        private void ButtonClearTextAreaClick(object sender, EventArgs e)
        {
            // I clear all the old information.
            this.textBox_receivedText.Clear();
            this.label_lineNumber.Text = "Line number: xxx";
            this.label_destination.Text = "Destination: xxx";
            this.label_destArab.Text = "Dest. arab: xxx";

            this.labelDestinationNo.Text = "Destination No: xxx";
            this.labelCurrentDirectionNo.Text = "Current Direction No: xxx";
            this.labelLineNumber.Text = "Line number: xxx";

            this.labelExi_DestinationNo.Text = "Destination No: xxx";
            this.labelExi_CurrentDirectionNo.Text = "Current Direction No: xxx";
            this.labelExi_LineNo.Text = "Line number: xxx";
            this.labelExi_Destination.Text = "Destination: xxx";
            this.labelExi_DestinationArabic.Text = "Dest. arab: xxx";

            this.labelSpecialInputState.Text = "SpecialInput state:";
        }

        private void ButtonClearInternalMsgClick(object sender, EventArgs e)
        {
            this.textBox_internalMessages.Clear();
        }

        private void Stop()
        {
            this.started = false;
            this.udpServer.Close();
            this.timerCtuSender.Stop();
            this.isFirstCtuArrived = false;
        }

        private void Start()
        {
            this.started = true;
            var port = (int)this.numericUpDown_port.Value;
            var endPoint = new IPEndPoint(IPAddress.Any, port);

            this.udpServer = new UdpClient(port);
            var state = new UdpState(this.udpServer, endPoint);
            this.udpServer.BeginReceive(this.ReceiveCallback, state);
            this.timerCtuSender.Interval = (int)this.numericUpDown_period.Value;
        }

        /// <summary>
        /// Function called whenever a UDP datagram is received.
        /// </summary>
        /// <param name="ar">
        /// The async result.
        /// </param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!this.started)
            {
                // UDP not started.
                return;
            }

            var server = ((UdpState)ar.AsyncState).UdpServer;
            var endPoint = ((UdpState)ar.AsyncState).EndPoint;

            byte[] receivedBytes;
            try
            {
                receivedBytes = server.EndReceive(ar, ref endPoint);
            }
            catch (Exception e)
            {
                this.Invoke(
                    (MethodInvoker)(() => this.textBox_internalMessages.AppendText(e.Message + Environment.NewLine)));
                return;
            }

            if (!this.isFirstCtuArrived)
            {
                // now I've to connect myself to the remote end point
                // (because from now my communication are based on it)
                // and it's the time to start a timer that
                // sends periodically the status CTU datagram.
                this.udpServer.Connect(endPoint);
                this.isFirstCtuArrived = true;
                this.timerCtuSender.Start();
                this.TimerSendCtuElapsed(this, EventArgs.Empty);
                this.stateMachineContext.FtpServerIP = endPoint.Address.ToString();
            }

            this.ManageReceivedBuffer(receivedBytes);

            // let's re-enable the read for the next CTU datagram...
            server.BeginReceive(this.ReceiveCallback, new UdpState(server, endPoint));
        }

        private void ManageReceivedBuffer(byte[] receivedBytes)
        {
            string lineNumber = string.Empty;
            string destination = string.Empty;
            string destArab = string.Empty;
            CtuDatagram datagram = this.ctuSerializer.Deserialize(receivedBytes);
            var descrBuilder = new StringBuilder();
            descrBuilder.AppendLine();
            descrBuilder.AppendLine("Version: " + datagram.Header.VersionNumber);
            descrBuilder.AppendLine("Flags: " + datagram.Header.Flags);
            descrBuilder.AppendLine("Seq. Num: " + datagram.Header.SequenceNumber);

            int tripletCounter = 0;
            bool receivedTripInfo = false;
            foreach (var triplet in datagram.Payload.Triplets)
            {
                descrBuilder.AppendLine(string.Format("Triplet {0}", tripletCounter++));
                descrBuilder.AppendLine("\t Tag: " + triplet.Tag);
                descrBuilder.AppendLine("\t Length: " + triplet.Length);
                descrBuilder.AppendLine("\t Value: " + triplet);
                var tripInfo = triplet as TripInfo;
                if (tripInfo != null)
                {
                    // trip info.
                    receivedTripInfo = true;
                    lineNumber = tripInfo.LineNumber;
                    destination = tripInfo.Destination;
                    destArab = tripInfo.DestinationArabic;
                    continue;
                }

                this.ManageReceivedTriplet(triplet);
            }

            string description = descrBuilder.ToString();
            this.Invoke(
                (MethodInvoker)delegate
                    {
                        if (this.textBox_receivedText.Text.Length + description.Length
                            >= this.textBox_receivedText.MaxLength)
                        {
                            this.textBox_receivedText.Clear();
                        }

                        description += Environment.NewLine;
                        this.textBox_receivedText.AppendText(description);

                        this.textBox_receivedText.SelectionStart = this.textBox_receivedText.TextLength;
                        this.textBox_receivedText.ScrollToCaret();
                        this.textBox_receivedText.Update();
                        if (receivedTripInfo)
                        {
                            // I update the labels with the trip infos.
                            this.label_lineNumber.Text = "Line number: " + lineNumber;
                            this.label_destination.Text = "Destination: " + destination;
                            this.label_destArab.Text = "Dest. arab: " + destArab;
                        }
                    });
        }

        private void ManageReceivedTriplet(Triplet triplet)
        {
            var lineInfo = triplet as LineInfo;
            if (lineInfo != null)
            {
                // line info.
                this.DisplayLineInfo(lineInfo);
                return;
            }

            var extendedLineInfo = triplet as ExtendedLineInfo;
            if (extendedLineInfo != null)
            {
                // Extended line info.
                this.DisplayExtendedLineInfo(extendedLineInfo);
                return;
            }

            var countdownNumber = triplet as CountdownNumber;
            if (countdownNumber != null)
            {
                // Countdown number.
                this.DisplayCountdownNumber(countdownNumber);
                return;
            }

            var specialInputState = triplet as SpecialInputInfo;
            if (specialInputState != null)
            {
                // SpecialInputInfo
                this.DisplaySpecialInputState(specialInputState);
                return;
            }

            switch (triplet.Tag)
            {
                case TagName.DeviceInfoRequest:
                    this.SendDatagram(
                        new DeviceInfoResponse
                            {
                                SerialNumber = 123456,
                                SoftwareVersion = this.GetType().Assembly.GetName().Version.ToString(4),
                                DataVersion = "CU5 Simulator"
                            });
                    break;
                case TagName.DownloadProgressRequest:
                case TagName.DownloadAbort:
                case TagName.DownloadStart:
                    this.UpdateStateMachine(triplet);
                    break;
            }
        }

        private void DisplayExtendedLineInfo(ExtendedLineInfo triplet)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.DisplayExtendedLineInfo(triplet)));
                return;
            }

            this.labelExi_DestinationNo.Text = "Destination No: " + triplet.DestinationNo;
            this.labelExi_CurrentDirectionNo.Text = "Current Direction No: " + triplet.CurrentDirectionNo;
            this.labelExi_LineNo.Text = "Line number: " + triplet.LineNumber;
            this.labelExi_Destination.Text = "Destination: " + triplet.Destination;
            this.labelExi_DestinationArabic.Text = "Dest. arab: " + triplet.DestinationArabic;
        }

        private void DisplayCountdownNumber(CountdownNumber triplet)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.DisplayCountdownNumber(triplet)));
                return;
            }

            this.labelCountdownNumber.Text = "Countdown number: " + triplet.Number;
        }

        private void DisplaySpecialInputState(SpecialInputInfo triplet)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.DisplaySpecialInputState(triplet)));
                return;
            }

            this.labelSpecialInputState.Text = "SpecialInput state: " + triplet.SpecialInputState;
        }

        private void DisplayLineInfo(LineInfo triplet)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.DisplayLineInfo(triplet)));
                return;
            }

            this.labelDestinationNo.Text = "Destination No: " + triplet.DestinationNo;
            this.labelCurrentDirectionNo.Text = "Current Direction No: " + triplet.CurrentDirectionNo;
            this.labelLineNumber.Text = "Line number: " + triplet.LineNumber;
        }

        private void UpdateStateMachine(Triplet triplet)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.UpdateStateMachine(triplet)));
                return;
            }

            if (this.stateMachineUi == null)
            {
                this.stateMachineUi = new StateMachineUi(this.stateMachineContext);
            }

            this.stateMachineUi.Visible = true;
            this.stateMachineContext.Trigger(triplet);
        }

        private void TimerSendCtuElapsed(object sender, EventArgs e)
        {
            var status = (StatusCode)this.comboBoxStatus.SelectedItem;
            var displays = new[]
                               {
                                   new DisplayStatus.Item(
                                       1, (DisplayStatusCode)this.comboBoxDisplayStateFront.SelectedItem),
                                   new DisplayStatus.Item(
                                       2, (DisplayStatusCode)this.comboBoxDisplayStateLeft.SelectedItem),
                                   new DisplayStatus.Item(
                                       3, (DisplayStatusCode)this.comboBoxDisplayStateRight.SelectedItem),
                                   new DisplayStatus.Item(
                                       4, (DisplayStatusCode)this.comboBoxDisplayStateRear.SelectedItem)
                               };
            this.SendDatagram(new Status(status, "CU5 Sim. Status: " + status), new DisplayStatus(displays));
        }

        private void StateMachineTripletsProduced(object sender, TripletsProducedEventArgs e)
        {
            Triplet[] triplets = e.Triplets.ToArray();
            this.SendDatagram(triplets);
        }

        private void SendDatagram(params Triplet[] triplets)
        {
            lock (this.udpServer)
            {
                if (!this.started)
                {
                    // server not started.
                    return;
                }

                var header = new Header { SequenceNumber = this.seqNum++ };

                var payload = new Payload { Triplets = new List<Triplet>(triplets) };

                var datagram = new CtuDatagram(header, payload);
                byte[] buffer = this.ctuSerializer.Serialize(datagram);

                try
                {
                    this.udpServer.Send(buffer, buffer.Length);
                }
                catch (Exception exc)
                {
                    this.Invoke(
                        (MethodInvoker)delegate
                            {
                                this.textBox_internalMessages.AppendText(Environment.NewLine);
                                this.textBox_internalMessages.AppendText(exc.Message);
                            });
                }
            }
        }

        private void DownloadStateMachineToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.stateMachineUi != null)
            {
                this.stateMachineUi.Close();
                this.stateMachineUi.Dispose();
            }

            this.stateMachineUi = new StateMachineUi(this.stateMachineContext);
            this.stateMachineUi.Show();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Container of the couple of information
        /// made by a <see cref="UdpClient"/> object and an IPEndPoint object.
        /// </summary>
        private class UdpState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UdpState"/> class.
            /// </summary>
            /// <param name="udpServer">
            /// The UDP server.
            /// </param>
            /// <param name="endPoint">
            /// The end point.
            /// </param>
            public UdpState(UdpClient udpServer, IPEndPoint endPoint)
            {
                this.UdpServer = udpServer;
                this.EndPoint = endPoint;
            }

            /// <summary>
            /// Gets the UDP server instance.
            /// </summary>
            public UdpClient UdpServer { get; private set; }

            /// <summary>
            /// Gets the IP end point.
            /// </summary>
            public IPEndPoint EndPoint { get; private set; }
        }
    }
}
