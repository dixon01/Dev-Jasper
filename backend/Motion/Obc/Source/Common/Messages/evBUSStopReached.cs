// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evBUSStopReached.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evBUSStopReached type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The bus stop reached event.
    /// </summary>
    public class evBUSStopReached : evBUSStop
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStopReached"/> class.
        /// </summary>
        public evBUSStopReached()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStopReached"/> class.
        /// </summary>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        public evBUSStopReached(int stopId)
            : base(stopId)
        {
        }
    }
}