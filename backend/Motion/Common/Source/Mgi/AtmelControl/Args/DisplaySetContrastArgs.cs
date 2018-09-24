// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySetContrastArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySetContrastArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    using System;

    /// <summary>
    /// Arguments for the <code>displaySetContrast</code> method.
    /// </summary>
    internal class DisplaySetContrastArgs : DisplayValueArgsBase
    {
        private int contrast;

        /// <summary>
        /// Gets or sets the contrast.
        /// </summary>
        public int Contrast
        {
            get
            {
                return this.contrast;
            }

            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Values 0..100 allowed");
                }

                this.contrast = value;
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
            return string.Format("{0}, Contrast={1}", base.ToString(), this.Contrast);
        }
    }
}