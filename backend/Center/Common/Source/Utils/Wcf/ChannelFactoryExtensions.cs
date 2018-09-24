// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactoryExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelFactoryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils.Wcf
{
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    /// <summary>
    /// Defines utility extension methods for channel factory.
    /// </summary>
    public static class ChannelFactoryExtensions
    {
        /// <summary>
        /// Sets the login.
        /// </summary>
        /// <typeparam name="T">The type of the channel.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="serviceCertificateValidationMode">The service certificate validation mode.</param>
        public static void SetLogin<T>(
            this ChannelFactory<T> factory,
            string username,
            string password,
            X509CertificateValidationMode serviceCertificateValidationMode = X509CertificateValidationMode.None)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            // step two - instantiate your credentials
            var loginCredentials = new ClientCredentials();
            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                serviceCertificateValidationMode;
            loginCredentials.UserName.UserName = username;
            loginCredentials.UserName.Password = password;

            factory.Endpoint.Behaviors.Add(loginCredentials);
        }

        /// <summary>
        /// Sets the certificate to be used in credentials for the channels created by the factory.
        /// </summary>
        /// <typeparam name="T">The type of the channel.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="x509FindType">Type of the find.</param>
        /// <param name="serviceCertificateValidationMode">The service certificate validation mode.</param>
        public static void SetCertificate<T>(
            this ChannelFactory<T> factory,
            string name,
            StoreLocation storeLocation = StoreLocation.LocalMachine,
            StoreName storeName = StoreName.My,
            X509FindType x509FindType = X509FindType.FindBySubjectName,
            X509CertificateValidationMode serviceCertificateValidationMode = X509CertificateValidationMode.None)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            var loginCredentials = new ClientCredentials();
            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.None;

            loginCredentials.ClientCertificate.SetCertificate(
                storeLocation, storeName, x509FindType, name);
            factory.Endpoint.Behaviors.Add(loginCredentials);
        }
    }
}