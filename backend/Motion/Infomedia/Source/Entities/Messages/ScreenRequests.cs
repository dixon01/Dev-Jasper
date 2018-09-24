// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenRequests.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Message that can be sent to the layout manager via Medi
    /// to request the current screen.
    /// </summary>
    public sealed class ScreenRequests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenRequests"/> class.
        /// </summary>
        public ScreenRequests()
        {
            this.Screens = new List<ScreenRequest>();
        }

        /// <summary>
        /// Gets or sets the screen requests.
        /// </summary>
        public List<ScreenRequest> Screens { get; set; } 
    }
}
