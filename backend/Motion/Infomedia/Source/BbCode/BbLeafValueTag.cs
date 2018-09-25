// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbLeafValueTag.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbLeafValueTag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Base class for tags that can contain a value and are also leaves.
    /// </summary>
    public abstract class BbLeafValueTag : BbLeafTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbLeafValueTag"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="BbParseException">
        /// Exception thrown of this leaf doesn't have a value.
        /// </exception>
        protected BbLeafValueTag(BbBranch parent, string tagName, string value)
            : base(parent, tagName)
        {
            if (value == null)
            {
                throw new BbParseException("No value provided for " + tagName);
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        protected internal string Value { get; private set; }
    }
}