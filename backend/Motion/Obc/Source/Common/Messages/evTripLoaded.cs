// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evTripLoaded.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evTripLoaded type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The trip loaded event.
    /// </summary>
    public class evTripLoaded
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evTripLoaded"/> class.
        /// </summary>
        public evTripLoaded()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTripLoaded"/> class.
        /// </summary>
        /// <param name="theoretic">
        /// The theoretic flag.
        /// </param>
        public evTripLoaded(bool theoretic)
        {
            this.Theoretic = theoretic;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the loaded Trip is
        /// Theoretic (according to timetable only) or Real (according to timetable and position)
        /// </summary>
        public bool Theoretic { get; set; }
    }
}