// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System.Drawing;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Screen item that displays a rectangle filled with a given color.
    /// </summary>
    public partial class RectangleItem
    {
        /// <summary>
        /// Gets the <see cref="System.Drawing.Color"/> for this <see cref="Color"/> string.
        /// </summary>
        /// <returns>
        /// the converted color.
        /// </returns>
        public Color GetColor()
        {
            if (string.IsNullOrEmpty(this.Color))
            {
                return System.Drawing.Color.Black;
            }

            Color c;
            return ParserUtil.TryParseColor(this.Color, out c) ? c : System.Drawing.Color.Black;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Rectangle: [{0},{1};{2}x{3}]", this.X, this.Y, this.Width, this.Height);
        }
    }
}
