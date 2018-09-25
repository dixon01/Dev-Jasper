// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialIbisOutput.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialIbisOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ibis
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;

    /// <summary>
    /// Wrapper around a <see cref="SerialPort"/> that allows
    /// us to send telegrams
    /// </summary>
    public class SerialIbisOutput : IDisposable
    {
        private readonly SerialPort port;

        private readonly Parser parser;

        private Telegram expectedAnswer;

        private bool running;

        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialIbisOutput"/> class.
        /// </summary>
        /// <param name="port">
        /// The serial port to use. This port does not have to be opened beforehand.
        /// </param>
        public SerialIbisOutput(SerialPort port)
            : this(port, 7)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialIbisOutput"/> class.
        /// </summary>
        /// <param name="port">
        /// The serial port to use. This port does not have to be opened beforehand.
        /// </param>
        /// <param name="byteSize">
        /// The size of each character. This is required for answer parsing.
        /// </param>
        public SerialIbisOutput(SerialPort port, int byteSize)
        {
            this.port = port;

            var answerConfigs = new TelegramConfig[]
                                    {
                                        new DS120Config { Name = "DS120", Enabled = true },
                                        new DS130Config { Name = "DS130", Enabled = true }
                                    };

            switch (byteSize)
            {
                case 7:
                    this.parser = new Parser7Bit(true, answerConfigs);
                    break;
                case 8:
                    this.parser = new Parser8Bit(true, answerConfigs);
                    break;
                case 16:
                    this.parser = new Parser16Bit(true, answerConfigs);
                    break;
                default:
                    throw new ArgumentException("Unsupported byte size: " + byteSize);
            }

            this.parser.TelegramDataReceived += this.ParserOnTelegramDataReceived;
        }

        /// <summary>
        /// Gets the number of telegrams received.
        /// </summary>
        public int ReceiveCount { get; private set; }

        /// <summary>
        /// Gets the number of bad telegrams received.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Open the underlying serial port.
        /// </summary>
        public void Open()
        {
            this.port.Open();
            this.running = true;
            this.readThread = new Thread(this.Read) { IsBackground = true };
            this.readThread.Start();
        }

        /// <summary>
        /// Send a telegram through the underlying serial port.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        public void SendTelegram(Telegram telegram)
        {
            var data = telegram.Data;
            this.port.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Add an expectation for an answer telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        public void ExpectAnswer(Telegram telegram)
        {
            var expected = this.expectedAnswer;
            if (expected != null)
            {
                Console.Error.WriteLine(
                    "Warning: didn't get expected telegram: " + BitConverter.ToString(expected.Data));
            }

            this.expectedAnswer = telegram;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.running = false;
            this.port.Dispose();

            if (this.expectedAnswer == null)
            {
                return;
            }

            this.ErrorCount++;
            Console.Error.WriteLine("Didn't get expected telegram: " + BitConverter.ToString(this.expectedAnswer.Data));
        }

        private void Read()
        {
            try
            {
                while (this.running)
                {
                    this.parser.ReadFrom(this.port.BaseStream);
                }
            }
            catch (Exception ex)
            {
                if (this.running)
                {
                    Console.Error.WriteLine("Exception while reading: {0}", ex);
                }
            }
        }

        private void ParserOnTelegramDataReceived(object sender, TelegramDataEventArgs e)
        {
            var telegram = e.Data;
            this.ReceiveCount++;
            var expected = this.expectedAnswer;
            if (this.AreEqual(telegram, expected))
            {
                this.expectedAnswer = null;
                Console.Out.WriteLine("Got expected telegram: {0} ", BitConverter.ToString(telegram));
                return;
            }

            Console.Error.WriteLine("Unexpected Telegram: " + BitConverter.ToString(telegram));
            this.ErrorCount++;
        }

        private bool AreEqual(byte[] telegram, Telegram expected)
        {
            if (expected == null || telegram.Length != expected.Data.Length)
            {
                return false;
            }

            for (int i = 0; i < telegram.Length; i++)
            {
                if (expected.Data[i] != telegram[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
