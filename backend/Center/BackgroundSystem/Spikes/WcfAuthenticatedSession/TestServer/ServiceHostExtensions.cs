// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceHostExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using WcfAuthenticatedSession.ServiceModel;

    /// <summary>
    /// Service host extension methods.
    /// </summary>
    public static class ServiceHostExtensions
    {
        /// <summary>
        /// Sets the credentials for a service host.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void SetServiceCredentials(this ServiceHost host)
        {
            var serviceCredential = host.Description.Behaviors.Find<ServiceCredentials>();
            if (serviceCredential == null)
            {
                serviceCredential = new ServiceCredentials();
                host.Description.Behaviors.Add(serviceCredential);
            }

            serviceCredential.ServiceCertificate.Certificate = Utility.GetCertificate();
            serviceCredential.UserNameAuthentication.UserNamePasswordValidationMode
                = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
            serviceCredential.UserNameAuthentication.CustomUserNamePasswordValidator = new LoginValidator();
        }
    }
}