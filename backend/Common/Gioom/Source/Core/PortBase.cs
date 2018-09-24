// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Simple base class that implements <see cref="IPort"/>.
    /// </summary>
    public abstract class PortBase : IPort
    {
        /// <summary>
        /// Event that is fired when the <see cref="IPort.Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the information about this port.
        /// </summary>
        public abstract IPortInfo Info { get; }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// Setting the value might not immediately change the value
        /// returned by the getter. Especially if the port is on a remote
        /// Medi node, it might take some time until the value actually changes.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the port is not readable (in the getter) or writable (in the setter).
        /// </exception>
        public abstract IOValue Value { get; set; }

        /// <summary>
        /// Gets or sets the integer value.
        /// This is a shortcut around calling <see cref="IPort.Value"/> directly
        /// with an object created from <see cref="IPort.CreateValue"/>.
        /// </summary>
        public int IntegerValue
        {
            get
            {
                return this.Value.Value;
            }

            set
            {
                this.Value = this.CreateValue(value);
            }
        }

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
        public IOValue CreateValue(int value)
        {
            return this.Info.ValidValues.CreateValue(value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseValueChanged(EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}