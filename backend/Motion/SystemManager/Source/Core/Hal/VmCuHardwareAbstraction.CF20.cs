// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VmCuHardwareAbstraction.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GorbaHardwareAbstraction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.SystemManager.Core.Watchdog;

    using NLog;

    using OpenNETCF.WindowsCE;

    /// <summary>
    /// The HAL for Gorba Topbox.
    /// </summary>
    public partial class VmCuHardwareAbstraction : HardwareAbstractionBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<VmCuHardwareAbstraction>();

        private string serialNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="VmCuHardwareAbstraction"/> class.
        /// </summary>
        public VmCuHardwareAbstraction()
        {
            this.Watchdog = new VmCuWatchdogController();

            try
            {
                this.serialNumber = DeviceManagement.GetDeviceGuid().ToString();
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't get the device ID", ex);
            }
        }

        /// <summary>
        /// Gets the serial number or null if it is unknown.
        /// </summary>
        public override string SerialNumber
        {
            get
            {
                return this.serialNumber;
            }
        }
    }
}