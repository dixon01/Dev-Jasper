// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySetSharpnessArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySetSharpnessArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>displaySetSharpness</code> method.
    /// </summary>
    internal class DisplaySetSharpnessArgs : DisplayValueArgsBase
    {
        private int sharpness;

        /// <summary>
        /// Gets or sets the sharpness.
        /// </summary>
        public int Sharpness
        {
            get
            {
                return this.sharpness;
            }

            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..100 allowed");
                }

                this.sharpness = value;
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
            return string.Format("{0}, Sharpness={1}", base.ToString(), this.Sharpness);
        }
    }
}