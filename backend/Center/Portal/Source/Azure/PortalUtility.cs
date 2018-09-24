// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Portal.Host;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using NLog;

    /// <summary>
    /// Utility methods for the Portal.
    /// </summary>
    public static class PortalUtility
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a host for the given endpoint name. If the name of a flag is specified, then it is retrieved from the
        /// cloud configuration to enable or not the host.
        /// </summary>
        /// <param name="endpointName">
        /// The endpoint name.
        /// </param>
        /// <param name="flag">
        /// The name of a flag setting that can be used to enable or not the host.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/> host. If the flag is specified (and set to false), a host that doesn't do
        /// anything is returned.
        /// </returns>
        public static IDisposable GetHost(string endpointName, string flag = null)
        {
            if (flag != null)
            {
                var flagValue = GetConfigurationSettingValue(flag, false);
                if (!flagValue)
                {
                    Logger.Info("{0} not set. Skipping endpoint '{1}'", flag, endpointName);
                    return new NullHost();
                }
            }

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[endpointName];
            var uri = string.Format(
                "{0}://{1}:{2}",
                endpoint.Protocol,
                endpoint.IPEndpoint.Address,
                endpoint.IPEndpoint.Port);
            return PortalHost.Create(uri);
        }

        private static T GetConfigurationSettingValue<T>(string name, T defaultValue = default(T))
        {
            var stringValue = CloudConfigurationManager.GetSetting(name);
            if (string.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromString(stringValue);
        }

        private class NullHost : IDisposable
        {
            public void Dispose()
            {
                // Nothing to do here
            }
        }
    }
}