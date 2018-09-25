// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfigurationDefaults.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceConfigurationDefaults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Net;

    /// <summary>
    /// Static class containing service configuration default values.
    /// </summary>
    public static class ServiceConfigurationDefaults
    {
        private const string DefaultMediConnectionStringFormat = "medi://{0}:1596";

        static ServiceConfigurationDefaults()
        {
            string hostName;
            try
            {
                hostName = Dns.GetHostEntry(Environment.MachineName).HostName.ToLowerInvariant();
            }
            catch (Exception)
            {
                hostName = "localhost";
            }

            DefaultDataServicesConfiguration = new RemoteServicesConfiguration
                                                   {
                                                       Host = hostName,
                                                       Protocol = RemoveServiceProtocol.Tcp
                                                   };
            DefaultFunctionalServicesConfiguration = new RemoteServicesConfiguration
                                                         {
                                                             Host = hostName,
                                                             Protocol = RemoveServiceProtocol.Tcp
                                                         };

            DefaultMediConnectionString = string.Format(DefaultMediConnectionStringFormat, hostName);
        }

        /// <summary>
        /// Gets the default (local) configuration for data services.
        /// </summary>
        public static RemoteServicesConfiguration DefaultDataServicesConfiguration { get; private set; }

        /// <summary>
        /// Gets the default (local) configuration for functional services.
        /// </summary>
        public static RemoteServicesConfiguration DefaultFunctionalServicesConfiguration { get; private set; }

        /// <summary>
        /// Gets the default (local) medi connection string.
        /// </summary>
        public static string DefaultMediConnectionString { get; private set; }
    }
}