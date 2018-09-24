// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleStopList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleStopList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Bus.Core.Data;

    /// <summary>
    /// List of <see cref="SimpleStop"/>s.
    /// </summary>
    public class SimpleStopList : StopListBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStopList"/> class.
        /// </summary>
        public SimpleStopList()
        {
            this.Stops = new List<SimpleStop>();
        }

        /// <summary>
        /// Gets the stop list.
        /// </summary>
        public List<SimpleStop> Stops { get; private set; }

        /// <summary>
        /// Gets the point at the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The point.
        /// </returns>
        protected override PointBase GetPoint(int index)
        {
            return this.Stops[index];
        }

        /// <summary>
        /// Gets the number of points in this list.
        /// </summary>
        /// <returns>
        /// The number of points in this list.
        /// </returns>
        protected override int GetPointCount()
        {
            return this.Stops.Count;
        }
    }
}