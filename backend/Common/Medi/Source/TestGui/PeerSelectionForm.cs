// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerSelectionForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PeerSelectionForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// The peer selection form.
    /// </summary>
    public partial class PeerSelectionForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeerSelectionForm"/> class.
        /// </summary>
        public PeerSelectionForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow to select the local address.
        /// </summary>
        public bool AllowLocalAddress
        {
            get
            {
                return this.networkList1.AllowLocalAddress;
            }

            set
            {
                this.networkList1.AllowLocalAddress = value;
            }
        }

        /// <summary>
        /// Gets the selected address.
        /// </summary>
        public MediAddress SelectedAddress
        {
            get
            {
                return this.mediAddressEditor1.Address;
            }
        }

        private void NetworkList1SelectedAddressChanged(object sender, EventArgs e)
        {
            this.mediAddressEditor1.Address = this.networkList1.SelectedAddress;
        }

        private void MediAddressEditor1AddressChanged(object sender, EventArgs e)
        {
            this.btnOk.Enabled = !string.IsNullOrEmpty(this.mediAddressEditor1.Address.Unit)
                                 && !string.IsNullOrEmpty(this.mediAddressEditor1.Address.Application);
        }
    }
}
