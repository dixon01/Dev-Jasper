// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediAddressEditor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediAddressEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// An editor for a Medi address.
    /// </summary>
    public partial class MediAddressEditor : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediAddressEditor"/> class.
        /// </summary>
        public MediAddressEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is risen whenever the <see cref="Address"/> changes.
        /// </summary>
        public event EventHandler AddressChanged;

        /// <summary>
        /// Gets or sets the address in this editor.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MediAddress Address
        {
            get
            {
                return new MediAddress(this.textBoxUnitName.Text, this.textBoxAppName.Text);
            }

            set
            {
                if (value == null)
                {
                    this.textBoxUnitName.Clear();
                    this.textBoxAppName.Clear();
                    return;
                }

                this.textBoxUnitName.Text = value.Unit;
                this.textBoxAppName.Text = value.Application;
            }
        }

        private void RaiseAddressChanged()
        {
            var handler = this.AddressChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            this.textBoxUnitName.Enabled = this.Enabled;
            this.textBoxAppName.Enabled = this.Enabled;
        }

        private void TextBoxOnTextChanged(object sender, EventArgs e)
        {
            this.RaiseAddressChanged();
        }
    }
}
