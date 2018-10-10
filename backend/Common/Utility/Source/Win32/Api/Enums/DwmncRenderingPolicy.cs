// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DwmncRenderingPolicy.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DwmncRenderingPolicy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// Flags used by the <c>DwmSetWindowAttribute</c> function to specify
    /// the non-client area rendering policy.
    /// </summary>
    public enum DwmncRenderingPolicy
    {
        /// <summary>
        /// The non-client rendering area is rendered based on the window style.
        /// </summary>
        UseWindowStyle,

        /// <summary>
        /// The non-client area rendering is disabled; the window style is ignored.
        /// </summary>
        Disabled,

        /// <summary>
        /// The non-client area rendering is enabled; the window style is ignored.
        /// </summary>
        Enabled,

        /// <summary>
        /// The maximum recognized <see cref="DwmncRenderingPolicy"/> value, used for validation purposes.
        /// </summary>
        Last
    }
}
