// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagType.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains the existing endianess types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    /// <summary>
    /// Contains the existing endianess types.
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// Invalid tag type.
        /// </summary>
        Invalid,

        /// <summary>
        /// Notification tag type.
        /// </summary>
        Notification,

        /// <summary>
        /// Request tag type.
        /// </summary>
        Request,

        /// <summary>
        /// Response tag type.
        /// </summary>
        Response
    }
}