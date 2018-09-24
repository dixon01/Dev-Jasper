// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SecurityExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// Security extension methods.
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Sets the login.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="userCredentials">
        /// Credentials used for validation against the system.
        /// </param>
        /// <typeparam name="T">The type of the channel.</typeparam>
        public static void SetLoginCredentials<T>(this ChannelFactory<T> factory, UserCredentials userCredentials)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            var loginCredentials = new ClientCredentials();

            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.Custom;
            loginCredentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                new BackgroundSystemX509CertificateValidator();
            loginCredentials.UserName.UserName = userCredentials.Username;
            loginCredentials.UserName.Password = userCredentials.HashedPassword;

            factory.Endpoint.Behaviors.Add(loginCredentials);
        }
    }
}