// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBlockDrive.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBlockDrive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The block drive field interface.
    /// </summary>
    public interface IBlockDrive : IMainField
    {
        /// <summary>
        ///   Sets the next three stations
        /// </summary>
        /// <param name = "station1">next station</param>
        /// <param name = "station2">next after next station</param>
        /// <param name = "station3">next after next next station</param>
        void SetStations(string station1, string station2, string station3);

        /// <summary>
        ///   The delay which the bus have.
        ///   0: in time
        ///   positive: bus has a delay -> drive faster
        ///   negative: bus is too fast -> drive slower
        /// </summary>
        /// <param name = "delay">delay in seconds</param>
        /// <param name = "isAtBusStop">
        ///   Describes if the bus is at a bus stop/station.
        ///   Default value is true
        ///   true: Bus is currently at a bus stop. Countdown window will be shown
        ///   false: Bus is driving
        /// </param>
        void SetDelaySec(int delay, bool isAtBusStop);
    }
}