// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility
{
    using System.IO;

    /// <summary>
    /// Helper class for application-wide properties.
    /// </summary>
    public partial class ApplicationHelper
    {
        /// <summary>
        /// Gets the name of the entry assembly (EXE name without extension).
        /// </summary>
        /// <returns>
        /// The name of the entry assembly.
        /// </returns>
        public static string GetEntryAssemblyName()
        {
            return Path.GetFileNameWithoutExtension(GetEntryAssemblyLocation());
        }

        /// <summary>
        /// Gets the file version of the application.
        /// </summary>
        /// <returns>
        /// The file version of the application.
        /// </returns>
        public static string GetApplicationFileVersion()
        {
            var filename = GetEntryAssemblyLocation();
            return GetFileVersion(filename);
        }
    }
}
