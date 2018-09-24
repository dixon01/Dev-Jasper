// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Alternation.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Alternation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// One alternative in <see cref="Alternating"/>.
    /// </summary>
    public sealed class Alternation : BbBranch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alternation"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if the <see cref="parent"/> is not an <see cref="Alternating"/>.
        /// </exception>
        public Alternation(BbBranch parent)
            : base(parent)
        {
            if (!(parent is Alternating))
            {
                throw new ArgumentException("Alternation's parent needs to be an Alternating", "parent");
            }
        }
    }
}