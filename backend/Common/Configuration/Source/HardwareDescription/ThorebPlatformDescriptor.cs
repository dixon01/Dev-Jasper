// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThorebPlatformDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ThorebPlatformDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;

    /// <summary>
    /// The hardware platform descriptor for the Thoreb products.
    /// </summary>
    [Serializable]
    public class ThorebPlatformDescriptor : PcPlatformDescriptorBase
    {
        /// <summary>
        /// Gets or sets the GPS.
        /// </summary>
        public GpsConfigFake Gps { get; set; }
    }
}