// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortFrameHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortFrameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Frame handler that uses a serial port for sending and receiving frames.
    /// </summary>
    public partial class SerialPortFrameHandler : IFrameHandler, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SerialPortFrameHandler>();

        private readonly RtsMode rtsMode;

        private readonly SerialPort serialPort;

        private readonly FrameDecoder decoder;
        private readonly FrameEncoder encoder;

        private readonly double millisecondsPerByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortFrameHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The serial port configuration.
        /// </param>
        public SerialPortFrameHandler(SerialPortConfig config)
        {
            this.rtsMode = config.RtsMode;

            this.serialPort = new SerialPort();
            this.serialPort.PortName = config.ComPort;
            this.serialPort.BaudRate = config.BaudRate;
            this.serialPort.DataBits = config.DataBits;
            this.serialPort.StopBits = config.StopBits;
            this.serialPort.Parity = config.Parity;
            this.serialPort.Handshake = Handshake.None;
            this.serialPort.DtrEnable = true;
            this.ResetRts();
            this.serialPort.Open();

            this.decoder = new FrameDecoder(config.IsHighSpeed) { IgnoreFrameStart = config.IgnoreFrameStart };
            this.encoder = new FrameEncoder(config.IsHighSpeed);

            double bitsPerByte = config.DataBits;
            switch (config.StopBits)
            {
                case StopBits.One:
                    bitsPerByte++;
                    break;
                case StopBits.Two:
                    bitsPerByte += 2;
                    break;
                case StopBits.OnePointFive:
                    bitsPerByte += 1.5;
                    break;
            }

            if (config.Parity != Parity.None)
            {
                bitsPerByte++;
            }

            this.millisecondsPerByte = bitsPerByte / this.serialPort.BaudRate * 1000;
        }

        /// <summary>
        /// Reads the next frame from the underlying stream.
        /// This method blocks until an entire frame is available or the
        /// end of the stream was reached (EOS).
        /// </summary>
        /// <returns>
        /// The decoded <see cref="FrameBase"/> or null if the end of the stream was reached (EOS).
        /// </returns>
        public FrameBase ReadNextFrame()
        {
            int read;
            while ((read = this.serialPort.ReadByte()) != -1)
            {
                var frame = this.decoder.AddByte((byte)read);
                if (frame != null)
                {
                    return frame;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes an entire frame to the underlying stream.
        /// </summary>
        /// <param name="frame">
        /// The frame to write.
        /// </param>
        public void WriteFrame(FrameBase frame)
        {
            var data = this.encoder.Encode(frame);
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Writing frame {0} ({1} bytes)", BitConverter.ToString(data), data.Length);
            }

            this.SetRtsSending(true);
            this.serialPort.Write(data, 0, data.Length);

            // add 10% to make sure the data is sent
            var sleepTime = Math.Ceiling(data.Length * this.millisecondsPerByte * 1.1);

            Logger.Trace("Sleeping {0:0} ms", sleepTime);
            Thread.Sleep((int)sleepTime);
            this.SetRtsSending(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.ResetRts();
            this.serialPort.Close();
            this.serialPort.Dispose();
        }

        private void ResetRts()
        {
            switch (this.rtsMode)
            {
                case RtsMode.Auto:
                    throw new NotSupportedException("Auto mode is not supported on .NET 2.0");
                case RtsMode.Enabled:
                case RtsMode.DisableForTx:
                    this.serialPort.RtsEnable = true;
                    break;
                default:
                    this.serialPort.RtsEnable = false;
                    break;
            }
        }

        private void SetRtsSending(bool sending)
        {
            switch (this.rtsMode)
            {
                case RtsMode.Default:
                case RtsMode.EnableForTx:
                    this.serialPort.RtsEnable = sending;
                    break;
                case RtsMode.DisableForTx:
                    this.serialPort.RtsEnable = !sending;
                    break;
            }
        }
    }
}
