// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoDescriptorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IoDescriptorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for all I/O descriptors.
    /// </summary>
    [Serializable]
    public abstract class IoDescriptorBase
    {
        /// <summary>
        /// Gets or sets the internal index of the I/O used by Hardware Manager.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the human readable name of the I/O.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
    }
}