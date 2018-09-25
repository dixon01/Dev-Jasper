// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005RowControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO005Control type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Control to create GO005 telegrams
    /// </summary>
    public partial class GO005RowControl : UserControl
    {
        private int byteSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO005RowControl"/> class.
        /// </summary>
        public GO005RowControl()
        {
            this.InitializeComponent();
            this.byteSize = 1;
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
        /// Gets or sets the byte size used for the fields.
        /// Default value is 1 (8 or 7 bit)
        /// </summary>
        public int ByteSize
        {
            get
            {
                return this.byteSize;
            }

            set
            {
                this.byteSize = value;
                this.TextBoxTextChanged(null, null);
            }
        }

        /// <summary>
        /// Gets or sets the data length string.
        /// </summary>
        public string DataLength
        {
            get
            {
                return this.textBoxDataLength.Text;
            }

            set
            {
                this.textBoxDataLength.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the line number string.
        /// </summary>
        public string LineNumber
        {
            get
            {
                return this.textBoxLineNumber.Text;
            }

            set
            {
                this.textBoxLineNumber.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop index string.
        /// </summary>
        public string StopIndex
        {
            get
            {
                return this.textBoxStopIndex.Text;
            }

            set
            {
                this.textBoxStopIndex.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop name string.
        /// </summary>
        public string Destination
        {
            get
            {
                return this.textBoxDestination.Text;
            }

            set
            {
                this.textBoxDestination.Text = value;
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

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            // according to the documentation, the length is done
            // with the header plus all the other fields except the trailer (marker and crc)
            var length = 1; // <03>
            length += this.textBoxLineNumber.TextLength;
            length += this.textBoxStopIndex.TextLength;
            if (this.textBoxDestination.TextLength > 0)
            {
                length++; // <04>
                length += this.textBoxDestination.TextLength;
            }

            length *= this.ByteSize;
            if (length == 1)
            {
                this.textBoxDataLength.Text = string.Empty;
            }
            else
            {
                length += 3; // round up
                this.textBoxDataLength.Text = string.Format("{0}{1}", length / 16, (length % 16) / 4);
            }
        }
    }
}
