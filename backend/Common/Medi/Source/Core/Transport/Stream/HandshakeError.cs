// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandshakeError.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HandshakeError type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    /// <summary>
    /// Possible stream transport handshake error codes.
    /// </summary>
    internal enum HandshakeError
    {
        /// <summary>
        /// Failure, only used in legacy versions of the protocol.
        /// </summary>
        Failure = 0, // has to be 0 since this means failure in the old protocol

        /// <summary>
        /// Handshake was sucessful. No error.
        /// </summary>
        Sucess = 1, // has to be 1 since this means success in the old protocol

        /// <summary>
        /// Provided version of handshake is not supported.
        /// </summary>
        VersionNotSupported = 2,

        /// <summary>
        /// Internal error.
        /// </summary>
        InternalError = 3,

        /// <summary>
        /// No connection could be made.
        /// </summary>
        NoConnection = 4,

        /// <summary>
        /// Stream read or write failed.
        /// </summary>
        StreamError = 5,

        /// <summary>
        /// Could not parse the handshake or ack/nack.
        /// </summary>
        BadContent = 6,

        /// <summary>
        /// Given codec not supported.
        /// </summary>
        CodecNotSupported = 7,
    }
}