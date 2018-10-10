// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evTTSVolume.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evTTSVolume type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The TTS volume event.
    /// </summary>
    public class evTTSVolume
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evTTSVolume"/> class.
        /// </summary>
        public evTTSVolume()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTTSVolume"/> class.
        /// </summary>
        /// <param name="volume">Volume in percent between 0 and 100</param>
        public evTTSVolume(int volume)
        {
            this.Volume = volume;
        }

        /// <summary>
        /// Gets or sets the volume in percent between 0 and 100.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "TTS Volume: " + this.Volume + "%";
        }
    }
}