// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeparturesExpiredEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System;

    /// <summary>
    /// Event fired whenever a set of departures expires.
    /// </summary>
    public class DeparturesExpiredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeparturesExpiredEventArgs"/> class.
        /// </summary>
        /// <param name="departures">The departures that are expired.</param>
        public DeparturesExpiredEventArgs(DeparturesConfig departures)
        {
            this.Departures = departures;
        }

        /// <summary>
        /// Gets the set of departures that are expired.
        /// </summary>
        public DeparturesConfig Departures { get; private set; }
    }
}
