// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Resource type handled by media.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    /// <summary>
    /// Defines the type of a Resource
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// The image type.
        /// </summary>
        Image,

        /// <summary>
        /// The video type.
        /// </summary>
        Video,

        /// <summary>
        /// The symbol type.
        /// </summary>
        Symbol,

        /// <summary>
        /// The audio type.
        /// </summary>
        Audio,

        /// <summary>
        /// The font type.
        /// </summary>
        Font,

        /// <summary>
        /// The csv type.
        /// </summary>
        Csv
    }
}
