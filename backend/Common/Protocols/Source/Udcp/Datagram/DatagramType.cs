// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatagramType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DatagramType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Datagram
{
    /// <summary>
    /// The possible types of datagrams.
    /// The type is the operation to be executed on the unit.
    /// The direction (request or response) is not part of the datagram
    /// type but is set by <see cref="HeaderFlags.Response"/>.
    /// </summary>
    public enum DatagramType
    {
        /// <summary>
        /// Get information from one or all units in the local network.
        /// </summary>
        GetInformation,

        /// <summary>
        /// Set the configuration of a certain unit.
        /// </summary>
        SetConfiguration,

        /// <summary>
        /// Make the unit announce itself (e.g. by blinking a LED or showing an information on the screen).
        /// </summary>
        Announce,

        /// <summary>
        /// The unit should reboot.
        /// </summary>
        Reboot,
    }
}