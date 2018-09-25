// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignInfoControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignInfoControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Master;

    /// <summary>
    /// Control that handles a single sign.
    /// </summary>
    public partial class SignInfoControl : UserControl
    {
        private readonly List<TabPage> allTabs = new List<TabPage>();

        private AhdlcMaster master;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInfoControl"/> class.
        /// </summary>
        public SignInfoControl()
        {
            this.InitializeComponent();

            foreach (TabPage tabPage in this.tabControl.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    var sender = control as DataSendControlBase;
                    if (sender != null)
                    {
                        sender.Context = this;
                    }
                }

                this.allTabs.Add(tabPage);
            }

            this.UpdateVisibleTabs();
        }

        /// <summary>
        /// Event that is fired whenever <see cref="IDataSendContext.HasColor"/>
        /// or <see cref="IDataSendContext.SignSize"/> updated.
        /// </summary>
        public event EventHandler Updated;

        /// <summary>
        /// Gets or sets the AHDLC master.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AhdlcMaster Master
        {
            get
            {
                return this.master;
            }

            set
            {
                if (this.master == value)
                {
                    return;
                }

                if (this.master != null)
                {
                    this.master.StatusReceived -= this.MasterOnStatusReceived;
                }

                this.master = value;
                if (this.master != null)
                {
                    this.master.StatusReceived += this.MasterOnStatusReceived;
                }

                this.OnUpdated();
            }
        }

        /// <summary>
        /// Gets or sets the AHDLC address of this sign.
        /// </summary>
        public int Address { get; set; }

        bool IDataSendContext.HasColor
        {
            get
            {
                return this.checkBoxColor.Checked;
            }
        }

        Size IDataSendContext.SignSize
        {
            get
            {
                return new Size((int)this.numericUpDownWidth.Value, (int)this.numericUpDownHeight.Value);
            }
        }

        /// <summary>
        /// Sends a frame to the sign.
        /// </summary>
        /// <param name="frame">
        /// The frame; its <see cref="FrameBase.Address"/> will be set automatically by this method.
        /// </param>
        public void SendFrame(LongFrameBase frame)
        {
            if (this.Master == null)
            {
                return;
            }

            this.timerStatusRequest.Enabled = false;

            frame.Address = this.Address;
            this.Master.EnqueueFrame(frame);

            this.timerStatusRequest.Enabled = true;
        }

        /// <summary>
        /// Raises the <see cref="Updated"/> event.
        /// </summary>
        protected virtual void OnUpdated()
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateVisibleTabs()
        {
            this.tabControl.TabPages.Clear();
            if (this.checkBoxColor.Checked)
            {
                this.tabControl.TabPages.Add(this.tabPageStatus);
                this.tabControl.TabPages.Add(this.tabPageColor);
            }
            else
            {
                foreach (var tabPage in this.allTabs)
                {
                    if (tabPage != this.tabPageColor)
                    {
                        this.tabControl.TabPages.Add(tabPage);
                    }
                }
            }
        }

        private void CheckBoxEnabledCheckedChanged(object sender, EventArgs e)
        {
            var enabled = this.checkBoxEnabled.Checked;
            this.timerStatusRequest.Enabled = enabled;
            if (enabled)
            {
                this.Parent.Text += "*";
                this.SendFrame(new StatusRequestFrame());
            }
            else
            {
                this.Parent.Text = this.Parent.Text.TrimEnd('*');
            }
        }

        private void TimerStatusRequestTick(object sender, EventArgs e)
        {
            this.SendFrame(new StatusRequestFrame());
        }

        private void MasterOnStatusReceived(object sender, StatusResponseEventArgs e)
        {
            if (e.Status.Address != this.Address)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.MasterOnStatusReceived(sender, e)));
                return;
            }

            this.propertyGridStatus.SelectedObject = e.Status;
            this.buttonSizeFromStatus.Enabled = e.Status.SignType != StatusResponseFrame.SignTypes.Unknown;
        }

        private void ButtonSizeFromStatusClick(object sender, EventArgs e)
        {
            var status = this.propertyGridStatus.SelectedObject as StatusResponseFrame;
            if (status == null)
            {
                return;
            }

            this.checkBoxColor.Checked = status.SignType == StatusResponseFrame.SignTypes.ColorLed;
            if (!status.Width.HasValue || !status.Height.HasValue)
            {
                return;
            }

            this.numericUpDownWidth.Value = status.Width.Value;
            this.numericUpDownHeight.Value = status.Height.Value;
        }

        private void CheckBoxColorCheckedChanged(object sender, EventArgs e)
        {
            this.UpdateVisibleTabs();
            this.OnUpdated();
        }

        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            this.OnUpdated();
        }
    }
}
