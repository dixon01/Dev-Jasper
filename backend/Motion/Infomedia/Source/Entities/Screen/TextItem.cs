// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextItem.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Screen item that displays text with the given attributes.
    /// </summary>
    public partial class TextItem
    {
        /// <summary>
        /// Gets or sets the text element.
        /// </summary>
        public TextElement TextElement { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Text: \"{0}\" @ [{1},{2}]", this.Text, this.X, this.Y);
        }
    }
}