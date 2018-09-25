// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightCode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The eci traffic light code.
    /// </summary>
    public enum EciTrafficLightCode
    {
        /// <summary>
        /// The acknowledge.
        /// </summary>
        Ack = 'a',

        /// <summary>
        /// The entry.
        /// </summary>
        Entry = 'c',

        /// <summary>
        /// The checkpoint.
        /// </summary>
        Checkpoint = 'd',

        /// <summary>
        /// The exit.
        /// </summary>
        Exit = 'e'
    }
}
