// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlatformDescriptorBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlatformDescriptorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all hardware platform descriptions.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(InformPlatformDescriptor))]
    [XmlInclude(typeof(InfoVisionPlatformDescriptor))]
    [XmlInclude(typeof(ThorebPlatformDescriptor))]
    [XmlInclude(typeof(PowerUnitPlatformDescriptor))]
#if __UseLuminatorTftDisplay
    [XmlInclude(typeof(InfoTransitPlatformDescriptor))]
#endif
    public abstract class PlatformDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformDescriptorBase"/> class.
        /// </summary>
        protected PlatformDescriptorBase()
        {
            this.Inputs = new List<InputDescriptor>();
            this.Outputs = new List<OutputDescriptor>();
            this.SerialPorts = new List<SerialPortDescriptor>();
        }

        /// <summary>
        /// Gets or sets the list of digital inputs.
        /// </summary>
        public List<InputDescriptor> Inputs { get; set; }

        /// <summary>
        /// Gets or sets the list of digital outputs.
        /// </summary>
        public List<OutputDescriptor> Outputs { get; set; }

        /// <summary>
        /// Gets or sets the list of serial ports.
        /// </summary>
        public List<SerialPortDescriptor> SerialPorts { get; set; }
    }
}