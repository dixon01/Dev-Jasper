// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The alarm acknowledge state.
    /// </summary>
    public enum AlarmAck
    {
        /// <summary>
        /// The alarm was received.
        /// </summary>
        Received,

        /// <summary>
        /// The alarm was confirmed.
        /// </summary>
        Confirmed,

        /// <summary>
        /// The alarm has ended.
        /// </summary>
        Ended,
    }
}