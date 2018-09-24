// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The description of a serial port.
    /// </summary>
    public class SerialPortDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortDescriptor"/> class.
        /// </summary>
        public SerialPortDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortDescriptor"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isDefaultRs485">
        /// A value indicating whether this port is the default port for RS-485.
        /// </param>
        public SerialPortDescriptor(string name, bool isDefaultRs485 = false)
        {
            this.Name = name;
            this.IsDefaultRs485 = isDefaultRs485;
        }

        /// <summary>
        /// Gets or sets the name of the COM port in Windows.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this port is the default port for RS-485.
        /// </summary>
        [XmlAttribute("IsDefaultRS485")]
        [DefaultValue(false)]
        public bool IsDefaultRs485 { get; set; }
    }
}