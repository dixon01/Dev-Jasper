// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Channels
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Configuration.Obc.Common;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="ChannelBase"/> using a regular .NET serial port.
    /// </summary>
    public class SerialPortChannel : ChannelBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SerialPortChannel>();

        private readonly SerialPortConfig portConfig;

        private readonly TelegramSerializer serializer = new TelegramSerializer();

        private SerialPort serialPort;

        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortChannel"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SerialPortChannel(SerialPortConfig config)
        {
            this.portConfig = config;
        }

        /// <summary>
        /// Opens this channel.
        /// </summary>
        public override void Open()
        {
            this.Close();

            this.serialPort = new SerialPort(
                this.portConfig.ComPort,
                this.portConfig.BaudRate,
                this.portConfig.Parity,
                this.portConfig.DataBits,
                this.portConfig.StopBits);
            this.serialPort.Open();

            this.readThread = new Thread(this.ReadThread) { IsBackground = true };
            this.readThread.Start();
        }

        /// <summary>
        /// Closes this channel.
        /// </summary>
        public override void Close()
        {
            if (this.serialPort == null)
            {
                return;
            }

            this.readThread = null;

            this.serialPort.Close();
            this.serialPort.Dispose();
            this.serialPort = null;
        }

        /// <summary>
        /// Sends a telegram on this channel.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        public override void SendTelegram(Telegram telegram, TelegramConfigBase config)
        {
            this.serializer.Serialize(this.serialPort.BaseStream, telegram, config);
        }

        private void ReadThread()
        {
            try
            {
                Telegram received;
                while ((received = this.serializer.Deserialize(this.serialPort.BaseStream)) != null
                       && this.readThread != null)
                {
                    this.RaiseTelegramReceived(new TelegramEventArgs(received));
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't read from serial port", ex);
                this.Reopen();
            }
        }
    }
}