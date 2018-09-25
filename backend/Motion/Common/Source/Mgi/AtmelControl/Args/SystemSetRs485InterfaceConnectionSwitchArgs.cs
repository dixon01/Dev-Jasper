// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSetRs485InterfaceConnectionSwitchArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemSetRs485InterfaceConnectionSwitchArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>systemSetRS485InterfaceConnectionSwitch</code> method.
    /// </summary>
    public class SystemSetRs485InterfaceConnectionSwitchArgs
    {
        private int value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the value is outside the allowed range of 0..1.
        /// </exception>
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..15 allowed");
                }

                this.value = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Value={0}", this.Value);
        }
    }
}