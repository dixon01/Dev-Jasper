// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedAzurePortalSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtendedAzurePortalSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Extensions
{
    using Gorba.Center.Portal.Azure;
    using Gorba.Center.Portal.Host.Settings;

    /// <summary>
    /// Portal settings provider specific for Azure installed as single worker role together with BackgroundSystem.
    /// </summary>
    public class ExtendedAzurePortalSettingsProvider : AzurePortalSettingsProvider
    {
        /// <summary>
        /// Gets the <see cref="PortalSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PortalSettings"/>.
        /// </returns>
        public override PortalSettings GetSettings()
        {
            var settings = base.GetSettings();
            bool clickOnceUseBeta;
            if (this.TryGetSetting(PredefinedAzureItems.Settings.ClickOnceBeta, out clickOnceUseBeta))
            {
                settings.ClickOnceUseBeta = clickOnceUseBeta;
            }

            return settings;
        }
    }
}