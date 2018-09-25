// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    /// <summary>
    /// Screen item representing an image.
    /// </summary>
    public partial class ImageItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Image: \"{0}\" @ [{1},{2}]", this.Filename, this.X, this.Y);
        }
    }
}