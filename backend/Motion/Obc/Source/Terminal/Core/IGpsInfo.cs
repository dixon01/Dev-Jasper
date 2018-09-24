// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGpsInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IGpsInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The GPS information interface.
    /// </summary>
    public interface IGpsInfo
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets a value indicating whether GPS is valid.
        /// </summary>
        bool IsGpsValid { get; }

        // [wes] todo: do we really need all those Sxxx properties?

        /// <summary>
        /// Gets the GPS validity string (used for display).
        /// </summary>
        string SIsGpsValid { get; }

        /// <summary>
        /// Gets the X coordinate string (used for display).
        /// </summary>
        string SXCoordinate { get; }

        /// <summary>
        /// Gets the Y coordinate string (used for display).
        /// </summary>
        string SYCoordinate { get; }

        /// <summary>
        /// Gets the direction string (used for display).
        /// </summary>
        string SDirection { get; }

        // ReSharper restore InconsistentNaming
    }
}