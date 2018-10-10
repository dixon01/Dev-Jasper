// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutgoingData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutgoingData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;

    /// <summary>
    /// The possible outgoing data items.
    /// </summary>
    [Flags]
    public enum OutgoingData : uint
    {
        /// <summary>
        /// DirectX rendering.
        /// </summary>
        DirectX = 0x00000001,

        /// <summary>
        /// AHDLC rendering.
        /// </summary>
        Ahdlc = 0x00000002,

        /// <summary>
        /// Audio output.
        /// </summary>
        Audio = 0x00000004,

        /// <summary>
        /// Medi protocol.
        /// </summary>
        Medi = 0x80000000
    }
}