// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionNumber.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Indicator of the NTP/SNTP version number.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    /// <summary>
    /// Indicator of the NTP/SNTP version number.
    /// </summary>
    public enum VersionNumber
    {
        /// <summary>
        /// Version 3 (<c>IPv4</c> only).
        /// </summary>
        Version3 = 3,

        /// <summary>
        /// Version 4 (<c>IPv4</c>, <c>IPv6</c> and OSI).
        /// </summary>
        Version4 = 4,
    }
}
