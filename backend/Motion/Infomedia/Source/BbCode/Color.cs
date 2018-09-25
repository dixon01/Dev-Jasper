// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Color.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Color type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Color BBCode tag [color=xxx][/color].
    /// </summary>
    public class Color : BbValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent tag.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="colorName">
        /// The color name.
        /// </param>
        internal Color(BbBranch parent, string tagName, string colorName)
            : base(parent, tagName, colorName)
        {
        }

        /// <summary>
        /// Gets the name of the color.
        /// </summary>
        public string ColorName
        {
            get
            {
                return this.Value;
            }
        }
    }
}
