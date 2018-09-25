// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressHeaderTelegramParser{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AddressHeaderTelegramParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Telegram handler for telegrams that contain a
    /// predefined header including an IBIS address character (hex address number).
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram this handler creates in its <see cref="TelegramParser{T}.Parse"/> method.
    /// </typeparam>
    public class AddressHeaderTelegramParser<T> : SimpleHeaderTelegramParser<T>
        where T : Telegram, IAddressedTelegram, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressHeaderTelegramParser{T}"/> class.
        /// </summary>
        /// <param name="header">
        /// The header characters.
        /// </param>
        /// <param name="rules">
        /// The telegram checks used to check if the incoming telegram is valid.
        /// </param>
        public AddressHeaderTelegramParser(string header, params ITelegramRule[] rules)
            : base(header, rules)
        {
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header including the address character.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                // one more character for address
                return base.HeaderSize + this.ByteInfo.ByteSize;
            }
        }

        /// <summary>
        /// Checks if the given telegram is for the given address.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="address">
        /// The IBIS address (value between 0 and 15).
        /// </param>
        /// <returns>
        /// True if the telegram contains the given IBIS address.
        /// </returns>
        public override bool IsForAddress(byte[] telegram, int address)
        {
            return this.GetAddress(telegram) == address;
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// This method also populates the <see cref="IAddressedTelegram.IbisAddress"/> property.
        /// </summary>
        /// <param name="data">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// the telegram object containing the payload of the given telegram.
        /// </returns>
        protected override T Parse(byte[] data)
        {
            var telegram = base.Parse(data);
            telegram.IbisAddress = this.GetAddress(data);
            return telegram;
        }

        private int GetAddress(byte[] telegram)
        {
            int address = telegram[this.IdentifierSize + this.ByteInfo.ByteSize - 1];

            // IBIS addresses are very simple: 0-9 followed by :;<=>? which
            // are exactly the next 6 ASCII characters, so "- '0'" creates
            // always the right address
            return address - '0';
        }
    }
}
