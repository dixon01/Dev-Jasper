// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VolumeInputOutput.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VolumeInputOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// An input/output GIOoM port to get and set the system volume.
    /// </summary>
    public class VolumeInputOutput : SimplePort
    {
        private readonly VolumeWrapperBase volumeWrapper;

        //  warning CS0649: Field 'VolumeInputOutput.value' is never assigned to, and will always have its default value null
        private IOValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeInputOutput"/> class.
        /// </summary>
        public VolumeInputOutput()
            : this(VolumeWrapperBase.Create())
        {
        }

        private VolumeInputOutput(VolumeWrapperBase volumeWrapper)
            : base("SystemVolume", true, true, new IntegerValues(0, 100), volumeWrapper.GetVolume())
        {
            this.volumeWrapper = volumeWrapper;
        }

        /// <summary>
        /// Gets or sets the current I/O value of this port.
        /// </summary>
        public override IOValue Value
        {
            get
            {
                return this.CreateValue(this.volumeWrapper.GetVolume());
            }

            set
            {
                if (this.value != null && this.value.Value == value.Value)
                {
                    return;
                }

                this.volumeWrapper.SetVolume(value.Value);
                this.RaiseValueChanged(EventArgs.Empty);
            }
        }
    }
}
