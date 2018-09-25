// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evBUSStopLeft.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evBUSStopLeft type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The bus stop left event.
    /// </summary>
    public class evBUSStopLeft : evBUSStop
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStopLeft"/> class.
        /// </summary>
        public evBUSStopLeft()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evBUSStopLeft"/> class.
        /// </summary>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        public evBUSStopLeft(int stopId)
            : base(stopId)
        {
        }
    }
}