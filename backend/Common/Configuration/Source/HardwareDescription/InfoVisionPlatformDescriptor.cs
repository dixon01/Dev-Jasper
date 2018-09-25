// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoVisionPlatformDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfoVisionPlatformDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The hardware platform descriptor for the Gorba InfoVision products.
    /// </summary>
    [Serializable]
    public class InfoVisionPlatformDescriptor : PcPlatformDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoVisionPlatformDescriptor"/> class.
        /// </summary>
        public InfoVisionPlatformDescriptor()
        {
            this.Transceivers = new List<MultiProtocolTransceiverDescriptor>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the hardware has a shared RS-485 port.
        /// </summary>
        public bool HasSharedRs485Port { get; set; }

        /// <summary>
        /// Gets or sets the list of all multi-protocol transceivers.
        /// </summary>
        public List<MultiProtocolTransceiverDescriptor> Transceivers { get; set; }
    }
}