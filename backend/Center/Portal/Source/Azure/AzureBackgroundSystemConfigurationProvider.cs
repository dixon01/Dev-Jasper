// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBackgroundSystemConfigurationProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureBackgroundSystemConfigurationProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Azure;
    using Gorba.Center.Common.ServiceModel;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;

    /// <summary>
    /// Configuration provider specific for Azure.
    /// </summary>
    public class AzureBackgroundSystemConfigurationProvider : BackgroundSystemConfigurationProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the configuration from the given address (if specified); otherwise, it returns the default (local)
        /// configuration.
        /// If the address value is 'local', the default configuration is provided.
        /// </summary>
        /// <param name="address">
        /// The address. A <c>null</c> or 'local' value is used to return the default (local) configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the configuration is cached for the application lifetime for each specific address.
        /// It is required to restart the application to get an updated configuration.
        /// </remarks>
        public override BackgroundSystemConfiguration GetConfiguration(string address = null)
        {
            Logger.Debug("Getting configuration for address '{0}'", address);
            var host = RoleEnvironment.GetConfigurationSettingValue(PredefinedAzureItems.Settings.Host);
            if (string.IsNullOrEmpty(host))
            {
                throw new Exception("Environment Host value not set");
            }

            var remoteServices = new RemoteServicesConfiguration
                                     {
                                         Host = host,
                                         Protocol = RemoveServiceProtocol.Tcp
                                     };
            Logger.Debug("Remote services: {0}", host);
            var configuration = new BackgroundSystemConfiguration
                       {
                           DataServices = remoteServices,
                           FunctionalServices = remoteServices
                       };
            var notificationsConnectionString =
                CloudConfigurationManager.GetSetting(PredefinedAzureItems.Settings.NotificationsConnectionString);
            configuration.NotificationsConnectionString = !string.IsNullOrEmpty(notificationsConnectionString)
                                                              ? notificationsConnectionString
                                                              : string.Format("medi://{0}", host);
            return configuration;
        }

        /// <summary>
        /// Gets the configuration from the given address (if specified); otherwise, it returns the default (local)
        /// configuration.
        /// If the address value is 'local', the default configuration is provided.
        /// </summary>
        /// <param name="address">
        /// The address. A <c>null</c> or 'local' value is used to return the default (local) configuration.
        /// </param>
        /// <returns>
        /// The task returning the <see cref="Gorba.Center.Common.ServiceModel.BackgroundSystemConfiguration"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the configuration is cached for the application lifetime for each specific address.
        /// It is required to restart the application to get an updated configuration.
        /// </remarks>
        public override Task<BackgroundSystemConfiguration> GetConfigurationAsync(string address = null)
        {
            return Task.FromResult(this.GetConfiguration(address));
        }
    }
}