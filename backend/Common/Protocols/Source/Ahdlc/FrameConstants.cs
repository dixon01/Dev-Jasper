// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameConstants.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    /// <summary>
    /// Constants used when encoding and decoding AHDLC frames.
    /// </summary>
    internal static class FrameConstants
    {
        /// <summary>
        /// The boundary byte (0x7E).
        /// </summary>
        public const byte Boundary = 0x7E;

        /// <summary>
        /// The escape byte (0x7D).
        /// </summary>
        public const byte Escape = 0x7D;

        /// <summary>
        /// The XOR byte (0x20).
        /// </summary>
        public const byte XOr = 0x20;
    }
}