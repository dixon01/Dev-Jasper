// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        private static readonly TimeSpan SendReceiveTimeout = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Creates a channel factory for data services. The name of the service gets the 'DataService' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The <see cref="ChannelFactory&lt;T&gt;"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ChannelFactory<T> CreateDataServicesChannelFactory<T>(
            this RemoteServicesConfiguration configuration,
            string name)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            return CreateChannelFactory<T>(configuration, name + "DataService");
        }

        /// <summary>
        /// Creates an endpoint for data services. The name of the service gets the 'DataService' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The <see cref="ServiceEndpoint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ServiceEndpoint CreateDataServicesEndpoint<T>(
            this RemoteServicesConfiguration configuration,
            string name)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            return CreateServiceEndpoint<T>(configuration, name + "DataService");
        }

        /// <summary>
        /// Creates a channel factory for functional services. The name of the service gets the 'Service' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The <see cref="ChannelFactory&lt;T&gt;"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ChannelFactory<T> CreateFunctionalServicesChannelFactory<T>(
            this RemoteServicesConfiguration configuration,
            string name)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            return CreateChannelFactory<T>(configuration, name + "Service");
        }

        /// <summary>
        /// Creates an endpoint for functional services. The name of the service gets the 'Service' suffix.
        /// </summary>
        /// <param name="configuration">
        /// The remote services configuration.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">The type of the contract for the service.</typeparam>
        /// <returns>
        /// The <see cref="ServiceEndpoint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The provided configuration is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The provided name is null or empty.</exception>
        public static ServiceEndpoint CreateFunctionalServicesEndpoint<T>(
            this RemoteServicesConfiguration configuration,
            string name)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }

            return CreateServiceEndpoint<T>(configuration, name + "Service");
        }

        private static ChannelFactory<T> CreateChannelFactory<T>(RemoteServicesConfiguration configuration, string name)
        {
            EndpointAddress endpointAddress;
            Binding binding;
            configuration.GetServicesConfiguration(
                name,
                out endpointAddress,
                out binding,
                configuration.Protocol == RemoveServiceProtocol.Tcp,
                false);
            return new ChannelFactory<T>(binding, endpointAddress);
        }

        private static ServiceEndpoint CreateServiceEndpoint<T>(
            RemoteServicesConfiguration remoteServicesConfiguration,
            string name)
        {
            EndpointAddress endpointAddress;
            Binding binding;
            remoteServicesConfiguration.GetServicesConfiguration(
                name,
                out endpointAddress,
                out binding,
                true,
                true);
            return new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, endpointAddress);
        }

        /// <summary>
        /// Gets the configuration for data services.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <param name="name">
        ///     The name of the service.
        /// </param>
        /// <param name="endpointAddress">
        ///     The endpoint address.
        /// </param>
        /// <param name="binding">
        ///     The binding.
        /// </param>
        /// <param name="useCertificate">
        ///     Flag indicating whether the 'BackgroundSystem' DNS identity should be set for the address.
        /// </param>
        /// <param name="isHost">Flag indicating whether the configuration is used for a host or not.</param>
        /// <exception cref="ConfigurationErrorsException">The given configuration is not valid.</exception>
        private static void GetServicesConfiguration(
            this RemoteServicesConfiguration configuration,
            string name,
            out EndpointAddress endpointAddress,
            out Binding binding,
            bool useCertificate,
            bool isHost)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration", "The configuration can't be null");
            }

            endpointAddress = GetAddress(configuration, name, useCertificate, isHost);
            binding = GetBinding(configuration);
        }

        private static EndpointAddress GetAddress(
            RemoteServicesConfiguration configuration,
            string name,
            bool useCertificate,
            bool isHost)
        {
            string protocol;
            switch (configuration.Protocol)
            {
                case RemoveServiceProtocol.Http:
                    protocol = "http";
                    break;
                case RemoveServiceProtocol.Tcp:
                    protocol = "net.tcp";
                    break;
                default:
                    throw new NotSupportedException("Protocol '" + configuration.Protocol + "' not supported");
            }

            var port = isHost && configuration.InternalPort != -1
                           ? configuration.InternalPort
                           : configuration.Port;

            var uriBuilder = new UriBuilder(
                protocol,
                isHost ? "0.0.0.0" : configuration.Host,
                port);
            if (string.IsNullOrEmpty(configuration.Path))
            {
                uriBuilder.Path = name;
            }
            else
            {
                var path = configuration.Path.Last() == '/' ? configuration.Path : configuration.Path + '/';
                uriBuilder.Path += path + name;
            }

            if (useCertificate)
            {
                return new EndpointAddress(uriBuilder.Uri, new DnsEndpointIdentity("BackgroundSystem"));
            }

            return new EndpointAddress(uriBuilder.Path);
        }

        private static Binding GetBinding(RemoteServicesConfiguration configuration)
        {
            switch (configuration.Protocol)
            {
                case RemoveServiceProtocol.Http:
                    return GetBasicHttpBinding();
                case RemoveServiceProtocol.Tcp:
                    return GetNetTcpBinding();
                default:
                    throw new NotSupportedException(
                        "Protocol '" + configuration.Protocol + "' not supported");
            }
        }

        private static BasicHttpBinding GetBasicHttpBinding()
        {
            return new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
                       {
                           MaxBufferPoolSize = int.MaxValue,
                           MaxBufferSize = int.MaxValue,
                           MaxReceivedMessageSize = int.MaxValue,
                           ReaderQuotas =
                               new XmlDictionaryReaderQuotas
                                   {
                                       MaxArrayLength = int.MaxValue,
                                       MaxBytesPerRead = int.MaxValue,
                                       MaxDepth = int.MaxValue,
                                       MaxNameTableCharCount = int.MaxValue,
                                       MaxStringContentLength = int.MaxValue
                                   },
                           Security =
                               new BasicHttpSecurity
                                   {
                                       Message =
                                           new BasicHttpMessageSecurity
                                               {
                                                   ClientCredentialType
                                                       =
                                                       BasicHttpMessageCredentialType
                                                       .UserName
                                               },
                                       Mode =
                                           BasicHttpSecurityMode
                                           .TransportWithMessageCredential
                                   }
                       };
        }

        private static NetTcpBinding GetNetTcpBinding()
        {
            return new NetTcpBinding
                       {
                           MaxBufferPoolSize = int.MaxValue,
                           MaxBufferSize = int.MaxValue,
                           MaxConnections = 1024,
                           MaxReceivedMessageSize = int.MaxValue,
                           ReaderQuotas =
                               new XmlDictionaryReaderQuotas
                                   {
                                       MaxArrayLength = int.MaxValue,
                                       MaxBytesPerRead = int.MaxValue,
                                       MaxDepth = int.MaxValue,
                                       MaxNameTableCharCount = int.MaxValue,
                                       MaxStringContentLength = int.MaxValue
                                   },
                           ReceiveTimeout = SendReceiveTimeout,
                           Security =
                               new NetTcpSecurity
                                   {
                                       Message =
                                           new MessageSecurityOverTcp
                                               {
                                                   ClientCredentialType
                                                       =
                                                       MessageCredentialType
                                                       .UserName
                                               },
                                       Mode = SecurityMode.TransportWithMessageCredential,
                                       Transport =
                                           new TcpTransportSecurity
                                               {
                                                   ClientCredentialType
                                                       =
                                                       TcpClientCredentialType
                                                       .None
                                               }
                                   },
                           SendTimeout = SendReceiveTimeout,
                           TransferMode = TransferMode.Streamed
                       };
        }
    }
}