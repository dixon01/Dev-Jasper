// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzurePortalSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzurePortalSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using Gorba.Center.Portal.Host.Settings;

    using Microsoft.WindowsAzure;

    /// <summary>
    /// Gets settings using the <see cref="CloudConfigurationManager"/>.
    /// </summary>
    public class AzurePortalSettingsProvider : PortalSettingsProvider.RootDirectoryPortalSettingsProvider
    {
        /// <summary>
        /// Gets the string value for a setting.
        /// </summary>
        /// <param name="settingName">
        /// The name of the setting.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value.
        /// </returns>
        protected override string GetStringValue(string settingName)
        {
            return CloudConfigurationManager.GetSetting(settingName);
        }
    }
}