// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramParser{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Generic base class for telegram handlers.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram this handler creates in its <see cref="Parse"/> method.
    /// </typeparam>
    public abstract class TelegramParser<T> : ITelegramParser
        where T : Telegram, new()
    {
        /// <summary>
        /// Gets the priority, the lower the number the higher the priority.
        /// This class uses the header size to determine the priority.
        /// </summary>
        public virtual int Priority
        {
            get
            {
                return 1000 - (10 * this.HeaderSize);
            }
        }

        /// <summary>
        /// Gets the byte information for this handler.
        /// This is set before any other methods of this class
        /// are called.
        /// </summary>
        [XmlIgnore]
        public ByteInfo ByteInfo { get; private set; }

        /// <summary>
        /// Gets the telegram configuration.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlIgnore]
        public TelegramConfig Config { get; private set; }

        /// <summary>
        /// Gets the size in bytes of the telegram header.
        /// </summary>
        public abstract int HeaderSize { get; }

        /// <summary>
        /// Gets the size in bytes of the telegram footer (marker and CRC).
        /// </summary>
        public virtual int FooterSize
        {
            get
            {
                return this.ByteInfo.ByteSize * 2;
            }
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <param name="config">
        /// The config of the telegram.
        /// </param>
        public virtual void Configure(ByteInfo byteInfo, TelegramConfig config)
        {
            this.ByteInfo = byteInfo;
            this.Config = config;
        }

        /// <summary>
        /// Check whether this object can handle the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given telegram.
        /// </returns>
        public abstract bool Accept(byte[] telegram);

        /// <summary>
        /// Checks if the given telegram is for the given address.
        /// This method always returns true.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="address">
        /// The IBIS address (value between 0 and 15).
        /// </param>
        /// <returns>
        /// This method always returns true.
        /// </returns>
        public virtual bool IsForAddress(byte[] telegram, int address)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the given telegram needs an answer to be sent.
        /// This method always returns false.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// This method always returns false.
        /// </returns>
        public virtual bool RequiresAnswer(byte[] telegram)
        {
            return false;
        }

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="status">The status with which make the answer.</param>
        /// <returns>
        /// This method always returns null.
        /// </returns>
        public virtual byte[] CreateAnswer(byte[] telegram, Status status)
        {
            return null;
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// the telegram object containing the payload of the given telegram.
        /// </returns>
        Telegram ITelegramParser.Parse(byte[] telegram)
        {
            return this.Parse(telegram);
        }

        /// <summary>
        /// To return a status if telegram had previously arrived
        /// </summary>
        /// <returns>
        /// if telegram has previously arrived
        /// </returns>
        public virtual bool TelegramArrivedBefore()
        {
            return false;
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="data">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// the telegram object containing the payload of the given telegram.
        /// </returns>
        protected virtual T Parse(byte[] data)
        {
            int offset = this.HeaderSize;
            int length = data.Length - this.HeaderSize - this.FooterSize;
            var telegram = new T { Payload = new byte[length] };
            Array.Copy(data, offset, telegram.Payload, 0, length);
            return telegram;
        }
    }
}