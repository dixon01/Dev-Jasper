// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Alternating.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Alternating type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// The BBCode [a][/a] tag.
    /// This node can only contain <see cref="Alternation"/>s as children.
    /// </summary>
    public sealed class Alternating : BbValueTag
    {
        private readonly List<BbNode> tempChildren = new List<BbNode>();

        private bool cleanedUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="Alternating"/> class.
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
        internal Alternating(BbBranch parent, string tagName, string value)
            : base(parent, tagName, value ?? string.Empty)
        {
            int interval;
            if (ParserUtil.TryParse(this.Value, out interval))
            {
                this.IntervalSeconds = interval;
            }
        }

        /// <summary>
        /// Gets the alternation interval in seconds.
        /// If this value is null, the renderer should use the pre-configured alternation interval.
        /// </summary>
        public int? IntervalSeconds { get; private set; }

        /// <summary>
        /// Adds a node to the list of <see cref="BbBranch.Children"/>.
        /// This method should only be called during parsing.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <exception cref="ArgumentException">if something else than <see cref="Alternation"/>s is added.</exception>
        internal override void Add(BbNode node)
        {
            if (!this.cleanedUp)
            {
                this.tempChildren.Add(node);
                return;
            }

            if (!(node is Alternation))
            {
                throw new ArgumentException("Can't add node to Alternating: " + node.GetType().Name, "node");
            }

            base.Add(node);
        }

        /// <summary>
        /// Cleans up this node by placing children in
        /// <see cref="Alternation"/>s, splitting on <see cref="Delimiter"/>.
        /// </summary>
        /// <param name="context">
        /// the context.
        /// </param>
        /// <returns>
        /// returns this instance.
        /// </returns>
        internal override BbNode Cleanup(IBbParserContext context)
        {
            if (this.cleanedUp)
            {
                return this;
            }

            this.cleanedUp = true;
            var alt = new Alternation(this);
            this.Add(alt);
            foreach (var child in this.tempChildren)
            {
                var delim = child as Delimiter;
                if (delim != null)
                {
                    alt = new Alternation(this);
                    this.Add(alt);
                }
                else
                {
                    alt.Add(child);
                }
            }

            return this;
        }

        /// <summary>
        /// The BBCode [|] tag. This tag can only be used inside [a][/a].
        /// </summary>
        internal class Delimiter : BbLeafTag
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Delimiter"/> class.
            /// </summary>
            /// <param name="parent">
            /// The parent.
            /// </param>
            /// <param name="tagName">
            /// The tag name.
            /// </param>
            /// <exception cref="ArgumentException">
            /// if the <see cref="parent"/> is not an <see cref="Alternating"/> tag.
            /// </exception>
            public Delimiter(BbBranch parent, string tagName)
                : base(parent, tagName)
            {
                if (!(parent is Alternating))
                {
                    throw new ArgumentException("Alternation's parent needs to be an Alternating", "parent");
                }
            }
        }
    }
}