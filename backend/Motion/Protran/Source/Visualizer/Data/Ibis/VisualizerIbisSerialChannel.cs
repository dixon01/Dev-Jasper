// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerIbisSerialChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;

    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;

    /// <summary>
    /// Channel to get telegrams from Ibis bus
    /// </summary>
    public class VisualizerIbisSerialChannel : IbisSerialChannel
    {
        private TelegramControl control;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerIbisSerialChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public VisualizerIbisSerialChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            // disable the RemoteComputer state changes
            this.RemoteComputer = new VisualizerRemoteComputer();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the IBIS bus.
        /// </summary>
        public bool IbisBusEnabled { get; set; }

        /// <summary>
        /// Enqueues a new input into the channel of this service.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public void SetControl(TelegramControl controller)
        {
            this.control = controller;
        }

        /// <summary>
        /// Opens the IBIS channel.
        /// </summary>
        protected override void DoOpen()
        {
            if (this.IbisBusEnabled)
            {
                base.DoOpen();
            }
        }

        /// <summary>
        /// Read thread for serial port
        /// </summary>
        protected override void ReadThread()
        {
            // kick off JIT, so we are faster when we actually get some data
            this.PreJitMethods();
            while (this.RunReader)
            {
                try
                {
                    {
                        bool read = this.Parser.ReadFrom(this.SerialPort.BaseStream);

                        if (this.MonitorTime && this.IsFirst)
                        {
                            this.StopWatch.Start();
                            this.IsFirst = false;
                        }

                        if (read)
                        {
                            this.RemoteComputer.HasSentData = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // an exception is occurred reading bytes from the serial port.
                    // I've to stop everything and then restart.
                    Logger.Info(ex, "Error on reading from the serial port.");
                }
            }
        }

        /// <summary>
        /// On Telegram Data Received
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void ParserOnTelegramDataReceived(object sender, TelegramDataEventArgs e)
        {
            // firstly, I manage the telegram's "low level" requirements
            this.ManageAnswer(e.Data, null);

            // secondly, I do all the stuffs about this UI channel
            this.Logger.Trace(() => "Received telegram: " + BufferUtils.FromByteArrayToHexString(e.Data));
            this.control.DisplayTelegram(e.Data);
            this.control.SendTelegram(e.Data, true);

            // and finally, I log the data just arrived
            // (if this feature is enabled)
            if (this.Recorder != null)
            {
                this.Recorder.Write(e.Data);
            }
        }
    }
}
