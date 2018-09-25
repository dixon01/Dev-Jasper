// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System.Text;

    /// <summary>
    /// List of images used for example to show connecting bus lines / trains.
    /// </summary>
    public partial class ImageListItem
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = (ImageListItem)base.Clone();
            clone.images = (string[])this.Images.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ImageList: ");
            foreach (var image in this.Images)
            {
                sb.Append(image).Append(";");
            }

            sb.Length--;
            sb.Append(" @ [").Append(this.X).Append(",").Append(this.Y).Append("]");
            return sb.ToString();
        }
    }
}
