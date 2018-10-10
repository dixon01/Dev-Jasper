// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Image.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Image type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Object tasked to represent the information about
    /// an image, within a BBCode formatted text.
    /// </summary>
    public sealed class Image : BbLeafValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="value">The value.</param>
        internal Image(BbBranch parent, string tagName, string value)
            : base(parent, tagName, value)
        {
        }

        /// <summary>
        /// Gets the name of this font face.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.Value;
            }
        }

        /// <summary>
        /// Method called after this tag was successfully
        /// parsed. Subclasses have the possibility to
        /// replace children or create a completely new
        /// node representing the contents of this node.
        /// </summary>
        /// <param name="context">
        /// The context
        /// </param>
        /// <returns>
        /// The default implementation simply returns this instance.
        /// </returns>
        internal override BbNode Cleanup(IBbParserContext context)
        {
            // make the path absolute using the context
            return new Image(this.Parent, this.TagName, context.GetAbsolutePathRelatedToConfig(this.FileName));
        }
    }
}