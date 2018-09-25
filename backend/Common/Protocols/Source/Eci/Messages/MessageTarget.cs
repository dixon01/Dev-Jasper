// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTarget.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageTarget type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The targets of a <see cref="EciTextMessage"/>.
    /// </summary>
    [Flags]
    public enum MessageTarget
    {
        /// <summary>
        /// The message is for the driver.
        /// </summary>
        Driver = 0x01,

        /// <summary>
        /// The message is shown on the interior display.
        /// </summary>
        Display = 0x02,

        /// <summary>
        /// The message is announced through the speaker.
        /// </summary>
        Speaker = 0x04,
    }
}