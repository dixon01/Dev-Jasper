// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureHostingSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureHostingSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Extensions
{
    using Gorba.Center.Common.ServiceModel.Settings;

    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Settings provider specific for Azure.
    /// </summary>
    public class AzureHostingSettingsProvider : HostingSettingsProvider
    {
        /// <summary>
        /// Gets the settings
        /// </summary>
        /// <param name="path">Optional path to the settings file.</param>
        /// <returns>
        /// The <see cref="Gorba.Center.Common.ServiceModel.Settings.HostingSettings"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the settings are read from a file named Settings.xml located in the directory
        /// of the application; the file is loaded once and cached for the application lifetime.
        /// It is required to restart the application to get updated settings.
        /// </remarks>
        public override HostingSettings GetSettings(string path = null)
        {
            return
                new HostingSettings(
                    RoleEnvironment.GetLocalResource(Common.Azure.PredefinedAzureItems.LocalStorage.Resources).RootPath,
                    string.Empty);
        }
    }
}