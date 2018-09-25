// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisIpTimeSyncController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisIpTimeSyncController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Vdv301
{
    using Gorba.Common.Configuration.HardwareManager.Vdv301;
    using Gorba.Motion.HardwareManager.Core.Common;
    using Gorba.Motion.HardwareManager.Core.TimeSync;

    /// <summary>
    /// The time sync controller for IBIS-IP.
    /// </summary>
    public class IbisIpTimeSyncController : SntpTimeSyncControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIpTimeSyncController"/> class.
        /// </summary>
        /// <param name="address">
        /// The IP address of the SNTP server.
        /// </param>
        /// <param name="port">
        /// The UDP port of the SNTP server.
        /// </param>
        /// <param name="config">
        /// The time sync config.
        /// </param>
        /// <param name="systemTimeOutput">
        /// The system time port to be used to update the time.
        /// </param>
        public IbisIpTimeSyncController(
            string address, int port, TimeSyncConfig config, SystemTimeOutput systemTimeOutput)
            : base(address, port, config, systemTimeOutput)
        {
        }
    }
}