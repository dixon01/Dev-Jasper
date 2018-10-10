// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MgiPortBase.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MgiPortBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Mgi
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Motion.Common.Mgi.IO;

    using NLog;

    /// <summary>
    /// Base class for all MGI <see cref="IPort"/> implementations.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value of this port.
    /// </typeparam>
    internal abstract partial class MgiPortBase<T> : SimplePort
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private bool updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="MgiPortBase{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        /// <param name="canRead">
        /// A value indicating whether this port can be read.
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
        protected MgiPortBase(string name, bool canRead, bool canWrite, ValuesBase validValues, IOValue initialValue)
            : base(name, canRead, canWrite, validValues, initialValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MgiPortBase{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the port.
        /// </param>
        /// <param name="canRead">
        /// A value indicating whether this port can be read.
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
        protected MgiPortBase(string name, bool canRead, bool canWrite, ValuesBase validValues, int initialValue)
            : base(name, canRead, canWrite, validValues, initialValue)
        {
        }

        /// <summary>
        /// Updates the value of the GIOoM port without setting the value to the
        /// <see cref="IOBase"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void UpdateValue(T value)
        {
            this.updating = true;
            try
            {
                this.Value = this.ToIOValue(value);
            }
            finally
            {
                this.updating = false;
            }
        }

        /// <summary>
        /// Sets the underlying hardware port to the given value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetPort(T value)
        {
            this.Value = this.ToIOValue(value);
        }

        /// <summary>
        /// Converts the value of the port to an <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The original value.
        /// </param>
        /// <returns>
        /// The converted <see cref="IOValue"/>.
        /// </returns>
        protected abstract IOValue ToIOValue(T value);

        /// <summary>
        /// Updates the port with the given <see cref="IOValue"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be set to the port.
        /// </param>
        protected abstract void UpdateIO(IOValue value);

        /// <summary>
        /// Raises the <see cref="PortBase.ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseValueChanged(EventArgs e)
        {
            if (!this.updating)
            {
                this.UpdateIO(this.Value);
            }

            this.logger.Info("Port '{0}' changed to value {1}", this.Info.Name, this.Value);
            base.RaiseValueChanged(e);
        }
    }
}