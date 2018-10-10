// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Control that contains all the lines of the DS021a telegram creation UI element.
    /// </summary>
    public partial class DS021AControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS021AControl"/> class.
        /// </summary>
        public DS021AControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired when the apply button is pressed.
        /// </summary>
        public event EventHandler ApplyClick;

        /// <summary>
        /// Event that is fired when the send button is pressed.
        /// </summary>
        public event EventHandler SendClick;

        /// <summary>
        /// Gets the selected row.
        /// </summary>
        public DS021ARowControl SelectedRow { get; private set; }

        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        public string Address
        {
            get
            {
                return this.textBoxIbisAddrDS021A.Text;
            }

            set
            {
                this.textBoxIbisAddrDS021A.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the block length.
        /// </summary>
        public string BlockLength
        {
            get
            {
                return this.textBoxBlockLengthDS021A.Text;
            }

            set
            {
                this.textBoxBlockLengthDS021A.Text = value;
            }
        }

        private void DS021AControlApplyClick(object sender, EventArgs e)
        {
            this.SelectedRow = (DS021ARowControl)sender;
            foreach (Control other in this.Controls)
            {
                if (other is DS021ARowControl)
                {
                    other.BackColor = (other == this.SelectedRow) ? Color.Gray : Color.Transparent;
                }
            }

            var handler = this.ApplyClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void DS021AControlSendClick(object sender, EventArgs e)
        {
            this.SelectedRow = (DS021ARowControl)sender;

            var handler = this.SendClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
