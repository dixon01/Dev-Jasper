// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayAdapterDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayAdapterDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The descriptor for a display adapter (graphics output).
    /// </summary>
    [Serializable]
    public class DisplayAdapterDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAdapterDescriptor"/> class.
        /// </summary>
        public DisplayAdapterDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAdapterDescriptor"/> class.
        /// </summary>
        /// <param name="index">
        /// The <see cref="Index"/>.
        /// </param>
        /// <param name="connection">
        /// The connection type.
        /// </param>
        public DisplayAdapterDescriptor(int index, DisplayConnectionType connection)
        {
            this.Index = index;
            this.Connection = connection;
        }

        /// <summary>
        /// Gets or sets the adapter index in the system.
        /// This index is from 0 (contrary to HMW configuration where it is from 1).
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the type of the connection this adapter has with the display.
        /// </summary>
        [XmlAttribute]
        public DisplayConnectionType Connection { get; set; }
    }
}