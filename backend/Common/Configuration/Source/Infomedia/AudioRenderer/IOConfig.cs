// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    using System;

    /// <summary>
    /// The I/O configuration.
    /// </summary>
    [Serializable]
    public class IOConfig
    {
        /// <summary>
        /// Gets or sets the port to be used to change the speaker volume.
        /// The port has to be an integer value port (0..100).
        /// </summary>
        public IOPortConfig VolumePort { get; set; }
    }
}