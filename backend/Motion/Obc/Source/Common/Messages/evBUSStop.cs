// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evBUSStop.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evBUSStop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The bus stop event.
    /// </summary>
    public abstract class evBUSStop
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStop"/> class.
        /// </summary>
        protected evBUSStop()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStop"/> class.
        /// </summary>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        protected evBUSStop(int stopId)
        {
            this.StopId = stopId;
        }

        public int StopId { get; set; }
    }
}