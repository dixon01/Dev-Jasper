// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayCalculator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DelayCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    ///   This class is responsible to calculate the driving time (delay/advanced) when bus is in a block drive
    ///   The calculated time will be shown in the block driving screen
    /// </summary>
    internal class DelayCalculator
    {
        private readonly BlockDrive blockDrive;

        private readonly DriveInfo driveInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayCalculator"/> class.
        /// </summary>
        /// <param name="driveInfo">
        /// The drive info.
        /// </param>
        /// <param name="blockDrive">
        /// The block drive.
        /// </param>
        public DelayCalculator(DriveInfo driveInfo, BlockDrive blockDrive)
        {
            this.driveInfo = driveInfo;
            this.blockDrive = blockDrive;
        }

        /// <summary>
        /// Gets the delay in seconds.
        /// Only valid if isDriving is true
        /// Positive: Bus has a delay -> too late
        /// Negative: Bus is in advanced
        /// When a bus leaves a bus station there should be no jump in the GetDelay() return value.
        /// There can be delay jumps from switching drive to bus stop.
        /// Because during the drive we don't know where the bus is. So it's not possible to update the delay...
        /// </summary>
        /// <returns>
        /// The delay in seconds.
        /// </returns>
        public int GetDelay()
        {
            if (this.blockDrive.IsTripValid)
            {
                if (this.blockDrive.IsDriving)
                {
                    if (RemoteEventHandler.CurrentTrip.Stop.Count > this.driveInfo.StopIdCurrent)
                    {
                        if (this.driveInfo.IsAtBusStop)
                        {
                            return this.CalcAtBusStopDelay();
                        }

                        return this.CalcDrivingDelay();
                    }
                }
                else
                {
                    return
                        (int)
                        TimeProvider.Current.Now.Subtract(RemoteEventHandler.CurrentTrip.DateTimeStart).TotalSeconds;
                }
            }

            return 0;
        }

        /// <summary>
        ///   Calculates the driving time
        ///   For details/documentation, please check Documentation directory!
        /// </summary>
        /// <returns>
        /// The delay.
        /// </returns>
        private int CalcDrivingDelay()
        {
            DateTime stationA = RemoteEventHandler.CurrentTrip.Stop.Get(this.driveInfo.StopIdLeft).DateTimetoLeave;

            // Station N
            ////DateTime dtStationA = RemoteEventHandler.CurrentTrip.Stop[driveInfo.StopIDLeft].DateTimetoLeave;
            // Station N
            DateTime stationB = RemoteEventHandler.CurrentTrip.Stop.Get(this.driveInfo.StopIdCurrent).DateTimetoLeave;

            // Station N+1
            var driveTime = new TimeSpan(stationB.Ticks - stationA.Ticks); // Drive time from Station N to Station N+1
            DateTime now = TimeProvider.Current.Now; // Time Right now
            DateTime departureTime = this.driveInfo.StopLeftTime; // Time when bus has left Station N
            DateTime expectArrival = departureTime.Add(driveTime); // Time when bus should arrive at Station N+1

            var diffT1 = new TimeSpan(expectArrival.Ticks - stationB.Ticks);

            // Difference between Station N+1 and the Arrival time.
            var diffT2 = new TimeSpan(now.Ticks - stationB.Ticks); // Difference between The time now and Station N+1

            // For more details try to find the excelsheet drivingTimeCalc.xls
            return diffT2.Ticks > diffT1.Ticks ? (int)diffT2.TotalSeconds : (int)diffT1.TotalSeconds;
        }

        private int CalcAtBusStopDelay()
        {
            DateTime dt = RemoteEventHandler.CurrentTrip.Stop.Get(this.driveInfo.StopIdCurrent).DateTimetoLeave;
            return (int)TimeProvider.Current.Now.Subtract(dt).TotalSeconds;
        }
    }
}