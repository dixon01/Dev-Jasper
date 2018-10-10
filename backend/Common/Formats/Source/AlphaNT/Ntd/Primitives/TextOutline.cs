// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextOutline.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextOutline type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    /// <summary>
    /// The possible styles of the border around a (colored) text.
    /// </summary>
    public enum TextOutline
    {
        /// <summary>
        /// No outline is drawn.
        /// </summary>
        None = 0,

        /// <summary>
        /// A black outline is drawn.
        /// </summary>
        Black = 1,

        /// <summary>
        /// A white outline is drawn.
        /// </summary>
        White = 2
    }
}