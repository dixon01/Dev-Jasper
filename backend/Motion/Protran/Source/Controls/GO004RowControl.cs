// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004RowControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004Control type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Control to create GO004 (message) telegrams
    /// </summary>
    public partial class GO004RowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO004RowControl"/> class.
        /// </summary>
        public GO004RowControl()
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
        /// Gets or sets the message index string.
        /// </summary>
        public string MessageIndex
        {
            get
            {
                return this.textBoxIndex.Text;
            }

            set
            {
                this.textBoxIndex.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the message type string.
        /// </summary>
        public string MessageType
        {
            get
            {
                return this.textBoxType.Text;
            }

            set
            {
                this.textBoxType.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the time range start string.
        /// </summary>
        public string TimeRangeStart
        {
            get
            {
                return this.textBoxStartTime.Text;
            }

            set
            {
                this.textBoxStartTime.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the time range end string.
        /// </summary>
        public string TimeRangeEnd
        {
            get
            {
                return this.textBoxEndTime.Text;
            }

            set
            {
                this.textBoxEndTime.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the message text string.
        /// </summary>
        public string MessageText
        {
            get
            {
                return this.textBoxMessage.Text.Replace("<10>", "\x10");
            }

            set
            {
                this.textBoxMessage.Text = value.Replace("\x10", "<10>");
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
