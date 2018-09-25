// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusCode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    /// <summary>
    /// Enum for the status codes.
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// Data is missing on the device
        /// (from CU5 to topbox and vice versa).
        /// </summary>
        MissingData = -1,

        /// <summary>
        /// Everything Ok
        /// (from CU5 to topbox and vice versa).
        /// </summary>
        Ok = 0,

        /// <summary>
        /// INIT OBU not available
        /// (from topbox to CU5).
        /// </summary>
        Fallback
    }
}