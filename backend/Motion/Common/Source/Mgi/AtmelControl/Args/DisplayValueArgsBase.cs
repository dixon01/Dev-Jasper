// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayValueArgsBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayValueArgsBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.Args
{
    /// <summary>
    /// Base class for all arguments to methods that change something on a display.
    /// </summary>
    internal abstract class DisplayValueArgsBase
    {
        /// <summary>
        /// Gets or sets the panel address.
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the panel number.
        /// </summary>
        public int PanelNo { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Address={0}, PanelNo={1}", this.Address, this.PanelNo);
        }
    }
}