// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortForwardingView.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortForwardingView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// The port forwarding view.
    /// </summary>
    public partial class PortForwardingView : UserControl
    {
        private readonly IPortForwardingService service;

        private MediAddress clientPeer;

        private MediAddress serverPeer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortForwardingView"/> class.
        /// </summary>
        public PortForwardingView()
        {
            this.InitializeComponent();

            this.service = MessageDispatcher.Instance.GetService<IPortForwardingService>();
        }

        private void ListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonRemove.Enabled = this.listBox.SelectedItem != null;
        }

        private MediAddress SelectPeer()
        {
            var selector = new PeerSelectionForm();
            selector.AllowLocalAddress = true;
            if (selector.ShowDialog(this) != DialogResult.OK)
            {
                return null;
            }

            return selector.SelectedAddress;
        }

        private void Verify()
        {
            this.buttonAdd.Enabled = this.clientPeer != null && this.serverPeer != null;
        }

        private void ButtonBrowseClientClick(object sender, EventArgs e)
        {
            this.clientPeer = this.SelectPeer();
            this.textBoxClientPeer.Text = Convert.ToString(this.clientPeer);
            this.Verify();
        }

        private void ButtonBrowseServerClick(object sender, EventArgs e)
        {
            this.serverPeer = this.SelectPeer();
            this.textBoxServerPeer.Text = Convert.ToString(this.serverPeer);
            this.Verify();
        }

        private void ButtonAddClick(object sender, EventArgs e)
        {
            var serverConfig = new TcpServerEndPointConfig { LocalPort = (int)this.numericUpDownServerPort.Value };
            var clientConfig = new TcpClientEndPointConfig
                                   {
                                       RemoteAddress = this.textBoxClientAddress.Text,
                                       RemotePort = (int)this.numericUpDownClientPort.Value
                                   };

            var info = new PortForwardingInfo(this.clientPeer, clientConfig, this.serverPeer, serverConfig);
            this.listBox.Items.Add(info);
            this.service.BeginCreateForwarding(
                this.clientPeer,
                clientConfig,
                this.serverPeer,
                serverConfig,
                this.ForwardingCreated,
                info);
        }

        private void ForwardingCreated(IAsyncResult ar)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.ForwardingCreated(ar)));
                return;
            }

            var info = (PortForwardingInfo)ar.AsyncState;
            try
            {
                info.Forwarding = this.service.EndCreateForwarding(ar);
                MessageBox.Show(this, info.ToString(), "Created", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                Exception exception;
                for (exception = ex; exception.InnerException != null; exception = exception.InnerException)
                {
                }

                MessageBox.Show(this, exception.Message, exception.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.listBox.Items.Remove(info);
            }
        }

        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            var info = this.listBox.SelectedItem as PortForwardingInfo;
            if (info == null || info.Forwarding == null)
            {
                return;
            }

            info.Forwarding.Dispose();
            this.listBox.Items.Remove(info);
        }

        private class PortForwardingInfo
        {
            private readonly MediAddress clientPeer;

            private readonly TcpClientEndPointConfig clientConfig;

            private readonly MediAddress serverPeer;

            private readonly TcpServerEndPointConfig serverConfig;

            public PortForwardingInfo(
                MediAddress clientPeer,
                TcpClientEndPointConfig clientConfig,
                MediAddress serverPeer,
                TcpServerEndPointConfig serverConfig)
            {
                this.clientPeer = clientPeer;
                this.clientConfig = clientConfig;
                this.serverPeer = serverPeer;
                this.serverConfig = serverConfig;
            }

            public IPortForwarding Forwarding { get; set; }

            public override string ToString()
            {
                return string.Format(
                    "{0} @ {1} --> {2} --> {3}:{4}",
                    this.serverPeer,
                    this.serverConfig.LocalPort,
                    this.clientPeer,
                    this.clientConfig.RemoteAddress,
                    this.clientConfig.RemotePort);
            }
        }
    }
}
