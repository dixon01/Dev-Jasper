// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Face.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Face type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Font face BBCode tag [face=FaceName][/face].
    /// </summary>
    public sealed class Face : BbValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent tag.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="faceName">
        /// The face name.
        /// </param>
        internal Face(BbBranch parent, string tagName, string faceName)
            : base(parent, tagName, faceName)
        {
        }

        /// <summary>
        /// Gets the name of this font face.
        /// </summary>
        public string FaceName
        {
            get
            {
                return this.Value;
            }
        }
    }
}