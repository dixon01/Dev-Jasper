// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The possible error codes sent in response datagrams.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Everything was OK.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// The datagram could not be read.
        /// </summary>
        BadDatagram = -1,

        /// <summary>
        /// One of the fields could not be read.
        /// Check the <see cref="FieldType.ErrorField"/> to know which field was not read.
        /// </summary>
        BadField = -2,

        /// <summary>
        /// The datagram could not be processed by the unit (e.g. a functionality is not supported on a given device).
        /// </summary>
        CouldNotProcess = -3,
    }
}