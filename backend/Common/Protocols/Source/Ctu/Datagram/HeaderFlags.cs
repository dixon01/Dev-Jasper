// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderFlags.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    using System;

    /// <summary>
    /// Contains the existing flags for the "Flags" field in the CTU header.
    /// </summary>
    [Flags]
    public enum HeaderFlags
    {
        /// <summary>
        /// Little endian-ness
        /// </summary>
        LittleEndian = 0x01
    }
}