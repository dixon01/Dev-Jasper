// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointSubTypeStop.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointSubTypeStop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The type of stop.
    /// </summary>
    public enum PointSubTypeStop
    {
        /// <summary>
        /// A normal stop.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// A stop with an iqube.
        /// </summary>
        Iqube = 2
    }
}