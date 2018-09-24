// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeechType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpeechType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    /// <summary>
    /// The speech type.
    /// </summary>
    public enum SpeechType
    {
        /// <summary>
        /// No speech is used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Speech is done over GSM.
        /// </summary>
        Gsm = 1,

        /// <summary>
        /// Speech is done over radio.
        /// </summary>
        Radio = 2,
    }
}