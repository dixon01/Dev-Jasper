// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="GpioInfo.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The gpio info.</summary>
    [Serializable]
    public class GpioInfo
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="GpioInfo"/> class.</summary>
        /// <param name="gpio">The gpio.</param>
        /// <param name="active">The active.</param>
        public GpioInfo(PeripheralGpioType gpio, bool active = true, ushort changeMask = 0, ushort gpioStatus = 0)
        {
            this.Gpio = gpio;
            this.Active = active;
            this.ChangeMask = changeMask;
            this.GpioStatus = gpioStatus;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GpioInfo" /> class.
        ///     Default Parameter less constructor
        /// </summary>
        public GpioInfo()
            : this(PeripheralGpioType.None, false)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether active.</summary>
        public bool Active { get; set; }

        public ushort ChangeMask { get; set; }

        public ushort GpioStatus { get; set; }

        /// <summary>Gets or sets the gpio.</summary>
        public PeripheralGpioType Gpio { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Gpio, this.Active);
        }

        #endregion
    }
}