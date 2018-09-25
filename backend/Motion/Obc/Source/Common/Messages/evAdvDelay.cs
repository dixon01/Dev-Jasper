// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evAdvDelay.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evAdvDelay type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The advance or delay event.
    /// </summary>
    public class evAdvDelay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evAdvDelay"/> class.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        public evAdvDelay(int delay)
        {
            this.Delay = delay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evAdvDelay"/> class.
        /// </summary>
        public evAdvDelay()
        {
            this.Delay = 0;
        }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        public int Delay { get; set; }
    }
}