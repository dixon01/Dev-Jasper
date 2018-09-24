// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002RowControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO002Control type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    /// <summary>
    /// Control to create GO002 telegrams
    /// </summary>
    public partial class GO002RowControl : UserControl
    {
        private int byteSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO002RowControl"/> class.
        /// </summary>
        public GO002RowControl()
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
                return this.textBox_DataLength.Text;
            }

            set
            {
                this.textBox_DataLength.Text = value;
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
        /// Gets or sets the row number string.
        /// </summary>
        public string RowNumber
        {
            get
            {
                return this.textBox_RowNumber.Text;
            }

            set
            {
                this.textBox_RowNumber.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the pictogram string.
        /// </summary>
        public string Pictogram
        {
            get
            {
                return this.textBox_Pictogram.Text;
            }

            set
            {
                this.textBox_Pictogram.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the line number  string.
        /// </summary>
        public string LineNumber
        {
            get
            {
                return this.textBox_LineNumber.Text;
            }

            set
            {
                this.textBox_LineNumber.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the departure time  string.
        /// </summary>
        public string DepartureTime
        {
            get
            {
                return this.textBox_DepartureTime.Text;
            }

            set
            {
                this.textBox_DepartureTime.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the track number string.
        /// </summary>
        public string TrackNumber
        {
            get
            {
                return this.textBox_TrackNumber.Text;
            }

            set
            {
                this.textBox_TrackNumber.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation string.
        /// </summary>
        public string Deviation
        {
            get
            {
                return this.textBox_Deviation.Text;
            }

            set
            {
                this.textBox_Deviation.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination name string.
        /// </summary>
        public string DestinationName
        {
            get
            {
                return this.textBox_DestinationName.Text;
            }

            set
            {
                this.textBox_DestinationName.Text = value;
            }
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            var length = this.textBox_StopIndex.TextLength;
            length += this.textBox_RowNumber.TextLength;
            length += this.textBox_Pictogram.TextLength;
            length += this.textBox_LineNumber.TextLength;
            length += this.textBox_DepartureTime.TextLength;
            length += this.textBox_TrackNumber.TextLength;
            length += this.textBox_Deviation.TextLength;
            length += this.textBox_DestinationName.TextLength;

            // According to the details received by our P.O.
            // the length field doesn't care about the header, its lenght, the marker and the crc.
            // that's why here below are commented the following two lines.
            // length += this.textBox_DataLength.TextLength;
            // length += 2; // for the header
            length *= this.ByteSize;
            this.textBox_DataLength.Text = length.ToString("D3", CultureInfo.InvariantCulture);
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
