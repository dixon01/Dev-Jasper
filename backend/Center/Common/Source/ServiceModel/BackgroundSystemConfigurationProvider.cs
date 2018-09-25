// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemConfigurationProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemConfigurationProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Schema;
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// Provider for the BackgroundSystem configuration.
    /// </summary>
    public abstract class BackgroundSystemConfigurationProvider
    {
        static BackgroundSystemConfigurationProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static BackgroundSystemConfigurationProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = new DefaultBackgroundSystemConfigurationProvider();
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(BackgroundSystemConfigurationProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the Xml schema for the configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlSchema"/> for the background system configuration.
        /// </returns>
        public static XmlSchema GetXmlSchema()
        {
            using (
                var stream =
                    typeof(BackgroundSystemConfigurationProvider).Assembly.GetManifestResourceStream(
                        typeof(BackgroundSystemConfigurationProvider),
                        "BackgroundSystemConfiguration.xsd"))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException(
                        "Can't find the BackgroundSystemConfiguration XML schema. Ensure that the file is set as"
                        + "'EmbeddedResource' and it is in the same directory as this class.");
                }

                var exceptions = new List<XmlSchemaException>();
                var schema = XmlSchema.Read(stream, (sender, args) => exceptions.Add(args.Exception));
                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }

                return schema;
            }
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
        /// The <see cref="BackgroundSystemConfiguration"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the configuration is cached for the application lifetime for each specific address.
        /// It is required to restart the application to get an updated configuration.
        /// </remarks>
        public abstract BackgroundSystemConfiguration GetConfiguration(string address = null);

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
        public abstract Task<BackgroundSystemConfiguration> GetConfigurationAsync(string address = null);

        private class DefaultBackgroundSystemConfigurationProvider : BackgroundSystemConfigurationProvider
        {
            private readonly Dictionary<string, BackgroundSystemConfiguration> configurations =
                new Dictionary<string, BackgroundSystemConfiguration>();

            public override BackgroundSystemConfiguration GetConfiguration(string address = null)
            {
                return this.GetConfigurationAsync(address).Result;
            }

            public override async Task<BackgroundSystemConfiguration> GetConfigurationAsync(string address = null)
            {
                BackgroundSystemConfiguration configuration;
                var configurationAddress = address ?? ConfigurationManager.AppSettings["BackgroundSystemPortal"];
                if (string.IsNullOrEmpty(configurationAddress)
                    || string.Equals("local", configurationAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    configuration = new BackgroundSystemConfiguration();
                    configuration.NotificationsConnectionString =
                        ServiceConfigurationDefaults.DefaultMediConnectionString;
                    return configuration;
                }

                if (!configurationAddress.Contains("://"))
                {
                    configurationAddress = "http://" + configurationAddress;
                }

                if (!configurationAddress.EndsWith("/"))
                {
                    configurationAddress = configurationAddress + "/";
                }

                if (this.configurations.TryGetValue(configurationAddress, out configuration))
                {
                    return configuration;
                }

                var baseUri = new UriBuilder(configurationAddress).Uri;
                var uri = new Uri(baseUri, "Configuration");

                // this is done only to get a SocketException if the name couldn't be resolved
                await Dns.GetHostAddressesAsync(uri.Host);

                using (var stream = await DownloadConfigurationAsync(uri))
                {
                    try
                    {
                        var configurator = new Configurator(stream, GetXmlSchema());
                        var configManager = new ConfigManager<BackgroundSystemConfiguration>
                                                {
                                                    Configurator = configurator
                                                };
                        this.configurations[configurationAddress] = configManager.Config;
                        return configManager.Config;
                    }
                    catch (Exception exception)
                    {
                        throw new ConfigurationErrorsException("Can't download configuration", exception);
                    }
                }
            }

            private static async Task<Stream> DownloadConfigurationAsync(Uri uri)
            {
                var handler = HttpMessageHandlerFactory.Current.Create();
                using (var client = new HttpClient(handler, true))
                {
                    var response = await client.GetAsync(uri);

                    var buffer = await response.Content.ReadAsByteArrayAsync();
                    return new MemoryStream(buffer, false);
                }
            }
        }
    }
}