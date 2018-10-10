// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatus.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransferStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.TopboxSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Requests;
    using Gorba.Common.Protocols.Ctu.Responses;

    /// <summary>
    /// Form to start and report the transfer status of files to CU5
    /// </summary>
    public partial class TransferStatus : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferStatus"/> class.
        /// </summary>
        public TransferStatus()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Dsiplay the datagram sent in the text box
        /// </summary>
        /// <param name="triplets">
        /// The triplets.
        /// </param>
        public void DisplayDatagramSent(List<Triplet> triplets)
        {
            foreach (var triplet in triplets)
            {
                var downloadStart = triplet as DownloadStart;
                if (downloadStart != null)
                {
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("Datagram {0} sent with:", triplet.Tag));
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("Filename: {0}", downloadStart.FileAbsPath));
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("Filesize: {0}", downloadStart.FileSize));
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("CRC of file: {0}", downloadStart.FileCrc));
                    continue;
                }

                var progressRequest = triplet as DownloadProgressRequest;
                if (progressRequest != null)
                {
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("Datagram {0} sent", triplet.Tag));
                    continue;
                }

                var downlaodAbort = triplet as DownloadAbort;
                if (downlaodAbort != null)
                {
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(Environment.NewLine);
                    this.textBox_datagramsSent.AppendText(string.Format("Datagram {0} sent", triplet.Tag));
                }
            }
        }

        /// <summary>
        /// Dsiplay the datagram received in the text box
        /// </summary>
        /// <param name="downloadProgress">
        /// The download Progress.
        /// </param>
        public void DisplayDatagramReceived(DownloadProgressResponse downloadProgress)
        {
            this.textBox_datagramsReceived.AppendText(Environment.NewLine);
            this.textBox_datagramsReceived.AppendText(Environment.NewLine);
            this.textBox_datagramsReceived.AppendText("Datagram Progress Response received with");
            this.textBox_datagramsReceived.AppendText(Environment.NewLine);
            this.textBox_datagramsReceived.AppendText(string.Format("Filename: {0}", downloadProgress.FileAbsName));
            this.textBox_datagramsReceived.AppendText(Environment.NewLine);
            this.textBox_datagramsReceived.AppendText(string.Format("Status Code: {0}", downloadProgress.StatusCode));
        }

        private void ButtonClearDatagramsSentClick(object sender, EventArgs e)
        {
            this.textBox_datagramsSent.Clear();
        }

        private void ButtonClearDatagramsReceivedClick(object sender, EventArgs e)
        {
            this.textBox_datagramsReceived.Clear();
        }
    }
}
