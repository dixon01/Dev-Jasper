// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FieldType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using Gorba.Common.Protocols.Udcp.Datagram;

    /// <summary>
    /// The possible UDCP field types.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Not a valid field type, only used by <see cref="ErrorFieldField"/>.
        /// </summary>
        None = 0,

        /// <summary>
        /// The unit name as a string.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses.
        /// </summary>
        UnitName = 1,

        /// <summary>
        /// The software version as a human readable string.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses.
        /// </summary>
        SoftwareVersion = 2,

        /// <summary>
        /// The IP version 4 address.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses
        /// and <see cref="DatagramType.SetConfiguration"/> requests.
        /// </summary>
        IpAddress = 3,

        /// <summary>
        /// The IP version 4 network mask.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses
        /// and <see cref="DatagramType.SetConfiguration"/> requests.
        /// </summary>
        NetworkMask = 4,

        /// <summary>
        /// The IP version 4 gateway address.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses
        /// and <see cref="DatagramType.SetConfiguration"/> requests.
        /// </summary>
        Gateway = 5,

        /// <summary>
        /// A flag indicating if the network interface should be or is configured using DHCP.
        /// This field is available in <see cref="DatagramType.GetInformation"/> responses
        /// and <see cref="DatagramType.SetConfiguration"/> requests.
        /// </summary>
        DhcpEnabled = 6,

        /// <summary>
        /// The error code in a response.
        /// <seealso cref="Fields.ErrorCode"/>
        /// This field can be available in all response datagrams.
        /// </summary>
        ErrorCode = 100,

        /// <summary>
        /// The field which is erroneous. This is only valid if
        /// <see cref="ErrorCode"/> is <see cref="Fields.ErrorCode.BadField"/>
        /// This field can be available in all response datagrams.
        /// </summary>
        ErrorField = 101,

        /// <summary>
        /// The error message as a string.
        /// This field can be available in all response datagrams.
        /// </summary>
        ErrorMessage = 102,
    }
}