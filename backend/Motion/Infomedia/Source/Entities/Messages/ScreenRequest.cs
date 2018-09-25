// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    /// <summary>
    /// A single request for a screen.
    /// </summary>
    public class ScreenRequest
    {
        /// <summary>
        /// Gets or sets the screen id.
        /// </summary>
        public ScreenId ScreenId { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        public int Height { get; set; }
    }
}