// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaHardwareAbstraction.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GorbaHardwareAbstraction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    using Gorba.Motion.SystemManager.Core.Watchdog;

    /// <summary>
    /// The HAL for Gorba Topbox.
    /// </summary>
    public partial class GorbaHardwareAbstraction : HardwareAbstractionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaHardwareAbstraction"/> class.
        /// </summary>
        public GorbaHardwareAbstraction()
        {
            this.Watchdog = new Port80WatchdogController();
        }

        /// <summary>
        /// Gets the serial number or null if it is unknown.
        /// </summary>
        public override string SerialNumber
        {
            get
            {
                return null;
            }
        }
    }
}