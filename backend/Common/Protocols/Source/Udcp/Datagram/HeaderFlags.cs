// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderFlags.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HeaderFlags type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Datagram
{
    using System;

    /// <summary>
    /// The possible UDCP header flags.
    /// </summary>
    [Flags]
    public enum HeaderFlags
    {
        /// <summary>
        /// No header flags are defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// The datagram is a response (contrary to a request which doesn't have this flag set).
        /// </summary>
        Response = 1,
    }
}