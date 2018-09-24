// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SecurityExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    using Gorba.Center.BackgroundSystem.Core.Certificates;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Defines extensions for security.
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Creates a service host for a data service instance. The name of the service gets the 'DataService' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <param name="serviceInstance">the service instance.</param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The service host.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ServiceHost CreateDataServiceHost<T>(
            this RemoteServicesConfiguration configuration,
            string name,
            T serviceInstance)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            var serviceHost = new ServiceHost(serviceInstance);
            var endpoint = configuration.CreateDataServicesEndpoint<T>(name);
            serviceHost.SetServiceCredentials();
            serviceHost.AddServiceEndpoint(endpoint);
            return serviceHost;
        }

        /// <summary>
        /// Creates a service host for a functional service instance. The name of the service gets the 'Service' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <param name="serviceInstance">the service instance.</param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The service host.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ServiceHost CreateFunctionalServiceHost<T>(
            this RemoteServicesConfiguration configuration,
            string name,
            T serviceInstance)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            var serviceHost = new ServiceHost(serviceInstance);
            var endpoint = configuration.CreateFunctionalServicesEndpoint<T>(name);
            serviceHost.SetServiceCredentials();
            serviceHost.AddServiceEndpoint(endpoint);
            return serviceHost;
        }

        /// <summary>
        /// Sets the credentials for a service host.
        /// </summary>
        /// <param name="host">The host.</param>
        private static void SetServiceCredentials(this ServiceHost host)
        {
            var serviceCredential = host.Description.Behaviors.Find<ServiceCredentials>();
            if (serviceCredential == null)
            {
                serviceCredential = new ServiceCredentials();
                host.Description.Behaviors.Add(serviceCredential);
            }

            serviceCredential.ServiceCertificate.Certificate = CertificatesUtility.GetBackgroundSystemCertificate();
            serviceCredential.UserNameAuthentication.UserNamePasswordValidationMode =
                UserNamePasswordValidationMode.Custom;
            serviceCredential.UserNameAuthentication.CustomUserNamePasswordValidator =
                LoginValidatorProvider.Current.Provide();
        }
    }
}