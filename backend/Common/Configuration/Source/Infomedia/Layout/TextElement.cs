// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextElement.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Text to be displayed on a layout.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System.Drawing;

    /// <summary>
    /// Text to be displayed on a layout.
    /// </summary>
    public partial class TextElement
    {
        /// <summary>
        /// Gets or sets the last position.
        /// </summary>
        public Rectangle LastPosition { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Text: {0}", this.Value);
        }
    }
}
