// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfovisionSystemState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfovisionSystemState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The Atmel Controller system state.
    /// </summary>
    public class InfovisionSystemState : AtmelControlObject
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the serial number of the device.
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        /// Gets or sets the hardware reference.
        /// Format:
        /// {Hardware board revision};{Device type};{Default resolution}
        /// </summary>
        public string HWRef { get; set; }

        /// <summary>
        /// Gets or sets the AT91 Controller version information.
        /// </summary>
        public string At91Version { get; set; }

        /// <summary>
        /// Gets or sets the revision of the AT91 Controller firmware.
        /// </summary>
        public string At91Rev { get; set; }

        /// <summary>
        /// Gets or sets the fan speed of the device in RPM.
        /// This value is unused on PC-2.
        /// </summary>
        public int? FanSpeedRPM { get; set; }

        /// <summary>
        /// Gets or sets the temperature the device.
        /// This value is unused on PC-2.
        /// </summary>
        public int? Temperature { get; set; }

        // ReSharper restore InconsistentNaming
    }
}