// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDataRootDirectoryPortalSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDataRootDirectoryPortalSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Settings
{
    /// <summary>
    /// Provides the <see cref="PortalSettings"/> from a given AppData directory.
    /// </summary>
    public class AppDataRootDirectoryPortalSettingsProvider : PortalSettingsProvider.RootDirectoryPortalSettingsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDataRootDirectoryPortalSettingsProvider"/> class.
        /// </summary>
        /// <param name="appDataPath">
        /// The app data path.
        /// </param>
        public AppDataRootDirectoryPortalSettingsProvider(string appDataPath)
        {
            this.AppDataPath = appDataPath;
        }

        /// <summary>
        /// Gets the app data path.
        /// </summary>
        public string AppDataPath { get; private set; }

        /// <summary>
        /// Gets the <see cref="PortalSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PortalSettings"/>.
        /// </returns>
        public override PortalSettings GetSettings()
        {
            var appData = this.GetDirectoryInfo(this.AppDataPath);
            return new PortalSettings { AppDataPath = appData.FullName };
        }
    }
}