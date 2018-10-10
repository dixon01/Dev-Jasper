// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextDirection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextDirection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    /// <summary>
    /// The direction of a text.
    /// </summary>
    public enum TextDirection
    {
        /// <summary>
        /// The default direction (usually <see cref="LTR"/>).
        /// </summary>
        Default,

        /// <summary>
        /// Left to right direction (normal "western" flow of characters).
        /// </summary>
        LTR,

        /// <summary>
        /// Right to left direction ("arabic" flow of characters).
        /// </summary>
        RTL
    }
}