// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceStateValueProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceStateValueProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// Value provider for device state.
    /// </summary>
    public class DeviceStateValueProvider : IDataItemValueProvider
    {
        private DeviceState state;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceStateValueProvider"/> class.
        /// </summary>
        /// <param name="deviceClass">
        /// The device class.
        /// </param>
        /// <param name="deviceNumber">
        /// The device number.
        /// </param>
        /// <param name="state">
        /// The initial state.
        /// </param>
        public DeviceStateValueProvider(DeviceClass deviceClass, int deviceNumber, DeviceState state)
        {
            this.state = state;
            this.DeviceNumber = deviceNumber;
            this.DeviceClass = deviceClass;
        }

        /// <summary>
        /// This event is fired every time the <see cref="IDataItemValueProvider.Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the value from this provider.
        /// </summary>
        public string Value
        {
            get
            {
                return string.Format("{0:D},{1:D},{2:D}", this.DeviceClass, this.DeviceNumber, this.State);
            }
        }

        /// <summary>
        /// Gets the device class.
        /// </summary>
        public DeviceClass DeviceClass { get; private set; }

        /// <summary>
        /// Gets the device number.
        /// </summary>
        public int DeviceNumber { get; private set; }

        /// <summary>
        /// Gets or sets the device state.
        /// </summary>
        public DeviceState State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaiseValueChanged(EventArgs.Empty);
            }
        }

        private void RaiseValueChanged(EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
