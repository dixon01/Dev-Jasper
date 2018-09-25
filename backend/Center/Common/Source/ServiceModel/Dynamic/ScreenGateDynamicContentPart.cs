// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenGateDynamicContentPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenGateDynamicContentPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Dynamic
{
    /// <summary>
    /// The dynamic content information for ScreenGate data.
    /// </summary>
    public class ScreenGateDynamicContentPart : DynamicContentPartBase
    {
        /// <summary>
        /// Gets or sets the configuration URL.
        /// This is where a player has to get its content data.
        /// </summary>
        public string ConfigUrl { get; set; }

        /// <summary>
        /// Gets or sets the username to log in to ScreenGate.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to log in to ScreenGate.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the <c>.wm2</c> file.
        /// </summary>
        public string Wm2FilePath { get; set; }
    }
}