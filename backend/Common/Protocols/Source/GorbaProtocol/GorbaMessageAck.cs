// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaMessageAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.GorbaProtocol
{
    using System;

    /// <summary>
    /// The acknowledge for Gorba messages.
    /// </summary>
    [Serializable]
    public class GorbaMessageAck
    {
        /// <summary>
        /// Gets or sets the unique identifier of the update message that was acknowledged.
        /// </summary>
        public Guid Id { get; set; }
    }
}