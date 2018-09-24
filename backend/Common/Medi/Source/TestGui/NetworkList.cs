// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NetworkList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// The network list.
    /// </summary>
    public partial class NetworkList : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkList"/> class.
        /// </summary>
        public NetworkList()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is risen whenever the <see cref="SelectedAddress"/> changes.
        /// </summary>
        public event EventHandler SelectedAddressChanged;

        /// <summary>
        /// Gets or sets a value indicating whether to allow to select the local address.
        /// </summary>
        public bool AllowLocalAddress { get; set; }

        /// <summary>
        /// Gets the selected address.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MediAddress SelectedAddress
        {
            get
            {
                return this.lbxAddresses.SelectedItem as MediAddress;
            }
        }

        private void BtnRefreshClick(object sender, EventArgs e)
        {
            this.nudTimeout.Enabled = false;
            this.lbxAddresses.Enabled = false;
            this.lbxAddresses.Items.Clear();

            var pinger = new Pinger(MessageDispatcher.Instance);
            pinger.BeginBroadcastPing(
                TimeSpan.FromMilliseconds((int)this.nudTimeout.Value), this.PingFinished, pinger);
        }

        private void PingFinished(IAsyncResult ar)
        {
            var pinger = (Pinger)ar.AsyncState;
            this.Invoke(new EventHandler((s, e) =>
                {
                    var addresses = pinger.EndBroadcastPing(ar);
                    if (this.AllowLocalAddress)
                    {
                        lbxAddresses.Items.Add(MessageDispatcher.Instance.LocalAddress);
                    }

                    foreach (var address in addresses)
                    {
                        lbxAddresses.Items.Add(address);
                    }

                    this.nudTimeout.Enabled = true;
                    this.lbxAddresses.Enabled = true;
                }));
        }

        private void LbxAddressesSelectedIndexChanged(object sender, EventArgs e)
        {
            var handler = this.SelectedAddressChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
