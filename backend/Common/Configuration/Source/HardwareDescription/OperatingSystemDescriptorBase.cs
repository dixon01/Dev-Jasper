// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperatingSystemDescriptorBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OperatingSystemDescriptorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for all operating system descriptions.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(WindowsEmbeddedDescriptor))]
    [XmlInclude(typeof(LinuxEmbeddedDescriptor))]
    [XmlInclude(typeof(MicroControllerDescriptor))]
    public abstract class OperatingSystemDescriptorBase
    {
        /// <summary>
        /// Gets or sets the operating system version.
        /// </summary>
        [XmlAttribute]
        public OperatingSystemVersion Version { get; set; }
    }
}