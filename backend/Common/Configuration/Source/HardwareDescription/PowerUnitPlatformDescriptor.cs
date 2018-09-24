// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerUnitPlatformDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The hardware platform descriptor for the Gorba iqube power units.
    /// </summary>
    [Serializable]
    public class PowerUnitPlatformDescriptor : PlatformDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerUnitPlatformDescriptor"/> class.
        /// </summary>
        public PowerUnitPlatformDescriptor()
        {
            this.DisplayUnits = new List<DisplayUnitDescriptor>();
        }

        /// <summary>
        /// Gets or sets the list of display adapters (connected e-paper units).
        /// </summary>
        public List<DisplayUnitDescriptor> DisplayUnits { get; set; }

        /// <summary>
        /// Gets or sets the power type.
        /// </summary>
        public PowerType PowerType { get; set; }
    }
}
