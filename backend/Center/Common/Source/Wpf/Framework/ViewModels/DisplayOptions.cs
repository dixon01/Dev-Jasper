// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayOptions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// Defines the options to display something on available surface (screen, position, etc.).
    /// </summary>
    public class DisplayOptions
    {
        private Screen screen;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayOptions"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="screen"/> is null.</exception>
        public DisplayOptions(Screen screen)
        {
            if (screen == null)
            {
                throw new ArgumentNullException("screen");
            }

            this.Screen = screen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayOptions"/> class.
        /// </summary>
        public DisplayOptions()
            : this(Screen.GetScreen(ScreenIdentifier.Primary))
        {
        }

        /// <summary>
        /// Gets or sets the screen.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        public Screen Screen
        {
            get
            {
                return this.screen;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.screen = value;
            }
        }
    }
}