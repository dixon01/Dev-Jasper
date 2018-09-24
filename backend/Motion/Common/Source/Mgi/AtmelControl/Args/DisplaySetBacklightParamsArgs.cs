// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySetBacklightParamsArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySetBacklightParamsArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>displaySetBacklightParams</code> method.
    /// </summary>
    internal class DisplaySetBacklightParamsArgs : DisplayValueArgsBase
    {
        private int minimum;

        private int maximum;

        private int speed;

        /// <summary>
        /// Gets or sets the minimum backlight value.
        /// </summary>
        public int Minimum
        {
            get
            {
                return this.minimum;
            }

            set
            {
                if (value < 1 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 1..255 allowed");
                }

                this.minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum backlight value.
        /// </summary>
        public int Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                if (value < 1 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 1..255 allowed");
                }

                this.maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed of backlight regulation
        /// 1 – slow (~ 1 minute)
        /// 10 – fast (instantly)
        /// </summary>
        public int Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                if (value < 1 || value > 10)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 1..10 allowed");
                }

                this.speed = value;
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
            return string.Format(
                "{0}, Minimum={1}, Maximum={2}, Speed={3}", base.ToString(), this.Minimum, this.Maximum, this.Speed);
        }
    }
}