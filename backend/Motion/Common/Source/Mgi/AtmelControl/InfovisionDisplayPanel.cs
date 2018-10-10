// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfovisionDisplayPanel.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfovisionDisplayPanel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// Part of <see cref="InfovisionDisplayDevice"/> describing a single panel controller.
    /// </summary>
    public class InfovisionDisplayPanel
    {
        /// <summary>
        /// Gets or sets the panel number.
        /// </summary>
        public int PanelNo { get; set; }

        /// <summary>
        /// Gets or sets the internal temperature sensor value.
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// Gets or sets the backlight value.
        /// </summary>
        public int BacklightValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum backlight value.
        /// </summary>
        public int BacklightMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum backlight value.
        /// </summary>
        public int BacklightMax { get; set; }

        /// <summary>
        /// Gets or sets the speed of automatic backlight adjustment.
        /// 1 – slow (~ 1 minute) ... 10 – fast (instantly)
        /// </summary>
        public int BacklightSpeed { get; set; }

        /// <summary>
        /// Gets or sets the current lux value of light sensor.
        /// </summary>
        public int Lux { get; set; }

        /// <summary>
        /// Gets or sets the EQ level in decibel of LVDS signal equalizer.
        /// </summary>
        public int EqLevel { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that signal is stable.
        /// </summary>
        public int SignalStable { get; set; }

        /// <summary>
        /// Gets or sets the flags showing signal quality.
        /// ESTIMATED = BIT9, // mode not in standard mode table
        /// OUT_RANGE = BIT10, // source timing violates board timing restrictions.
        /// </summary>
        public int SignalFlags { get; set; }

        /// <summary>
        /// Gets or sets the contrast value.
        /// This value is only available if the signal is stable.
        /// </summary>
        public int? Contrast { get; set; }

        /// <summary>
        /// Gets or sets the sharpness value.
        /// This value is only available if the signal is stable.
        /// </summary>
        public int? Sharpness { get; set; }

        /// <summary>
        /// Gets or sets the color balance red value.
        /// This value is only available if the signal is stable.
        /// </summary>
        public int? ColorRed { get; set; }

        /// <summary>
        /// Gets or sets the color balance green value.
        /// This value is only available if the signal is stable.
        /// </summary>
        public int? ColorGreen { get; set; }

        /// <summary>
        /// Gets or sets the color balance blue value.
        /// This value is only available if the signal is stable.
        /// </summary>
        public int? ColorBlue { get; set; }
    }
}