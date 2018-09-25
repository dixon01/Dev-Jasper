// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIbisContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIbisContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using Gorba.Common.Configuration.Obc.Ibis;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Context interface used by telegram providers.
    /// </summary>
    public interface IIbisContext
    {
        /// <summary>
        /// Gets the entire IBIS configuration.
        /// </summary>
        IbisConfig Config { get; }

        /// <summary>
        /// Gets the current line number (used for DS001).
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the current Didok number (used for DS004b).
        /// </summary>
        int Didok { get; }

        /// <summary>
        /// Gets the current stop name for the ticket printer (used for DS004c).
        /// </summary>
        string DruckName { get; }

        /// <summary>
        /// Gets the current route number (used for DS002).
        /// </summary>
        int RouteId { get; }

        /// <summary>
        /// Gets the current destination code (used for DS003).
        /// </summary>
        int Destination { get; }

        /// <summary>
        /// Gets the current razzia code (used for DS004a).
        /// </summary>
        int RazziaCode { get; }

        /// <summary>
        /// Gets the current zone (used for DS004).
        /// </summary>
        int CurrentZone { get; }

        /// <summary>
        /// Gets a value indicating whether is inside the stop buffer.
        /// </summary>
        bool IsInStopBuffer { get; }

        /// <summary>
        /// Gets a value indicating whether is door cycled.
        /// This is true between closing of the door (in the buffer) and exiting the buffer.
        /// </summary>
        bool IsDoorCycled { get; }

        /// <summary>
        /// Gets a value indicating whether the bus is driving.
        /// </summary>
        bool IsTheBusDriving { get; }

        /// <summary>
        /// Gets a string representing all names of the given stop (used for DS021b).
        /// </summary>
        /// <param name="stop">
        /// The stop.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetStopNames(BusStop stop);

        /// <summary>
        /// Gets the connection hints for a given stop index (used for DS021b).
        /// </summary>
        /// <param name="stopIndex">
        /// The stop index.
        /// </param>
        /// <returns>
        /// The connection hints as a string.
        /// </returns>
        string GetConnectionHints(int stopIndex);
    }
}
