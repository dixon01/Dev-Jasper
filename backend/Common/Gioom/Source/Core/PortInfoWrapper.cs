// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortInfoWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortInfoWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using Gorba.Common.Gioom.Core.Messages;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Internal class to convert a <see cref="PortInfo"/> (message) to an <see cref="IPortInfo"/>.
    /// </summary>
    internal class PortInfoWrapper : IPortInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortInfoWrapper"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="source">
        /// The source of the message.
        /// </param>
        public PortInfoWrapper(PortInfo info, MediAddress source)
        {
            this.Name = info.Name;
            this.Address = source;
            this.CanRead = info.CanRead;
            this.CanWrite = info.CanWrite;
            this.ValidValues = info.ValidValues.ToValues();
            this.InitialValue = this.ValidValues.CreateValue(info.Value);
        }

        /// <summary>
        /// Gets the name of the port.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the address where the port resides.
        /// </summary>
        public MediAddress Address { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this port can be read.
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this port can be written.
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary>
        /// Gets the valid values of this port.
        /// </summary>
        public ValuesBase ValidValues { get; private set; }

        /// <summary>
        /// Gets the initial value of this port at the moment when
        /// the constructor was called.
        /// </summary>
        public IOValue InitialValue { get; private set; }
    }
}