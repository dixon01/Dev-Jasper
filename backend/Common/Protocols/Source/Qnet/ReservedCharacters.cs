// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReservedCharacters.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of some (reserved) characters
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Definition of some (reserved) characters
    /// </summary>
    internal class ReservedCharacters
    {
        /// <summary>
        /// Start of Header reserved character
        /// </summary>
        public const byte SOH = 0x01;

        /// <summary>
        /// Enquiry reserved character
        /// </summary>
        public const byte ENQ = 0x05;

        /// <summary>
        /// Data Link Escape reserved character
        /// </summary>
        public const byte DLE = 0x10; 
    }
}
