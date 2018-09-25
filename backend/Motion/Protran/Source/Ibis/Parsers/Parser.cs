// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Core.Buffers;

    using NLog;

    /// <summary>
    /// This class parses an IBIS byte array to
    /// a suitable class (with properties), checks
    /// the validity of a telegram and creates byte array
    /// starting from a class.
    /// </summary>
    public abstract class Parser
    {
        /// <summary>
        /// Logger to be used by subclasses.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// The length (in bytes) of the circular buffer used to
        /// read data from the serial port.
        /// </summary>
        private const int CircularBufferLength = 1024;

        /// <summary>
        /// The circular buffer used to store each byte received
        /// from the serial port.
        /// </summary>
        private readonly CircularBuffer circularBuffer;

        private readonly List<ITelegramParser> telegramParsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="byteType">
        /// The byte of the parser.
        /// </param>
        /// <param name="checkChecksum">
        /// The check sum.
        /// </param>
        /// <param name="configs">
        /// The telegram configurations.
        /// </param>
        protected Parser(ByteType byteType, bool checkChecksum, IEnumerable<TelegramConfig> configs)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.circularBuffer = new CircularBuffer(CircularBufferLength);

            this.ByteInfo = ByteInfo.For(byteType);
            this.Marker = 0x0D;
            this.CheckChecksum = checkChecksum;

            var factory = new TelegramParserFactory();
            this.telegramParsers = new List<ITelegramParser>();
            foreach (var config in configs)
            {
                if (!config.Enabled)
                {
                    continue;
                }

                var parser = factory.CreateParser(this.ByteInfo, config);
                this.telegramParsers.Add(parser);
            }

            // sort the handlers by priority
            // (this is required since some telegrams have the same first character and
            // then define a second character or not)
            this.telegramParsers.Sort((left, right) => left.Priority - right.Priority);
        }

        /// <summary>
        /// Event that is risen every time a telegram was parsed inside <see cref="ReadFrom"/>.
        /// </summary>
        public event EventHandler<TelegramDataEventArgs> TelegramDataReceived;

        /// <summary>
        /// Gets the byte information.
        /// </summary>
        public ByteInfo ByteInfo { get; private set; }

        /// <summary>
        /// Gets or sets the byte that is considered the marker
        /// of a telegram inside the circular buffer.
        /// </summary>
        public byte Marker { get; set; }

        /// <summary>
        /// Gets a value indicating whether
        /// this parser has to check the cyclic redundancy check code for each telegram.
        /// </summary>
        protected bool CheckChecksum { get; private set; }

        /// <summary>
        /// Creates a new instance of a subclass of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="behaviour">
        /// The behaviour configuration.
        /// </param>
        /// <param name="telegrams">
        /// The telegram configurations.
        /// </param>
        /// <returns>
        /// a new Parser
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If no suitable parser could be found.
        /// </exception>
        public static Parser Create(Behaviour behaviour, IEnumerable<TelegramConfig> telegrams)
        {
            switch (behaviour.ByteType)
            {
                case ByteType.UnicodeBigEndian:
                    return new Parser16Bit(behaviour.CheckCrc, telegrams);
                case ByteType.Hengartner8:
                    return new Parser8Bit(behaviour.CheckCrc, telegrams);
                case ByteType.Ascii7:
                    return new Parser7Bit(behaviour.CheckCrc, telegrams);
                default:
                    throw new ArgumentException("Byte type not supported: " + behaviour.ByteType);
            }
        }

        /// <summary>
        /// Reads from the given input stream and parses the data.
        /// If telegrams are found, <see cref="TelegramDataReceived"/> is risen
        /// for each telegram.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// true if data was read from the stream.
        /// </returns>
        public bool ReadFrom(Stream input)
        {
            int bytesRead = input.Read(
                this.circularBuffer.Buffer, this.circularBuffer.Tail, this.circularBuffer.RemainingLength);
            if (bytesRead <= 0)
            {
                return false;
            }

            // I've to log each byte read from the serial port.
            this.Logger.Trace(() => this.circularBuffer.ToString(bytesRead));

            this.circularBuffer.UpdateTail(bytesRead);

            while (true)
            {
                int indexMarker = this.FindMarker(this.circularBuffer);
                if (indexMarker == -1)
                {
                    break;
                }

                // until now I've received several bytes and
                // in this moment I've received the carriage return character.
                // this means that the next byte terminates the actual IBIS telegram.
                int charsForChecksum;
                var charSize = this.ByteInfo.ByteSize;
                int charsFormarker = charsForChecksum = charSize;
                if (!this.circularBuffer.IsIndexBetweenHeadAndTail(indexMarker + charsFormarker + charsForChecksum))
                {
                    break;
                }

                while (this.circularBuffer.CurrentLength > 0 && !this.IsTelegramHeader(this.circularBuffer))
                {
                    this.circularBuffer.UpdateHead(charSize);
                }

                if (this.circularBuffer.CurrentLength > 0)
                {
                    // a complete IBIS telegram was received.
                    // the last byte just read is the check sum.
                    byte[] telegram = this.circularBuffer.GetBufferPiece(indexMarker + charsFormarker +
                        charsForChecksum);
                    this.circularBuffer.UpdateHead(telegram.Length);

                    this.RaiseTelegramDataReceived(new TelegramDataEventArgs(telegram));
                }
            }

            return true;
        }

        /// <summary>
        /// Updates the last byte(s) of the given telegram to be a correct CRC.
        /// </summary>
        /// <param name="telegram">the telegram to update</param>
        public abstract void UpdateChecksum(byte[] telegram);

        /// <summary>
        /// Checks if the incoming telegram has a valid CRC value.
        /// </summary>
        /// <param name="telegram">The telegram that has to be checked.</param>
        /// <returns>True if the telegram has a valid CRC value, else false.</returns>
        public abstract bool IsChecksumCorrect(byte[] telegram);

        /// <summary>
        /// Get the registered telegram handler for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// The handler or null if no handler is registered for the given telegram.
        /// </returns>
        public ITelegramParser GetTelegramParser(byte[] telegram)
        {
            foreach (var parser in this.telegramParsers)
            {
                if (parser.Accept(telegram))
                {
                    return parser;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the index of the first marker inside the circular buffer,
        /// making a scan between the actual values of the head and the tail.
        /// </summary>
        /// <param name="circularBuffer">The buffer in which search for the IBIS marker.</param>
        /// <returns>The marker's index or -1 if the marker is not found.</returns>
        protected abstract int FindMarker(CircularBuffer circularBuffer);

        /// <summary>
        /// Checks the circular buffer if it starts with a telegram header.
        /// </summary>
        /// <param name="circularBuffer">
        /// The circular buffer.
        /// </param>
        /// <returns>
        /// True if the first character in the buffer is a telegram header character.
        /// </returns>
        protected abstract bool IsTelegramHeader(CircularBuffer circularBuffer);

        /// <summary>
        /// Raises the <see cref="TelegramDataReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTelegramDataReceived(TelegramDataEventArgs e)
        {
            var handler = this.TelegramDataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
