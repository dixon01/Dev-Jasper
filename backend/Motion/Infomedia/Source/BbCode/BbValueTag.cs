// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbValueTag.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbValueTag type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Base class for all BBCode tags that contain a value like: [tag=value][/tag]
    /// </summary>
    public abstract class BbValueTag : BbTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbValueTag"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="BbParseException">
        /// if no value was provided.
        /// </exception>
        internal BbValueTag(BbBranch parent, string tagName, string value)
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
        /// Subclasses should implement their own porperty that
        /// wraps this property, giving it a reasonable name.
        /// </summary>
        protected internal string Value { get; private set; }
    }
}