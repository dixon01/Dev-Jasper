// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    /// <summary>
    /// Defines the context of the display surface area.
    /// It contains the state of the visualization from a <c>device</c> point of view.
    /// </summary>
    public class DisplayContext
    {
        /// <summary>
        /// Gets or sets the main screen.
        /// </summary>
        /// <value>The identifier of the last used main screen, or <c>null</c> if none was ever used.</value>
        public ScreenIdentifier MainScreen { get; set; }
    }
}