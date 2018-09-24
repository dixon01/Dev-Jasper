// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementScaling.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementScaling type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    /// <summary>
    /// How should an element be scaled to fit the defined bounds.
    /// </summary>
    public enum ElementScaling
    {
        /// <summary>
        /// Default value. Stretch the content in both directions to fit into
        /// the defined bounds.
        /// </summary>
        Stretch,

        /// <summary>
        /// Scale up or down the content so it fits into the defined bounds but
        /// it's aspect ratio is kept (i.e. width or height might be less than 
        /// the defined bounds).
        /// </summary>
        Scale,

        /// <summary>
        /// Doesn't scale the content but just shows it in its native size even
        /// if it doesn't fit the defined bounds.
        /// </summary>
        Fixed
    }
}