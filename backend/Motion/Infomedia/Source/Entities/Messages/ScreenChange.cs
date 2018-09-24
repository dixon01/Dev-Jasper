// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenChange.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenChange type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// The screen change.
    /// </summary>
    public class ScreenChange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenChange"/> class.
        /// </summary>
        public ScreenChange()
        {
            this.FontFiles = new List<string>();
        }

        /// <summary>
        /// Gets or sets the screen id to which this change refers.
        /// </summary>
        public ScreenId Screen { get; set; }

        /// <summary>
        /// Gets or sets the list font files required for rendering this screen.
        /// The file names are taken from the Infomedia presentation file.
        /// </summary>
        public List<string> FontFiles { get; set; }

        /// <summary>
        /// Gets or sets the root of the new screen.
        /// </summary>
        public ScreenRoot ScreenRoot { get; set; }

        /// <summary>
        /// Gets or sets the animation to be used when the screen changes.
        /// If this is null, the screen change should not be animated.
        /// </summary>
        public PropertyChangeAnimation Animation { get; set; }
    }
}