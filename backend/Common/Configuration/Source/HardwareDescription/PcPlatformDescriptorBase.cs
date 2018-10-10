// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PcPlatformDescriptorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PcPlatformDescriptorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for all hardware platform descriptions of a PC running a "regular" OS.
    /// </summary>
    [Serializable]
    public abstract class PcPlatformDescriptorBase : PlatformDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcPlatformDescriptorBase"/> class.
        /// </summary>
        protected PcPlatformDescriptorBase()
        {
            this.DisplayAdapters = new List<DisplayAdapterDescriptor>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this device has a generic button input.
        /// </summary>
        public bool HasGenericButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this device has a generic LED output.
        /// </summary>
        public bool HasGenericLed { get; set; }

        /// <summary>
        /// Gets or sets the list of display adapters (graphics card outputs).
        /// </summary>
        public List<DisplayAdapterDescriptor> DisplayAdapters { get; set; }

        /// <summary>
        /// Gets or sets the built-in screen.
        /// This can be null if the device doesn't have a built-in screen.
        /// </summary>
        public DisplayDescriptor BuiltInScreen { get; set; }
    }
}