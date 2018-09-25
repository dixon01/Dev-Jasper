// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingView.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggingView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Logging;

    using NLog;

    /// <summary>
    /// The logging view.
    /// </summary>
    public partial class LoggingView : UserControl
    {
        private int lastSelectedIndex;

        private ILogObserver currentObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingView"/> class.
        /// </summary>
        public LoggingView()
        {
            this.InitializeComponent();

            this.comboBoxLogLevel.Items.Add(LogLevel.Fatal);
            this.comboBoxLogLevel.Items.Add(LogLevel.Error);
            this.comboBoxLogLevel.Items.Add(LogLevel.Warn);
            this.comboBoxLogLevel.Items.Add(LogLevel.Info);
            this.comboBoxLogLevel.Items.Add(LogLevel.Debug);
            this.comboBoxLogLevel.Items.Add(LogLevel.Trace);

            this.comboBoxLogLevel.SelectedItem = LogLevel.Info;

            this.cbxSource.SelectedIndex = 0;
        }

        private void CbxSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedString = this.cbxSource.SelectedItem as string;
            if (selectedString != null)
            {
                if (this.cbxSource.SelectedIndex == 0)
                {
                    this.SetLogObserver(null);
                    this.lastSelectedIndex = this.cbxSource.SelectedIndex;
                    return;
                }

                if (this.cbxSource.SelectedIndex == 1)
                {
                    // use local
                    this.SetLogObserver(MessageDispatcher.Instance.LogObserverFactory.LocalObserver);
                    this.lastSelectedIndex = this.cbxSource.SelectedIndex;
                    return;
                }

                // Browse...
                var peerSelection = new PeerSelectionForm();
                peerSelection.Text = "Browse...";
                if (peerSelection.ShowDialog(this) == DialogResult.OK && peerSelection.SelectedAddress != null)
                {
                    int index = this.cbxSource.Items.IndexOf(peerSelection.SelectedAddress);
                    if (index >= 0)
                    {
                        this.cbxSource.SelectedIndex = index;
                    }
                    else
                    {
                        this.cbxSource.Items.Insert(this.cbxSource.Items.Count - 1, peerSelection.SelectedAddress);
                        this.cbxSource.SelectedItem = peerSelection.SelectedAddress;
                    }
                }
                else
                {
                    this.cbxSource.SelectedIndex = this.lastSelectedIndex;
                }

                return;
            }

            var selectedAddress = this.cbxSource.SelectedItem as MediAddress;
            if (selectedAddress == null)
            {
                return;
            }

            this.SetLogObserver(MessageDispatcher.Instance.LogObserverFactory.CreateRemoteObserver(selectedAddress));
            this.lastSelectedIndex = this.cbxSource.SelectedIndex;
        }

        private void SetLogObserver(ILogObserver observer)
        {
            if (this.currentObserver != null)
            {
                this.currentObserver.MessageLogged -= this.OnMessageLogged;
            }

            this.richTextBox1.Clear();

            this.currentObserver = observer;
            if (this.currentObserver != null)
            {
                var level = this.comboBoxLogLevel.SelectedItem as LogLevel;
                if (level != null)
                {
                    this.currentObserver.MinLevel = level;
                }

                this.currentObserver.MessageLogged += this.OnMessageLogged;
            }
        }

        private void OnMessageLogged(object sender, LogEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.OnMessageLogged(sender, e)));
                return;
            }

            if (this.IsDisposed)
            {
                return;
            }

            Color color;
            if (e.Level == LogLevel.Fatal)
            {
                color = Color.DarkRed;
            }
            else if (e.Level == LogLevel.Error)
            {
                color = Color.Red;
            }
            else if (e.Level == LogLevel.Warn)
            {
                color = Color.Orange;
            }
            else if (e.Level == LogLevel.Info)
            {
                color = Color.White;
            }
            else if (e.Level == LogLevel.Debug)
            {
                color = Color.LightGray;
            }
            else if (e.Level == LogLevel.Trace)
            {
                color = Color.DimGray;
            }
            else
            {
                return;
            }

            var loggerName = e.LoggerName;
            var lastDot = loggerName.LastIndexOf('.');
            if (lastDot >= 0)
            {
                loggerName = loggerName.Substring(lastDot + 1);
            }

            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionLength = 0;

            this.richTextBox1.SelectionColor = color;
            this.richTextBox1.AppendText(e.Timestamp.ToLocalTime().ToLongTimeString());
            this.richTextBox1.AppendText(" [");
            this.richTextBox1.AppendText(loggerName);
            this.richTextBox1.AppendText("] ");
            this.richTextBox1.AppendText(e.Message);
            this.richTextBox1.AppendText("\n");
            if (!string.IsNullOrEmpty(e.Exception))
            {
                this.richTextBox1.AppendText(e.Exception);
                this.richTextBox1.AppendText("\n");
            }

            this.richTextBox1.SelectionColor = this.richTextBox1.ForeColor;
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;

            if (this.checkBoxAutoScroll.Checked)
            {
                this.richTextBox1.ScrollToCaret();
            }
        }

        private void ComboBoxLogLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            var level = this.comboBoxLogLevel.SelectedItem as LogLevel;
            if (level != null && this.currentObserver != null)
            {
                this.currentObserver.MinLevel = level;
            }
        }
    }
}
