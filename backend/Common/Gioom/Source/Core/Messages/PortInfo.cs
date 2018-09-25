// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Information about a port.
    /// </summary>
    public class PortInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this port can be read.
        /// </summary>
        public bool CanRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this port can be written.
        /// </summary>
        public bool CanWrite { get; set; }

        /// <summary>
        /// Gets or sets the valid values of this port.
        /// </summary>
        public ValuesInfoBase ValidValues { get; set; }

        /// <summary>
        /// Gets or sets the current value of this port.
        /// </summary>
        public int Value { get; set; }
    }
}