// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbText.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Text leaf node of the BBCode tree.
    /// </summary>
    public sealed class BbText : BbNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbText"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public BbText(BbBranch parent, string text)
            : base(parent)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }
    }
}