// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySetBacklightValueArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySetBacklightValueArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>displaySetBacklightValue</code> method.
    /// </summary>
    internal class DisplaySetBacklightValueArgs : DisplayValueArgsBase
    {
        private int backlightValue;

        /// <summary>
        /// Gets or sets the backlight value.
        /// </summary>
        public int BacklightValue
        {
            get
            {
                return this.backlightValue;
            }

            set
            {
                if (value < -1 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values -1..255 allowed");
                }

                this.backlightValue = value;
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
            return string.Format("{0}, BacklightValue={1}", base.ToString(), this.BacklightValue);
        }
    }
}