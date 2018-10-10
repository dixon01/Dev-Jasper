// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DviLevelShifterConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the level shifter
    /// </summary>
    [Serializable]
    public class DviLevelShifterConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DviLevelShifterConfig"/> class.
        /// </summary>
        public DviLevelShifterConfig()
        {
            this.Index = -1;
            this.Trim = TrimOptions.StandardCurrent;
            this.OutputLevel = 0;
        }

        /// <summary>
        /// Gets or sets the index of the level shifter.
        /// -1 means disabled.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(-1)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the trim option.
        /// </summary>
        public TrimOptions Trim { get; set; }

        /// <summary>
        /// Gets or sets the output level config.
        /// </summary>
        public int OutputLevel { get; set; }
    }
}
