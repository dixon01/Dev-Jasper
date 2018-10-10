// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    ///   Message types for the driver
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///   Shows an information message
        /// </summary>
        Info = 1,

        /// <summary>
        ///   Shows an instruction message. The driver has to confirm this message
        /// </summary>
        Instruction = 2,

        /// <summary>
        ///   Shows an Error message
        /// </summary>
        Error = 3,

        /// <summary>
        ///   Shows an alarm message
        /// </summary>
        Alarm = 4,
    }
}