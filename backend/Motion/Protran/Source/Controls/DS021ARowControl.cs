﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021ARowControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// Control to create DS021a telegram payload
    /// </summary>
    [DefaultEvent("ApplyClick")]
    public partial class DS021ARowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS021ARowControl"/> class.
        /// </summary>
        public DS021ARowControl()
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

        /// <summary>
        /// Gets or sets the travel times.
        /// </summary>
        public string TravelTimes
        {
            get
            {
                return this.textBox_TravelTimes.Text;
            }

            set
            {
                this.textBox_TravelTimes.Text = value;
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
