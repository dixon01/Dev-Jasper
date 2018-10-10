// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CRowControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Control to create DS021c telegram payload
    /// </summary>
    public partial class DS021CRowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS021CRowControl"/> class.
        /// </summary>
        public DS021CRowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired whenever the "Apply" button on this control is clicked.
        /// </summary>
        public event EventHandler ApplyClick;

        /// <summary>
        /// Event that is fired whenever the "Send" button on this control is clicked.
        /// </summary>
        public event EventHandler SendClick;

        /// <summary>
        /// Gets or sets the status string.
        /// </summary>
        public string Status
        {
            get
            {
                return this.textBoxStatus.Text;
            }

            set
            {
                this.textBoxStatus.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop index string.
        /// </summary>
        public string StopIndex
        {
            get
            {
                return this.textBox_StopIndex.Text;
            }

            set
            {
                this.textBox_StopIndex.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop name.
        /// </summary>
        public string StopName
        {
            get
            {
                return this.textBox_StopName.Text;
            }

            set
            {
                this.textBox_StopName.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the transfer information string.
        /// </summary>
        public string Transfers
        {
            get
            {
                return this.textBox_Transfers.Text;
            }

            set
            {
                this.textBox_Transfers.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the transfer symbols string.
        /// </summary>
        public string TransferSymbols
        {
            get
            {
                return this.textBox_TransferSymbols.Text;
            }

            set
            {
                this.textBox_TransferSymbols.Text = value;
            }
        }

        private void ButtonApplyClick(object sender, EventArgs e)
        {
            var handler = this.ApplyClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ButtonSendClick(object sender, EventArgs e)
        {
            var handler = this.SendClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
