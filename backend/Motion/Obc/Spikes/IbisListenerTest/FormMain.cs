// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormMain.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The listener serial port for a DS001 telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IbisListenerTest
{
    using System.IO.Ports;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Obc.Common;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Obc.Ibis.Core.Channels;

    /// <summary>
    /// The form 1.
    /// </summary>
    public partial class FormMain : Form
    {
        private ChannelBase channel;

        private SerialPortConfig portConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class.
        /// </summary>
        public FormMain()
        {
            this.InitializeComponent();
            this.ConfigureSerialPort();
            this.channel.Open();
        }

        /// <summary>
        /// This delegate enables asynchronous calls for setting the text property of <see cref="label1"/>
        /// </summary>
        /// <param name="obj">
        /// object parameter
        /// </param>
        /// <param name="eventArgs">telegram event</param>
        internal delegate void SetTextCallBack(object obj, TelegramEventArgs eventArgs);

        private void ConfigureSerialPort()
        {
            this.portConfig = new SerialPortConfig("COM1", 1200, 7, Parity.Even, StopBits.Two, false, false, true);
            this.channel = new SerialPortChannel(this.portConfig);
            this.channel.TelegramReceived += this.ChannelOnTelegramReceived;
        }

        private void ChannelOnTelegramReceived(object sender, TelegramEventArgs e)
        {
            if (this.label1.InvokeRequired)
            {
                var d = new SetTextCallBack(this.ChannelOnTelegramReceived);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                if (e.Telegram is DS001)
                {
                    var telegram = (DS001)e.Telegram;
                    this.label1.Text = telegram.LineNumber;
                }
            }
        }
    }
}
