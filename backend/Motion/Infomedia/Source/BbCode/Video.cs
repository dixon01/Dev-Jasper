// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Video.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Object tasked to represent the information about
//   a video, within a BBCode formatted text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Object tasked to represent the information about
    /// a video, within a BBCode formatted text.
    /// </summary>
    public sealed class Video : BbLeafValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Video"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="value">The value.</param>
        internal Video(BbBranch parent, string tagName, string value)
            : base(parent, tagName, value)
        {
        }

        /// <summary>
        /// Gets the URI of the video.
        /// </summary>
        public string VideoUri
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
            return new Video(this.Parent, this.TagName, context.GetAbsolutePathRelatedToConfig(this.VideoUri));
        }
    }
}