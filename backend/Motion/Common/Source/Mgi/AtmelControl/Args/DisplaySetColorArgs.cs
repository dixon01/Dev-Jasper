// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySetColorArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySetColorArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>displaySetColor</code> method.
    /// </summary>
    internal class DisplaySetColorArgs : DisplayValueArgsBase
    {
        private int red;
        private int green;
        private int blue;

        /// <summary>
        /// Gets or sets the red color balance value.
        /// </summary>
        public int Red
        {
            get
            {
                return this.red;
            }

            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..100 allowed");
                }

                this.red = value;
            }
        }

        /// <summary>
        /// Gets or sets the green color balance value.
        /// </summary>
        public int Green
        {
            get
            {
                return this.green;
            }

            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..100 allowed");
                }

                this.green = value;
            }
        }

        /// <summary>
        /// Gets or sets the blue color balance value.
        /// </summary>
        public int Blue
        {
            get
            {
                return this.blue;
            }

            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..100 allowed");
                }

                this.blue = value;
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
            return string.Format("{0}, Red={1}, Green={2}, Blue={3}", base.ToString(), this.Red, this.Green, this.Blue);
        }
    }
}