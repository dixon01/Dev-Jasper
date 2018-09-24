// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteriorSoundMode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteriorSoundMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The interior sound mode.
    /// </summary>
    public enum InteriorSoundMode
    {
        /// <summary>
        /// No sound is played.
        /// </summary>
        None = 0,

        /// <summary>
        /// The sound from the poi.csv file is played.
        /// </summary>
        Poi = 1,

        /// <summary>
        /// The sound from the via.csv file is played.
        /// </summary>
        Via = 2,

        /// <summary>
        /// The sound is muted.
        /// </summary>
        Mute = 3
    }
}