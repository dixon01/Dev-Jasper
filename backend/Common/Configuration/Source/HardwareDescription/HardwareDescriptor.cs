// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The root of the hardware descriptor that contains everything.
    /// </summary>
    [Serializable]
    [XmlRoot("HardwareDescriptor")]
    public class HardwareDescriptor
    {
        /// <summary>
        /// Gets or sets the human readable name of the hardware.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hardware platform description.
        /// </summary>
        public PlatformDescriptorBase Platform { get; set; }

        /// <summary>
        /// Gets or sets the operating system description.
        /// </summary>
        public OperatingSystemDescriptorBase OperatingSystem { get; set; }
    }
}
