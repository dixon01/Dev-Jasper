// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamFeature.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamFeature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    /// <summary>
    /// Features of a stream, used in <see cref="StreamHandshake"/>.
    /// Values between 1 and 7 are reserved for the type of the stream
    /// and are mutually exclusive, most other features can be
    /// combined freely
    /// </summary>
    [Flags]
    public enum StreamFeature : byte
    {
        /// <summary>
        /// Messages are transmitted through the stream.
        /// This can't be combined with <see cref="StreamsType"/>!
        /// </summary>
        MessagesType = 1,

        /// <summary>
        /// Streams are transmitted through the stream.
        /// This can't be combined with <see cref="MessagesType"/>!
        /// </summary>
        StreamsType = 2,

        /// <summary>
        /// The mask to be applied when looking at the type of the stream only.
        /// </summary>
        TypeMask = 0x07,

        /// <summary>
        /// All messages are appended with a frame number and an acknowledged frame
        /// number.
        /// </summary>
        Framing = 0x08,

        /// <summary>
        /// The stream will be compressed using GZip encoding (after the handshake).
        /// </summary>
        GZip = 0x10,

        /// <summary>
        /// The stream will be secured using SSL (after the handshake).
        /// </summary>
        Ssl = 0x20,

        /// <summary>
        /// The connection is a gateway connection with limited communication (routing and subscription updates).
        /// </summary>
        Gateway = 0x40,
    }
}
