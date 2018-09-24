// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServicesConfigurationExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServicesConfigurationExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Xml;

    /// <summary>
    /// Extension methods for the <see cref="ServicesConfiguration"/>.
    /// </summary>
    public static class ServicesConfigurationExtensions
    {
        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The address.</returns>
        public static EndpointAddress GetAddress(this ServicesConfiguration configuration)
        {
            return new EndpointAddress(
                new Uri("net.tcp://localhost:8099/CalculatorService"),
                new DnsEndpointIdentity("BackgroundSystem"));
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Binding"/>.
        /// </returns>
        public static Binding GetBinding(this ServicesConfiguration configuration)
        {
            return new NetTcpBinding
                       {
                          Security = new NetTcpSecurity
                                         {
                                             Message = new MessageSecurityOverTcp
                                                           {
                                                               ClientCredentialType = MessageCredentialType.UserName
                                                           },
                                             Mode = SecurityMode.Message,
                                             Transport = new TcpTransportSecurity
                                                             {
                                                                 ClientCredentialType = TcpClientCredentialType.None
                                                             }
                                         },
                           MaxBufferPoolSize = int.MaxValue,
                           MaxBufferSize = int.MaxValue,
                           MaxConnections = 2048,
                           MaxReceivedMessageSize = int.MaxValue,
                           ReaderQuotas =
                               new XmlDictionaryReaderQuotas
                                   {
                                       MaxArrayLength =
                                           int.MaxValue,
                                       MaxBytesPerRead =
                                           int.MaxValue,
                                       MaxDepth = int.MaxValue,
                                       MaxNameTableCharCount =
                                           int.MaxValue,
                                       MaxStringContentLength =
                                           int.MaxValue
                                   }
                       };
        }

        /// <summary>
        /// Sets the login.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="serviceCertificateValidationMode">
        /// The service certificate validation mode.
        /// </param>
        /// <typeparam name="T">The type of the channel.</typeparam>
        public static void SetLogin<T>(
            this ServicesConfiguration configuration,
            ChannelFactory<T> factory,
            X509CertificateValidationMode serviceCertificateValidationMode = X509CertificateValidationMode.None)
        {
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            // step two - instantiate your credentials
            var loginCredentials = new ClientCredentials();
            loginCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                serviceCertificateValidationMode;
            loginCredentials.UserName.UserName = configuration.Username;
            loginCredentials.UserName.Password = configuration.Password;

            factory.Endpoint.Behaviors.Add(loginCredentials);
        }

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceEndpoint"/>.
        /// </returns>
        public static ServiceEndpoint GetEndpoint(this ServicesConfiguration configuration)
        {
            var address = configuration.GetAddress();
            var binding = configuration.GetBinding();
            return new ServiceEndpoint(ContractDescription.GetContract(typeof(ISampleService)), binding, address);
        }
    }
}