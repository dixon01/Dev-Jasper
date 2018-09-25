// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfovisionDisplayState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfovisionDisplayState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The Atmel Controller display state.
    /// </summary>
    public class InfovisionDisplayState : AtmelControlObject
    {
        /// <summary>
        /// Gets or sets the number of connected displays.
        /// </summary>
        public int ConnectedDisplayNo { get; set; }

        /// <summary>
        /// Gets or sets the connected display information.
        /// </summary>
        public InfovisionDisplayDevice[] Display { get; set; }
    }
}
