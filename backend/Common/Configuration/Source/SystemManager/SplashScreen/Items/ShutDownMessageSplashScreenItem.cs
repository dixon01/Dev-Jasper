// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownMessageSplashScreenItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Items
{
    using System;

    /// <summary>
    /// Show information that system is shutting down within specified time
    /// and if the USB stick remove message on the splash screen.
    /// </summary>
    public class ShutDownMessageSplashScreenItem : SplashScreenItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutDownMessageSplashScreenItem"/> class.
        /// </summary>
        public ShutDownMessageSplashScreenItem()
        {
            this.ShutDownMessage = "The System will be shutting down in..";
        }

        /// <summary>
        /// Gets or sets the shut down message.
        /// </summary>
        public string ShutDownMessage { get; set; }

        /// <summary>
        /// Gets or sets the shutdown time.
        /// </summary>
        public TimeSpan ShutdownTime { get; set; }
    }
}
