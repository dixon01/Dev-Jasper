// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfovisionDisplayDevice.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfovisionDisplayDevice type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// Part of <see cref="InfovisionDisplayState"/> describing a single display.
    /// </summary>
    public class InfovisionDisplayDevice
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the RS485 bus address of the attached device.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the connection state.
        /// </summary>
        public DisplayConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Gets or sets the ignition state.
        /// Valid values: 0 = off, 1 = on
        /// </summary>
        public int Ignition_On { get; set; }

        /// <summary>
        /// Gets or sets the power system power hold state. If ignition is off this
        /// indicates that the system is still running on purpose.
        /// </summary>
        public int PowerHold_On { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that power (voltage and current) are within spec limits.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int PowerState { get; set; }

        /// <summary>
        /// Gets or sets if the external backlight 1 is ok.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int BacklightExternal_1_OK { get; set; }

        /// <summary>
        /// Gets or sets if the external backlight 2 is ok.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int BacklightExternal_2_OK { get; set; }

        /// <summary>
        /// Gets or sets if the internal backlight 1 is ok.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int BacklightInternal_1_OK { get; set; }

        /// <summary>
        /// Gets or sets if the internal backlight 2 is ok.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int BacklightInternal_2_OK { get; set; }

        /// <summary>
        /// Gets or sets if the backlight 24V is ok.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int Backlight24V_OK { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that genesis display controller
        /// for panel 0 has been discovered.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int GenesisPresent_1 { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that genesis display controller
        /// for panel 1 has been discovered.
        /// Valid values: 0 = false, 1 = true
        /// </summary>
        public int GenesisPresent_2 { get; set; }

        /// <summary>
        /// Gets or sets the backlight mode for panel 0.
        /// Valid values: 0 = manual, 1 = automatic
        /// </summary>
        public int BacklightMode_1 { get; set; }

        /// <summary>
        /// Gets or sets the backlight mode for panel 1.
        /// Valid values: 0 = manual, 1 = automatic
        /// </summary>
        public int BacklightMode_2 { get; set; }

        /// <summary>
        /// Gets or sets the information about all panels.
        /// One for each controller.
        /// </summary>
        public InfovisionDisplayPanel[] Panel { get; set; }

        // ReSharper restore InconsistentNaming
    }
}