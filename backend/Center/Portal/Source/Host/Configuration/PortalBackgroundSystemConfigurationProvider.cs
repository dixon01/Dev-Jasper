// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalBackgroundSystemConfigurationProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalBackgroundSystemConfigurationProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Configuration
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Utility.Files;
    using NLog;

    /// <summary>
    /// Provides the system configuration for the Portal.
    /// It generated the configuration according to the Azure settings.
    /// If not found, it returns a default configuration.
    /// </summary>
    public class PortalBackgroundSystemConfigurationProvider :
        BackgroundSystemConfigurationProvider
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
        /// The <see cref="BackgroundSystemConfiguration"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the configuration is cached for the application lifetime for each specific address.
        /// It is required to restart the application to get an updated configuration.
        /// This implementation doesn't support the <paramref name="address"/> attribute.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Thrown if a value other than <c>null</c> is specified for the <paramref name="address"/>.
        /// </exception>
        public override BackgroundSystemConfiguration GetConfiguration(string address = null)
        {
            var settings = PortalSettingsProvider.Current.GetSettings();
            var filePath = Path.Combine(settings.AppDataPath, "BackgroundSystemConfiguration.xml");
            IFileInfo file;
            if (FileSystemManager.Local.TryGetFile(filePath, out file))
            {
                var configManager = new ConfigManager<BackgroundSystemConfiguration>
                                        {
                                            Configurator =
                                                new Configurator(
                                                file.FullName,
                                                GetXmlSchema())
                                        };
                return configManager.Config;
            }

            var fullMachineName = Dns.GetHostEntry(string.Empty).HostName;
            var defaultConnectionString = new Uri(new Uri("medi://"), fullMachineName);
            var configuration = new BackgroundSystemConfiguration
                       {
                           DataServices = new RemoteServicesConfiguration { Host = fullMachineName },
                           FunctionalServices =
                               new RemoteServicesConfiguration { Host = fullMachineName },
                           NotificationsConnectionString = defaultConnectionString.ToString()
                       };
                       Logger.Trace("Returning configuration: {0}", configuration);
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
        /// The task returning the <see cref="BackgroundSystemConfiguration"/>.
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