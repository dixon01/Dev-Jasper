// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Font.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System.Drawing;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Contains all properties needed to create a DirectX font.
    /// </summary>
    public partial class Font
    {
        /// <summary>
        /// Gets the <see cref="System.Drawing.Color"/> for this
        /// <see cref="Color"/> string.
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

            Color color;
            return ParserUtil.TryParseColor(this.Color, out color) ? color : System.Drawing.Color.Black;
        }

        /// <summary>
        /// Gets the <see cref="System.Drawing.Color"/> for this <see cref="OutlineColor"/> string.
        /// </summary>
        /// <returns>
        /// the converted outline color or null if it was not defined (null or empty).
        /// </returns>
        public Color? GetOutlineColor()
        {
            if (string.IsNullOrEmpty(this.OutlineColor))
            {
                return null;
            }

            Color color;
            return ParserUtil.TryParseColor(this.OutlineColor, out color) ? color : (Color?)null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "{0} ({1}) W={2} I={3} C={4}", this.Face, this.Height, this.Weight, this.Italic, this.Color);
        }
    }
}