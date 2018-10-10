// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.IO.Ports;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Master;

    /// <summary>
    /// The main application form.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly SerialPort serialPort = new SerialPort();

        private AhdlcMaster master;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            for (int i = 1; i <= 15; i++)
            {
                var page = new TabPage("Addr. " + i);
                var signInfo = new SignInfoControl();
                signInfo.Address = i;
                signInfo.Dock = DockStyle.Fill;
                page.Controls.Add(signInfo);

                this.tabControl.TabPages.Add(page);
            }

            foreach (var portName in SerialPort.GetPortNames())
            {
                this.comboBoxPorts.Items.Add(portName);
            }
        }

        private AhdlcMaster Master
        {
            get
            {
                return this.master;
            }

            set
            {
                this.master = value;
                foreach (TabPage tabPage in this.tabControl.TabPages)
                {
                    var signInfo = (SignInfoControl)tabPage.Controls[0];
                    signInfo.Master = value;
                }
            }
        }

        private void ButtonStartClick(object sender, EventArgs e)
        {
            if (this.Master == null)
            {
                try
                {
                    this.serialPort.PortName = this.comboBoxPorts.Text;
                    this.serialPort.BaudRate = this.checkBoxHighSpeed.Checked ? 38400 : 19200;
                    this.serialPort.DataBits = 8;
                    this.serialPort.StopBits = StopBits.Two;
                    this.serialPort.Parity = Parity.None;
                    this.serialPort.Handshake = Handshake.None;
                    this.serialPort.DtrEnable = true;
                    this.serialPort.Open();

                    var ahdlcMaster =
                        new AhdlcMaster(
                            new StreamFrameHandler(this.serialPort.BaseStream, this.checkBoxHighSpeed.Checked));
                    ahdlcMaster.Start();
                    this.Master = ahdlcMaster;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.buttonStart.Text = "Stop Master";
                return;
            }

            this.Master.Stop();
            this.Master = null;

            this.serialPort.Close();

            this.buttonStart.Text = "Start Master";
        }

        private void ComboBoxPortsSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = this.comboBoxPorts.SelectedItem is string;
        }
    }
}
