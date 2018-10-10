// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastVisibleLocation.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LastVisibleLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System.Drawing;

    /// <summary>
    /// The last visible location.
    /// </summary>
    public class LastVisibleLocation
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Rectangle Location { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Last Location [{0},({1})]", this.ItemId, this.Location);
        }
    }
}
