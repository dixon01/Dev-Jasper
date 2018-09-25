// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITelegramParser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Interface to be implemented by classes that handle IBIS telegrams.
    /// Every received byte array representing a telegram is given to the
    /// responsible handler for parsing and answer generation.
    /// </summary>
    public interface ITelegramParser
    {
        /// <summary>
        /// Gets the telegram configuration.
        /// </summary>
        TelegramConfig Config { get; }

        /// <summary>
        /// Gets the priority, the lower the number the higher the priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="byteInfo">
        /// The byte information.
        /// </param>
        /// <param name="config">
        /// The config of the telegram.
        /// </param>
        void Configure(ByteInfo byteInfo, TelegramConfig config);

        /// <summary>
        /// Check whether this object can handle the given telegram.
        /// Implementations of this method usually check the header and length of the telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given telegram.
        /// </returns>
        bool Accept(byte[] telegram);

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
        /// True if the telegram contains the given IBIS address or doesn't contain an address at all.
        /// </returns>
        bool IsForAddress(byte[] telegram, int address);

        /// <summary>
        /// Checks whether the given telegram needs an answer to be sent.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// true if the IBIS channel has to send an answer for the given telegram
        /// </returns>
        bool RequiresAnswer(byte[] telegram);

        /// <summary>
        /// Creates the IBIS telegram as answer for the incoming telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <param name="status">The status with which make the answer.</param>
        /// <returns>The telegram that is the answer for the incoming telegram.</returns>
        byte[] CreateAnswer(byte[] telegram, Status status);

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// the telegram object containing the payload of the given telegram.
        /// </returns>
        Telegram Parse(byte[] telegram);

        /// <summary>
        /// To return a status if telegram had previously arrived
        /// </summary>
        /// <returns>
        /// if telegram has previously arrived
        /// </returns>
        bool TelegramArrivedBefore();
    }
}
