// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Form1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.UdcpTestGui
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;

    /// <summary>
    /// The main form of the application.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        private const string AnnounceTitleFormat = "*** {0} ***";

        private const int MaxAnnounceCount = 50;

        private readonly UdcpServer server;

        private readonly string defaultTitle;

        private readonly LocalUdcpInfo localInfo = new LocalUdcpInfo();

        private int announceCounter;

        private DatagramType expectedResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.defaultTitle = this.Text;

            this.server = new UdcpServer();
            this.server.RequestReceived += this.ServerOnRequestReceived;
            this.server.ResponseReceived += this.ServerOnResponseReceived;

            this.InitializeLocalInfo();
            this.propertyGridLocal.SelectedObject = this.localInfo;

            this.server.Start();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closed"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.server.Stop();
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool invert);

        private void InitializeLocalInfo()
        {
            this.localInfo.UnitName = Environment.MachineName;
            this.localInfo.SoftwareVersion =
                FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).FileVersion;

            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                var props = iface.GetIPProperties();
                if (props == null)
                {
                    continue;
                }

                foreach (var address in props.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    this.localInfo.IpAddress = address.Address.ToString();
                    this.localInfo.NetworkMask = address.IPv4Mask.ToString();
                }

                foreach (var address in props.GatewayAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    this.localInfo.Gateway = address.Address.ToString();
                }

                break;
            }
        }

        private void SendRequest(UdcpRequest request)
        {
            this.expectedResponse = request.Header.Type;
            this.server.SendDatagram(request);
        }

        private void SendResponse(UdcpResponse response)
        {
            this.server.SendDatagram(response);
        }

        private void AddUnit(UdcpResponse datagram)
        {
            foreach (RemoteUdcpInfo item in this.listBoxDiscovery.Items)
            {
                if (item.UdcpAddress.Equals(datagram.Header.UnitAddress))
                {
                    // remove before readding
                    this.listBoxDiscovery.Items.Remove(item);
                    break;
                }
            }

            var info = new RemoteUdcpInfo(
                datagram.Header.UnitAddress,
                datagram.GetField<UnitNameField>(),
                datagram.GetField<SoftwareVersionField>(),
                datagram.GetField<IpAddressField>(),
                datagram.GetField<NetworkMaskField>(),
                datagram.GetField<GatewayField>());
            this.listBoxDiscovery.Items.Add(info);
        }

        private void SendInformation()
        {
            var response = new UdcpResponse(DatagramType.GetInformation, this.server.LocalAddress);
            response.Fields.Add(new UnitNameField(this.localInfo.UnitName));
            response.Fields.Add(new SoftwareVersionField(this.localInfo.SoftwareVersion));
            response.Fields.Add(new IpAddressField(IPAddress.Parse(this.localInfo.IpAddress)));
            response.Fields.Add(new NetworkMaskField(IPAddress.Parse(this.localInfo.NetworkMask)));
            response.Fields.Add(new GatewayField(IPAddress.Parse(this.localInfo.Gateway)));
            this.SendResponse(response);
        }

        private void SetConfiguration(UdcpRequest request)
        {
            var address = request.GetField<IpAddressField>();
            if (address != null)
            {
                this.localInfo.IpAddress = address.Value.ToString();
            }

            var networkMask = request.GetField<NetworkMaskField>();
            if (networkMask != null)
            {
                this.localInfo.NetworkMask = networkMask.Value.ToString();
            }

            var gateway = request.GetField<GatewayField>();
            if (gateway != null)
            {
                this.localInfo.Gateway = gateway.Value.ToString();
            }

            this.SendResponse(new UdcpResponse(DatagramType.SetConfiguration, this.server.LocalAddress));

            this.propertyGridLocal.SelectedObject = this.localInfo;
            this.tabControl1.SelectTab(this.tabPage2);
            MessageBox.Show(
                this,
                "Changed local configuration",
                "Set Configuration",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ServerOnRequestReceived(object sender, UdcpDatagramEventArgs<UdcpRequest> e)
        {
            if (!this.toolStripButtonEnabled.Checked)
            {
                // ignore all requests when we are disabled
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(
                    new EventHandler<UdcpDatagramEventArgs<UdcpRequest>>(this.ServerOnRequestReceived), sender, e);
                return;
            }

            switch (e.Datagram.Header.Type)
            {
                case DatagramType.GetInformation:
                    this.SendInformation();
                    return;
                case DatagramType.SetConfiguration:
                    this.SetConfiguration(e.Datagram);
                    return;
                case DatagramType.Announce:
                    this.announceCounter = 0;
                    this.announceTimer.Enabled = true;
                    FlashWindow(this.Handle, false);
                    this.SendResponse(new UdcpResponse(DatagramType.Announce, this.server.LocalAddress));
                    return;
                case DatagramType.Reboot:
                    MessageBox.Show(
                        this,
                        "A restart was requested by UDCP, not doing anything in this test application",
                        "Restart Requested",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    var response = new UdcpResponse(DatagramType.Reboot, this.server.LocalAddress);
                    response.Fields.Add(new ErrorCodeField(ErrorCode.CouldNotProcess));
                    response.Fields.Add(new ErrorMessageField("Reboot not supported in Test GUI"));
                    this.SendResponse(response);
                    return;
            }
        }

        private void ServerOnResponseReceived(object sender, UdcpDatagramEventArgs<UdcpResponse> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(
                    new EventHandler<UdcpDatagramEventArgs<UdcpResponse>>(this.ServerOnResponseReceived), sender, e);
                return;
            }

            if (e.Datagram.Header.Type != DatagramType.GetInformation
                && e.Datagram.Header.Type != this.expectedResponse)
            {
                // ignore responses that are not for us
                return;
            }

            // reset the expected response
            this.expectedResponse = DatagramType.GetInformation;

            switch (e.Datagram.Header.Type)
            {
                case DatagramType.GetInformation:
                    this.AddUnit(e.Datagram);
                    return;
                case DatagramType.SetConfiguration:
                    this.ShowMessage("Set Configuration", "Configuration was set successfully", e.Datagram);
                    return;
                case DatagramType.Announce:
                    MessageBox.Show(
                        this,
                        "Remote unit is showing announcement",
                        "Remote Announcement",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                case DatagramType.Reboot:
                    MessageBox.Show(
                        this,
                        "Remote unit is restarting",
                        "Remote Restart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
            }
        }

        private void ShowMessage(string caption, string successMessage, UdcpResponse response)
        {
            var error = response.GetField<ErrorCodeField>();
            if (error == null || error.ErrorCode == ErrorCode.Ok)
            {
                MessageBox.Show(
                    this,
                    successMessage,
                    caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var messageField = response.GetField<ErrorMessageField>();
            var message = messageField != null
                                 ? string.Format("{0}: {1}", error.ErrorCode, messageField.Value)
                                 : error.ErrorCode.ToString();
            MessageBox.Show(
                this,
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ToolStripButtonSerachClick(object sender, EventArgs e)
        {
            this.listBoxDiscovery.Items.Clear();
            this.SelectRemoteUnit(null);
            this.SendRequest(new UdcpRequest(DatagramType.GetInformation, UdcpAddress.BroadcastAddress));
        }

        private void SelectRemoteUnit(RemoteUdcpInfo info)
        {
            this.propertyGridDiscovery.SelectedObject = info;
            this.toolStripButtonAnnounce.Enabled = info != null;
            this.toolStripButtonConfigure.Enabled = info != null;
            this.toolStripButtonReboot.Enabled = info != null;
        }

        private void ListBoxDiscoverySelectedIndexChanged(object sender, EventArgs e)
        {
            var info = this.listBoxDiscovery.SelectedItem as RemoteUdcpInfo;
            this.SelectRemoteUnit(info);
        }

        private void ToolStripButtonAnnounceClick(object sender, EventArgs e)
        {
            var info = this.listBoxDiscovery.SelectedItem as RemoteUdcpInfo;
            if (info == null)
            {
                return;
            }

            this.SendRequest(new UdcpRequest(DatagramType.Announce, info.UdcpAddress));
        }

        private void ToolStripButtonConfigureClick(object sender, EventArgs e)
        {
            var info = this.listBoxDiscovery.SelectedItem as RemoteUdcpInfo;
            if (info == null)
            {
                return;
            }

            var request = new UdcpRequest(DatagramType.SetConfiguration, info.UdcpAddress);
            request.Fields.Add(new IpAddressField(IPAddress.Parse(info.IpAddress)));
            request.Fields.Add(new NetworkMaskField(IPAddress.Parse(info.NetworkMask)));
            request.Fields.Add(new GatewayField(IPAddress.Parse(info.Gateway)));
            this.SendRequest(request);
        }

        private void ToolStripButtonRebootClick(object sender, EventArgs e)
        {
            var info = this.listBoxDiscovery.SelectedItem as RemoteUdcpInfo;
            if (info == null)
            {
                return;
            }

            this.SendRequest(new UdcpRequest(DatagramType.Reboot, info.UdcpAddress));
        }

        private void ToolStripButtonEnabledClick(object sender, EventArgs e)
        {
            this.toolStripButtonEnabled.Checked = !this.toolStripButtonEnabled.Checked;
            var enabled = this.toolStripButtonEnabled.Checked;
            this.propertyGridLocal.Enabled = enabled;
        }

        private void AnnounceTimerTick(object sender, EventArgs e)
        {
            this.announceCounter++;
            if (this.announceCounter >= MaxAnnounceCount)
            {
                this.announceTimer.Enabled = false;
                this.Text = this.defaultTitle;
                return;
            }

            this.Text = (this.announceCounter % 2) == 1
                            ? string.Format(AnnounceTitleFormat, this.defaultTitle)
                            : this.defaultTitle;
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable MemberCanBePrivate.Local
        private class LocalUdcpInfo
        {
            public string UnitName { get; set; }

            public string SoftwareVersion { get; set; }

            public string IpAddress { get; set; }

            public string NetworkMask { get; set; }

            public string Gateway { get; set; }
        }

        private class RemoteUdcpInfo
        {
            public RemoteUdcpInfo(
                UdcpAddress udcpAddress,
                UnitNameField unitName,
                SoftwareVersionField softwareVersion,
                IpAddressField address,
                NetworkMaskField mask,
                GatewayField gateway)
            {
                this.UdcpAddress = udcpAddress;
                this.UnitName = GetString(unitName);
                this.SoftwareVersion = GetString(softwareVersion);
                this.IpAddress = GetString(address);
                this.NetworkMask = GetString(mask);
                this.Gateway = GetString(gateway);
            }

            public UdcpAddress UdcpAddress { get; private set; }

            public string UnitName { get; private set; }

            public string SoftwareVersion { get; private set; }

            public string IpAddress { get; set; }

            public string NetworkMask { get; set; }

            public string Gateway { get; set; }

            public override string ToString()
            {
                return this.UnitName;
            }

            private static string GetString(IpAddressFieldBase address)
            {
                return address == null ? "n/a" : address.Value.ToString();
            }

            private static string GetString(StringFieldBase stringField)
            {
                return stringField == null ? "n/a" : stringField.Value;
            }
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore MemberCanBePrivate.Local
    }
}
