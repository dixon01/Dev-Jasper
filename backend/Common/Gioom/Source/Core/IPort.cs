// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPort.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Interface for an I/O port.
    /// </summary>
    public interface IPort : IDisposable
    {
        /// <summary>
        /// Event that is fired when the <see cref="Value"/> changes.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Gets the information about this port.
        /// </summary>
        IPortInfo Info { get; }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// Setting the value might not immediately change the value
        /// returned by the getter. Especially if the port is on a remote
        /// Medi node, it might take some time until the value actually changes.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the port is not readable (in the getter) or writable (in the setter).
        /// </exception>
        IOValue Value { get; set; }

        /// <summary>
        /// Gets or sets the integer value.
        /// This is a shortcut around calling <see cref="Value"/> directly
        /// with an object created from <see cref="CreateValue"/>.
        /// </summary>
        int IntegerValue { get; set; }

        /// <summary>
        /// Creates an I/O value that can be used with this port (and this port only!).
        /// </summary>
        /// <param name="value">
        /// The integer value to be converted into an <see cref="IOValue"/>.
        /// </param>
        /// <returns>
        /// The resulting <see cref="IOValue"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <see cref="value"/> is outside the allowed range of I/O values for this port.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <see cref="value"/> is not a I/O valid value for this port.
        /// </exception>
        IOValue CreateValue(int value);
    }
}