// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineUI.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateMachineUI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Requests;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The GUI for the download process state machine.
    /// </summary>
    public partial class StateMachineUi : Form
    {
        private readonly Context stateMachine;
        private readonly Label[] labelsFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineUi"/> class.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine.
        /// </param>
        public StateMachineUi(Context stateMachine)
        {
            this.InitializeComponent();

            this.labelsFileName = new[]
            {
                this.label_file0_name, this.label_file1_name, this.label_file2_name,
                this.label_file3_name, this.label_file4_name
            };

            this.stateMachine = stateMachine;
            this.stateMachine.TripletsProduced += this.StateMachineTripletsProduced;
            this.stateMachine.LogMessageProduced += this.StateMachineLogMessageProduced;
            this.stateMachine.DownloadFileNotificationProduced += this.StateMachineDownloadFileNotificationProduced;
            this.CleanDownloadProgressPanel();
        }

        private void ButtonAddFileToDownloadClick(object sender, EventArgs e)
        {
            string newFile = this.comboBox_downloadStart.Text;
            int indexOldFileName = this.comboBox_downloadStart.Items.IndexOf(newFile);
            if (indexOldFileName != -1)
            {
                MessageBox.Show(this, "The file already exists.", "State Machine GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint size = 0;
            int crc = -1;
            string fileAbsName;
            
            bool ok = this.IsFileLineOk(newFile, out size, out crc, out fileAbsName);
            if (!ok)
            {
                return;
            }

            this.comboBox_downloadStart.Items.Add(newFile);
        }

        private void ButtonClearAllFilesClick(object sender, EventArgs e)
        {
            this.comboBox_downloadStart.Items.Clear();
        }

        private void SendCtuDatagramClick(object sender, System.EventArgs e)
        {
            var buttonSender = sender as Button;
            if (buttonSender == null)
            {
                // invlaid number
                return;
            }

            if (buttonSender == this.button_sendDownloadAbort)
            {
                var abortTriplet = new DownloadAbort();
                this.stateMachine.Trigger(abortTriplet);
            }
            else if (buttonSender == this.button_downloadProgressRequest)
            {
                var requestTriplet = new DownloadProgressRequest();
                this.stateMachine.Trigger(requestTriplet);
            }
            else if (buttonSender == this.button_downloadStart)
            {
                if (this.comboBox_downloadStart.Items.Count == 0)
                {
                    MessageBox.Show(this, string.Format("No files to send."), "State Machine GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var triplets = new List<Triplet>();
                foreach (var item in this.comboBox_downloadStart.Items)
                {
                    var line = item as string;
                    uint size = 0;
                    int crc = -1;
                    string fileAbsName;
                    bool ok = this.IsFileLineOk(line, out size, out crc, out fileAbsName);
                    if (!ok)
                    {
                        return;
                    }

                    var file = new DownloadStart { FileSize = size, FileCrc = crc, FileAbsPath = fileAbsName };
                    triplets.Add(file);
                }

                ErrorContainer.Instance.CleanError();
                this.CleanDownloadProgressPanel();
                foreach (var triplet in triplets)
                {
                    this.stateMachine.Trigger(triplet);
                }                
            }
        }

        private void ButtonSendErrorCodeClick(object sender, EventArgs e)
        {
            var index = (int)this.numericUpDown_fileIndexErrorCode.Value;
            var errorCode = (DownloadStatusCode)Enum.Parse(typeof(DownloadStatusCode), this.comboBox_errorCode.SelectedItem as string);
            string line = this.comboBox_downloadStart.Items[index] as string;
            string fileAbsName;
            uint size;
            int crc;
            bool ok = this.IsFileLineOk(line, out size, out crc, out fileAbsName);
            if (!ok)
            {
                return;
            }
            
            ErrorContainer.Instance.FileIndex = index;
            ErrorContainer.Instance.ErrorCode = errorCode;
            ErrorContainer.Instance.FileAbsName = fileAbsName;
        }

        private bool IsFileLineOk(string line, out uint size, out int crc, out string fileName)
        {
            size = 0;
            crc = 0;
            fileName = string.Empty;
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            string[] fileTokens = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (fileTokens.Length != 3)
            {
                MessageBox.Show(this, string.Format("Invalid string: {0}", line), "State Machine GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool ok = uint.TryParse(fileTokens[0], out size);
            if (!ok)
            {
                MessageBox.Show(this, string.Format("Invalid file size: {0}", line), "State Machine GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ok = int.TryParse(fileTokens[1].Substring(2), NumberStyles.HexNumber, null, out crc);
            if (!ok)
            {
                MessageBox.Show(this, string.Format("Invalid CRC: {0}", line), "State Machine GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            fileName = fileTokens[2];
            return true;
        }

        private void CleanDownloadProgressPanel()
        {
            this.label_file0_name.Text =
            this.label_file1_name.Text =
            this.label_file2_name.Text =
            this.label_file3_name.Text =
            this.label_file4_name.Text = string.Empty;
        }

        private Label GetLabelByFileName(string fileAbsName)
        {
            foreach (var label in this.labelsFileName)
            {
                if (label.Text.Contains(fileAbsName))
                {
                    // I return the label referring to the same file
                    // in order that it will be updated.
                    return label;
                }
            }

            // else I return the first label that is empty
            // in order that they will be initialized.
            foreach (var label in this.labelsFileName)
            {
                if (string.IsNullOrEmpty(label.Text))
                {
                    return label;
                }
            }

            // else I return nothing.
            return null;
        }

        private void StateMachineLogMessageProduced(object sender, LogMessageEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
                {
                    this.textBox_lastState.Text = e.PreviousStateName;
                    this.textBox_currentState.Text = e.CurrentStateName;
                    var nexts = new StringBuilder();
                    if (e.AvailableNextStates != null)
                    {
                        foreach (var next in e.AvailableNextStates)
                        {
                            nexts.AppendLine(next);
                        }
                    }

                    this.textBox_nextStates.Text = nexts.ToString();
                    this.textBox_messages.AppendText(Environment.NewLine);
                    this.textBox_messages.AppendText(e.Message);
                    this.textBox_messages.SelectionStart = this.textBox_messages.TextLength;
                    this.textBox_messages.ScrollToCaret();
                    this.textBox_messages.Update();
                });
        }

        private void StateMachineDownloadFileNotificationProduced(object sender, DownloadFileNotificationEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                Label labelsToUpdate = this.GetLabelByFileName(e.FileAbsName);
                if (labelsToUpdate != null)
                {
                    labelsToUpdate.Text = string.Format(
                        "{0}   {1}%   {2}",
                        e.FileAbsName,
                        e.Progress.ToString(CultureInfo.InvariantCulture),
                        e.Status.ToString());
                }
            });
        }

        private void StateMachineTripletsProduced(object sender, TripletsProducedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (var triplet in e.Triplets)
                {
                    var response = triplet as DownloadProgressResponse;
                    if (response != null)
                    {
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText("Download Progress Response:");
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText(response.ToString());
                        this.textBox_tripletsSent.SelectionStart = this.textBox_tripletsSent.TextLength;
                        this.textBox_tripletsSent.ScrollToCaret();
                        this.textBox_tripletsSent.Update();
                    }

                    var log = triplet as LogMessage;
                    if (log != null)
                    {
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText("Log Message:");
                        this.textBox_tripletsSent.AppendText(Environment.NewLine);
                        this.textBox_tripletsSent.AppendText(log.ToString());
                    }
                }
            });
        }

        private void ButtonClearFilesClick(object sender, EventArgs e)
        {
            foreach (var label in this.labelsFileName)
            {
                label.Text = string.Empty;
            }
        }

        private void ButtonClearTripletsSentClick(object sender, EventArgs e)
        {
            this.textBox_tripletsSent.Clear();
        }

        private void ButtonCalculateCrcClick(object sender, EventArgs e)
        {
            var file = new FileInfo(this.textBox_crc.Text);
            if (!File.Exists(file.FullName))
            {
                MessageBox.Show(this, "Invalid file.");
                return;
            }

            var crc32 = new Crc32();
            int calculatedCrc;
            string hash = string.Empty;

            using (FileStream fs = File.Open(file.FullName, FileMode.Open))
            {
                foreach (byte b in crc32.ComputeHash(fs))
                {
                    hash += b.ToString("x2").ToLower();
                }

                calculatedCrc = int.Parse(hash, NumberStyles.HexNumber);
            }

            this.label_crcValue.Text = calculatedCrc.ToString("X2");
        }
    }
}
