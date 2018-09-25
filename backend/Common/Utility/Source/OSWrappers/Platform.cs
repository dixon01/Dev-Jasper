// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Platform.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers
{
    using System;

    /// <summary>
    /// The platform.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        /// The is windows.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWindows()
        {
            return (Environment.OSVersion.Platform != PlatformID.Unix) &&
                   (Environment.OSVersion.Platform != PlatformID.MacOSX);
        }

        /// <summary>
        /// The is windowsCE.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWindowsCe()
        {
            return Environment.OSVersion.Platform == PlatformID.WinCE;
        }

        /// <summary>
        /// The is unix.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        /// <summary>
        /// The is OSX.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsOsx()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }
    }
}
