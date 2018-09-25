// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TTSVolume.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TTSVolume type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The TTS volume event.
    /// </summary>
    public class TTSVolume : TTSoverIBIS
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TTSVolume"/> class.
        /// </summary>
        public TTSVolume()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TTSVolume"/> class.
        /// </summary>
        /// <param name="volume">Volume in percent between 0 and 100</param>
        public TTSVolume(int volume)
        {
            this.Volume = volume;
        }

        /// <summary>
        /// Gets or sets the volume in percent between 0 and 100.
        /// </summary>
        public int Volume { get; set; }
    }
}