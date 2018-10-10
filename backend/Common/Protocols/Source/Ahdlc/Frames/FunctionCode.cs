// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionCode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FunctionCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    using System;

    /// <summary>
    /// The function code with the direction.
    /// </summary>
    [Flags]
    public enum FunctionCode
    {
        /// <summary>
        /// Status request.
        /// Direction: from master
        /// Code: 0
        /// </summary>
        StatusRequest = 0x80,

        /// <summary>
        /// Status response.
        /// Direction: from slave
        /// Code: 0
        /// </summary>
        StatusResponse = 0x00,

        /// <summary>
        /// Setup command.
        /// Direction: from master
        /// Code: 1
        /// </summary>
        SetupCommand = 0x90,

        /// <summary>
        /// Setup command response.
        /// Direction: from slave
        /// Code: 1
        /// </summary>
        SetupResponse = 0x10,

        /// <summary>
        /// Output command.
        /// Direction: from master
        /// Code: 2
        /// </summary>
        OutputCommand = 0xA0,

        /// <summary>
        /// Output command response.
        /// Direction: from slave
        /// Code: 2
        /// </summary>
        OutputResponse = 0x20,
    }
}