// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPortInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPortInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Information about an <see cref="IPort"/>.
    /// </summary>
    public interface IPortInfo
    {
        /// <summary>
        /// Gets the name of the port.
        /// The name has to be unique within the local application.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the address where the port resides.
        /// <seealso cref="MessageDispatcher.LocalAddress"/>
        /// </summary>
        MediAddress Address { get; }

        /// <summary>
        /// Gets a value indicating whether this port can be read.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Gets a value indicating whether this port can be written.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets the valid values of this port.
        /// </summary>
        ValuesBase ValidValues { get; }
    }
}