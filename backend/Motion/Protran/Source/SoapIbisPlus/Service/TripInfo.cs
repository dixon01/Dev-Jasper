// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TripInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using MFD.MFDCustomerService;

    /// <summary>
    /// The trip information.
    /// </summary>
    internal class TripInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TripInfo"/> class.
        /// </summary>
        /// <param name="tripIdx">
        /// The trip index.
        /// </param>
        /// <param name="patternIdx">
        /// The pattern index.
        /// </param>
        /// <param name="routeNo">
        /// The route number.
        /// </param>
        /// <param name="runNo">
        /// The run number.
        /// </param>
        /// <param name="blockNo">
        /// The block number.
        /// </param>
        /// <param name="stopPoints">
        /// The stop points.
        /// </param>
        /// <param name="direction">
        /// The direction.
        /// </param>
        public TripInfo(
            int tripIdx, int patternIdx, int routeNo, int runNo, int blockNo, StopInfoType[] stopPoints, int direction)
        {
            this.TripIdx = tripIdx;
            this.PatternIdx = patternIdx;
            this.RouteNo = routeNo;
            this.RunNo = runNo;
            this.BlockNo = blockNo;
            this.StopPoints = stopPoints;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets or sets the trip index.
        /// </summary>
        public int TripIdx { get; set; }

        /// <summary>
        /// Gets or sets the pattern index.
        /// </summary>
        public int PatternIdx { get; set; }

        /// <summary>
        /// Gets or sets the route number.
        /// </summary>
        public int RouteNo { get; set; }

        /// <summary>
        /// Gets or sets the run number.
        /// </summary>
        public int RunNo { get; set; }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public int BlockNo { get; set; }

        /// <summary>
        /// Gets or sets the stop points.
        /// </summary>
        public StopInfoType[] StopPoints { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        public int Direction { get; set; }
    }
}