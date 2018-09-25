// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BacklightControlRateConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BacklightControlRateConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The backlight control rate configuration.
    /// </summary>
    [Serializable]
    public class BacklightControlRateConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BacklightControlRateConfig"/> class.
        /// </summary>
        public BacklightControlRateConfig()
        {
            this.Minimum = 90;
            this.Maximum = 255;
            this.Speed = 6;
        }

        /// <summary>
        /// Gets or sets the minimum backlight value.
        /// Range is from 1..255
        /// </summary>
        [XmlAttribute]
        public int Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum backlight value.
        /// Range is from 1..255
        /// </summary>
        [XmlAttribute]
        public int Maximum { get; set; }

        /// <summary>
        /// Gets or sets the speed of backlight regulation.
        /// 1 => slow (~~1 minute)
        /// 10 => fast (instantly)
        /// </summary>
        [XmlAttribute]
        public int Speed { get; set; }
    }
}