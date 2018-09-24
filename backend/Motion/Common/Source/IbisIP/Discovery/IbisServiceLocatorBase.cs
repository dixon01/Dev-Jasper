// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisServiceLocatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisServiceLocatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP.Client;
    using Gorba.Motion.Common.IbisIP.Discovery.Messages;
    using Gorba.Motion.Common.IbisIP.Server;

    using NLog;

    /// <summary>
    /// Base class for all <see cref="IIbisServiceLocator"/> implementations.
    /// </summary>
    public abstract class IbisServiceLocatorBase : IIbisServiceLocator
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger = LogHelper.GetLogger<IbisServiceLocatorBase>();

        private readonly Dictionary<IServiceInfo, IVdv301Service> localServices =
            new Dictionary<IServiceInfo, IVdv301Service>();

        private IbisHttpServer httpServer;

        private IbisUdpServer udpServer;

        private bool validateHttpRequests;

        private bool validateHttpResponses;

        private bool verifyVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisServiceLocatorBase"/> class.
        /// </summary>
        protected IbisServiceLocatorBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Configures this object with the given configuration.
        /// This method must be called before any other method of this class.
        /// </summary>
        /// <param name="httpRequests">
        /// The validate Http Requests.
        /// </param>
        /// <param name="httpResponses">
        /// The validate Http Responses.
        /// </param>
        /// <param name="version">
        /// The verify Version.
        /// </param>
        public virtual void Configure(bool httpRequests, bool httpResponses, bool version)
        {
            this.validateHttpRequests = httpRequests;
            this.validateHttpResponses = httpResponses;
            this.verifyVersion = version;
        }

        /// <summary>
        /// Registers the given service with the service locator.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <typeparam name="TService">
        /// The type of the service to be registered,
        /// should be one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </typeparam>
        /// <exception cref="ArgumentException">
        /// If the given <paramref name="service"/> doesn't implement <see cref="IVdv301ServiceImpl"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <typeparamref name="TService"/> isn't one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </exception>
        public abstract void RegisterService<TService>(TService service)
            where TService : class, IVdv301Service;

        /// <summary>
        /// Gets a query for a given service type.
        /// You need to call <see cref="IServiceQuery{T}.Start"/> on the returned object to start getting services.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service to be returned,
        /// should be one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </typeparam>
        /// <returns>
        /// The query for <see cref="TService"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <typeparamref name="TService"/> isn't one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </exception>
        public abstract IServiceQuery<TService> QueryServices<TService>() where TService : class, IVdv301Service;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.httpServer != null)
            {
                this.httpServer.Stop();
            }
        }

        /// <summary>
        /// Gets the service name for the given service type.
        /// </summary>
        /// <typeparam name="TService">
        /// the service type (must be an interface).
        /// </typeparam>
        /// <returns>
        /// The name of the service
        /// </returns>
        protected string GetServiceName<TService>()
            where TService : IVdv301Service
        {
            var serviceType = typeof(TService);
            if (!serviceType.IsInterface)
            {
                throw new ArgumentException("Type parameter must be an interface implementing IVdv301Service");
            }

            return serviceType.Name.Substring(1);
        }

        /// <summary>
        /// Gets a flag indicating if the given service is an HTTP service.
        /// </summary>
        /// <typeparam name="TService">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// True if the given service is an HTTP service, otherwise false.
        /// </returns>
        protected bool IsHttpService<TService>()
        {
            return typeof(IVdv301HttpService).IsAssignableFrom(typeof(TService));
        }

        /// <summary>
        /// Registers the given service.
        /// </summary>
        /// <param name="service">
        /// The service implementation.
        /// </param>
        /// <typeparam name="TService">
        /// the service type (must be an interface).
        /// </typeparam>
        /// <returns>
        /// The <see cref="ServiceRegistrationRequest"/> containing all information about the service.
        /// </returns>
        protected ServiceRegistrationRequest DoRegisterService<TService>(TService service)
            where TService : IVdv301Service
        {
            var serviceName = this.GetServiceName<TService>();

            ServiceRegistrationRequest request;
            var httpService = service as IVdv301HttpService;
            var info = new ServiceInfo();
            if (httpService != null)
            {
                request = this.RegisterHttpService(httpService);
                foreach (var address in this.httpServer.LocalAddresses)
                {
                    info.Addresses.Add(address.ToString());
                }
            }
            else
            {
                var udpService = service as IVdv301UdpService;
                if (udpService == null)
                {
                    throw new ArgumentException("Service is neither an HTTP nor UDP service");
                }

                request = this.RegisterUdpService(udpService);
                foreach (var address in this.udpServer.LocalAddresses)
                {
                    info.Addresses.Add(address.ToString());
                }
            }

            request.Name = serviceName;

            info.Name = request.Name;
            info.Attributes = request.Attributes;
            info.Port = request.Port;
            info.Protocol = request.Protocol;

            lock (this.localServices)
            {
                this.localServices.Add(info, service);
            }

            if (this.Logger.IsInfoEnabled)
            {
                var attrs = new StringBuilder();
                foreach (var attr in info.Attributes)
                {
                    if (attrs.Length > 0)
                    {
                        attrs.Append(";");
                    }

                    attrs.Append(attr.Name).Append("=").Append(attr.Value);
                }

                this.Logger.Info("Registered {0}.{1} on {2} with {3}", info.Name, info.Protocol, info.Port, attrs);
            }

            return request;
        }

        /// <summary>
        /// Creates a service proxy with the given <see cref="IServiceInfo"/>.
        /// </summary>
        /// <param name="info">
        /// The service information.
        /// </param>
        /// <typeparam name="TService">
        /// The type of service expected.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TService"/>.
        /// </returns>
        protected TService CreateServiceProxy<TService>(IServiceInfo info)
            where TService : class, IVdv301Service
        {
            if (this.IsHttpService<TService>())
            {
                return this.CreateHttpServiceProxy<TService>(info);
            }

            if (typeof(TService) == typeof(ITimeService))
            {
                return (TService)this.CreateTimeServiceProxy(info);
            }

            return this.CreateUdpServiceProxy<TService>(info);
        }

        private TService CreateHttpServiceProxy<TService>(IServiceInfo info)
            where TService : class, IVdv301Service
        {
            this.EnsureHttpServer();

            var path = ArrayUtil.Find(info.Attributes, a => a.Name == Definitions.PathAttribute);
            return ServiceClientProxyFactory.Create<TService>(
                info.Addresses[0].ToString(), info.Port, path != null ? path.Value : null, this.httpServer);
        }

        private ITimeService CreateTimeServiceProxy(IServiceInfo info)
        {
            var server = ArrayUtil.Find(info.Attributes, a => a.Name == Definitions.SntpServerAttribute);
            var address = server == null ? info.Addresses[0].ToString() : server.Value;
            return new TimeService(address, info.Port);
        }

        private TService CreateUdpServiceProxy<TService>(IServiceInfo info)
            where TService : class, IVdv301Service
        {
            var multicast = ArrayUtil.Find(info.Attributes, a => a.Name == Definitions.MulticastAttribute);
            if (multicast == null)
            {
                throw new ArgumentException("Service info doesn't contain a multicast address");
            }

            return ServiceClientProxyFactory.Create<TService>(IPAddress.Parse(multicast.Value), info.Port);
        }

        private void EnsureHttpServer()
        {
            if (this.httpServer != null)
            {
                return;
            }

            this.httpServer = new IbisHttpServer(new IPEndPoint(IPAddress.Any, 0));
            this.httpServer.ValidateRequests = this.validateHttpRequests;
            this.httpServer.ValidateResponses = this.validateHttpResponses;
            this.httpServer.Start();

            foreach (var address in this.httpServer.LocalAddresses)
            {
                this.Logger.Info("Started IBIS-IP HTTP server on {0}:{1}", address, this.httpServer.LocalPort);
            }
        }

        private ServiceRegistrationRequest RegisterHttpService(IVdv301HttpService httpService)
        {
            this.EnsureHttpServer();

            var path = this.httpServer.AddService(httpService);
            this.Logger.Info("Registered {0} on path {1}", httpService.GetType().Name, path);

            return new ServiceRegistrationRequest
                {
                    Port = this.httpServer.LocalPort,
                    Protocol = Definitions.HttpProtocol,
                    Attributes =
                        {
                            new ServiceAttribute(Definitions.VersionAttribute, Definitions.CurrentVersion.ToString()),
                            new ServiceAttribute(Definitions.PathAttribute, path)
                        }
                };
        }

        private ServiceRegistrationRequest RegisterUdpService(IVdv301UdpService udpService)
        {
            if (this.udpServer == null)
            {
                this.udpServer = new IbisUdpServer();
            }

            var endPoint = this.udpServer.AddService(udpService);
            this.Logger.Info("Registered {0} on {1}:{2}", udpService.GetType().Name, endPoint.Address, endPoint.Port);
            return new ServiceRegistrationRequest
                {
                    Port = endPoint.Port,
                    Protocol = Definitions.HttpProtocol,
                    Attributes =
                        {
                            new ServiceAttribute(Definitions.VersionAttribute, Definitions.CurrentVersion.ToString()),
                            new ServiceAttribute(Definitions.MulticastAttribute, endPoint.Address.ToString())
                        }
                };
        }

        /// <summary>
        /// Non-generic base class for <see cref="ServiceQueryBase{T}"/>.
        /// </summary>
        protected abstract class ServiceQueryBase : IDisposable
        {
            /// <summary>
            /// Starts updating this query.
            /// </summary>
            public abstract void Start();

            /// <summary>
            /// Stops updating this query.
            /// </summary>
            public abstract void Stop();

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Base class for all <see cref="IServiceQuery{T}"/> implementations in
        /// subclasses of <see cref="IbisServiceLocatorBase"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of service expected to be returned from this query.
        /// </typeparam>
        protected abstract class ServiceQueryBase<T> : ServiceQueryBase, IServiceQuery<T>
            where T : class, IVdv301Service
        {
            private readonly IbisServiceLocatorBase owner;

            private readonly Dictionary<IServiceInfo, T> services;

            private readonly string serviceName;

            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceQueryBase{T}"/> class.
            /// </summary>
            /// <param name="owner">
            /// The owner.
            /// </param>
            protected ServiceQueryBase(IbisServiceLocatorBase owner)
            {
                this.owner = owner;
                this.services = new Dictionary<IServiceInfo, T>(new ServiceInfoComparer());
                this.serviceName = owner.GetServiceName<T>();

                this.Services = new T[0];
            }

            /// <summary>
            /// Event that is fired when the <see cref="Services"/> list changes.
            /// </summary>
            public event EventHandler ServicesChanged;

            /// <summary>
            /// Gets the list of found services.
            /// </summary>
            public T[] Services { get; private set; }

            /// <summary>
            /// Gets the list of found services.
            /// Subclasses need to implement this method to return the objects from the query (either DNS-SD or Medi).
            /// This method is called by <see cref="UpdateServices"/>.
            /// </summary>
            /// <returns>
            /// The enumeration over all found services.
            /// This is not the same as <see cref="Services"/>, which returns the actual service wrappers.
            /// </returns>
            protected abstract IEnumerable<IServiceInfo> GetFoundServices();

            /// <summary>
            /// Method to be called by subclasses to update the <see cref="Services"/> list using the
            /// <see cref="GetFoundServices"/> method.
            /// </summary>
            protected void UpdateServices()
            {
                lock (this.services)
                {
                    var updated = false;
                    var found = new Dictionary<IServiceInfo, IServiceInfo>(new ServiceInfoComparer());
                    foreach (var info in this.GetFoundServices())
                    {
                        if (!info.Name.StartsWith(this.serviceName))
                        {
                            continue;
                        }

                        if (this.owner.verifyVersion)
                        {
                            var ver = ArrayUtil.Find(info.Attributes, a => a.Name == Definitions.VersionAttribute);
                            if (ver == null || ver.Value != Definitions.CurrentVersion.ToString())
                            {
                                this.owner.Logger.Warn(
                                    "Ignoring {0} from {1} because version {2} <> {3}",
                                    info.Name,
                                    info.Addresses[0].ToString(),
                                    ver,
                                    Definitions.CurrentVersion);
                                continue;
                            }
                        }

                        found.Add(info, info);
                    }

                    // remove all services that are no longer in the query
                    foreach (var existing in new List<IServiceInfo>(this.services.Keys))
                    {
                        if (!found.Remove(existing))
                        {
                            this.owner.Logger.Debug(
                                "Removing {0} from {1}", existing.FullName, existing.Addresses[0].ToString());
                            this.services.Remove(existing);
                            updated = true;
                        }
                    }

                    // add all new services added to the query
                    foreach (var info in found.Keys)
                    {
                        this.owner.Logger.Debug(
                            "Adding {0} from {1}", info.FullName, info.Addresses[0].ToString());
                        var service = this.owner.CreateServiceProxy<T>(info);
                        this.services.Add(info, service);
                        updated = true;
                    }

                    if (!updated)
                    {
                        return;
                    }

                    this.Services = new List<T>(this.services.Values).ToArray();
                }

                var handler = this.ServicesChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private class ServiceInfoComparer : IEqualityComparer<IServiceInfo>
        {
            bool IEqualityComparer<IServiceInfo>.Equals(IServiceInfo x, IServiceInfo y)
            {
                if (!x.FullName.Equals(y.FullName) || x.Port != y.Port)
                {
                    return false;
                }

                var otherAttrs = new List<IServiceAttribute>(y.Attributes);
                foreach (var attribute in x.Attributes)
                {
                    var other = otherAttrs.Find(a => a.Name == attribute.Name);
                    if (other == null || other.Value != attribute.Value)
                    {
                        return false;
                    }

                    otherAttrs.Remove(other);
                }

                if (otherAttrs.Count > 0)
                {
                    return false;
                }

                var otherAddrs = new List<IPAddress>(y.Addresses);
                foreach (var address in x.Addresses)
                {
                    if (!otherAddrs.Remove(address))
                    {
                        return false;
                    }
                }

                return otherAddrs.Count == 0;
            }

            int IEqualityComparer<IServiceInfo>.GetHashCode(IServiceInfo obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        private class TimeService : ITimeService
        {
            public TimeService(string address, int port)
            {
                this.IPAddress = address;
                this.Port = port;
            }

            public string IPAddress { get; private set; }

            public int Port { get; private set; }
        }
    }
}