// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimplePort.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimplePort type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// A simple implementation of <see cref="IPort"/> that can be used to
    /// register a port with <see cref="GioomClient"/>.
    /// Simply call <see cref="set_Value"/> to change the value of the port
    /// and listen to <see cref="PortBase.ValueChanged"/> to update your port.
    /// </summary>
    public class SimplePort : PortBase, IPortInfo
    {
        private readonly string name;

        private readonly bool canRead;

        private readonly bool canWrite;

        private readonly ValuesBase validValues;

        private IOValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePort"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        /// <param name="canRead">
        /// A value indicating whether this port can be read.
        /// This has no effect on getting the value through <see cref="get_Value"/>
        /// but will be communicated to classes using <see cref="GioomClient"/> to
        /// access this port.
        /// </param>
        /// <param name="canWrite">
        /// A value indicating whether this port can be written.
        /// </param>
        /// <param name="validValues">
        /// The valid values.
        /// </param>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        public SimplePort(string name, bool canRead, bool canWrite, ValuesBase validValues, int initialValue)
            : this(name, canRead, canWrite, validValues, validValues.CreateValue(initialValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePort"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        /// <param name="canRead">
        /// A value indicating whether this port can be read.
        /// This has no effect on getting the value through <see cref="get_Value"/>
        /// but will be communicated to classes using <see cref="GioomClient"/> to
        /// access this port.
        /// </param>
        /// <param name="canWrite">
        /// A value indicating whether this port can be written.
        /// </param>
        /// <param name="validValues">
        /// The valid values.
        /// </param>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        public SimplePort(string name, bool canRead, bool canWrite, ValuesBase validValues, IOValue initialValue)
        {
            this.name = name;
            this.canRead = canRead;
            this.canWrite = canWrite;
            this.validValues = validValues;

            this.value = initialValue;
        }

        /// <summary>
        /// Gets the information about this port.
        /// </summary>
        public override IPortInfo Info
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// Setting the value might not immediately change the value
        /// returned by the getter. Especially if the port is on a remote
        /// Medi node, it might take some time until the value actually changes.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the port is not readable (in the getter) or writable (in the setter).
        /// </exception>
        public override IOValue Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value != null && this.value.Value == value.Value)
                {
                    return;
                }

                this.value = value;
                this.RaiseValueChanged(EventArgs.Empty);
            }
        }

        string IPortInfo.Name
        {
            get
            {
                return this.name;
            }
        }

        MediAddress IPortInfo.Address
        {
            get
            {
                return MessageDispatcher.Instance.LocalAddress;
            }
        }

        bool IPortInfo.CanRead
        {
            get
            {
                return this.canRead;
            }
        }

        bool IPortInfo.CanWrite
        {
            get
            {
                return this.canWrite;
            }
        }

        ValuesBase IPortInfo.ValidValues
        {
            get
            {
                return this.validValues;
            }
        }
    }
}