// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSetAddressArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisSetAddressArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>ibisSetAddress</code> method.
    /// </summary>
    public class IbisSetAddressArgs
    {
        private int address;

        /// <summary>
        /// Gets or sets the IBIS address.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the value is outside the allowed range of 1..15.
        /// </exception>
        public int Address
        {
            get
            {
                return this.address;
            }

            set
            {
                if (value < 1 || value > 15)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 1..15 allowed");
                }

                this.address = value;
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
            return string.Format("Address={0}", this.Address);
        }
    }
}