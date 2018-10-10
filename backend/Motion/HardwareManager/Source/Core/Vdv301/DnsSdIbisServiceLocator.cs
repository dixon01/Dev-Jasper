// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsSdIbisServiceLocator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DnsSdIbisServiceLocator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Common.IbisIP.Discovery;
    using Gorba.Motion.Common.IbisIP.Discovery.Messages;

    /// <summary>
    /// Implementation of <see cref="IIbisServiceLocator"/> that registers services with DNS-SD.
    /// </summary>
    public class DnsSdIbisServiceLocator : IbisServiceLocatorBase
    {
        private readonly List<ServiceQueryBase> queries = new List<ServiceQueryBase>();

        private readonly MediAddress broadcastAddress;

        private readonly IDnsSdProvider dnsSdProvider;

        private readonly IQuery httpQuery;

        private readonly IQuery udpQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdIbisServiceLocator"/> class.
        /// </summary>
        public DnsSdIbisServiceLocator()
        {
            this.broadcastAddress = new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*");

            Logger.Debug("Starting DNS-SD provider");
            this.dnsSdProvider = new AutoDnsSdProvider();
            this.dnsSdProvider.Start();
            Logger.Info("DNS-SD provider started");

            this.httpQuery = this.dnsSdProvider.CreateQuery(Definitions.HttpProtocol);
            this.httpQuery.ServicesChanged += this.QueryOnServicesChanged;
            this.httpQuery.Start();

            this.udpQuery = this.dnsSdProvider.CreateQuery(Definitions.UdpProtocol);
            this.udpQuery.ServicesChanged += this.QueryOnServicesChanged;
            this.udpQuery.Start();

            MessageDispatcher.Instance.Subscribe<ServiceRegistrationRequest>(this.HandleServiceRegistrationRequest);
            MessageDispatcher.Instance.Subscribe<ServiceListRequest>(this.HandleServiceListRequest);
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
        public override void RegisterService<TService>(TService service)
        {
            var request = this.DoRegisterService(service);
            this.RegisterService(request);
        }

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
        public override IServiceQuery<TService> QueryServices<TService>()
        {
            return new ServiceQuery<TService>(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            MessageDispatcher.Instance.Unsubscribe<ServiceRegistrationRequest>(this.HandleServiceRegistrationRequest);
            MessageDispatcher.Instance.Unsubscribe<ServiceListRequest>(this.HandleServiceListRequest);

            foreach (var query in this.queries.ToArray())
            {
                query.Stop();
            }

            this.queries.Clear();

            this.httpQuery.Stop();
            this.udpQuery.Stop();
            this.dnsSdProvider.Stop();

            base.Dispose();
        }

        private void RegisterService(ServiceRegistrationRequest request)
        {
            var attributes = new Dictionary<string, string>();
            foreach (var attribute in request.Attributes)
            {
                attributes.Add(attribute.Name, attribute.Value);
            }

            this.dnsSdProvider.RegisterService(request.Name, request.Protocol, request.Port, attributes);
        }

        private void QueryOnServicesChanged(object sender, EventArgs e)
        {
            this.SendServiceListResponse();
        }

        private void HandleServiceRegistrationRequest(object sender, MessageEventArgs<ServiceRegistrationRequest> e)
        {
            this.RegisterService(e.Message);
        }

        private void HandleServiceListRequest(object sender, MessageEventArgs<ServiceListRequest> e)
        {
            this.SendServiceListResponse();
        }

        private void SendServiceListResponse()
        {
            var response = new ServiceListResponse();
            foreach (var service in this.httpQuery.Services)
            {
                response.KnownServices.Add(this.ConvertServiceInfo(service));
            }

            foreach (var service in this.udpQuery.Services)
            {
                response.KnownServices.Add(this.ConvertServiceInfo(service));
            }

            MessageDispatcher.Instance.Send(this.broadcastAddress, response);
        }

        private ServiceInfo ConvertServiceInfo(IServiceInfo service)
        {
            return new ServiceInfo
                       {
                           Name = service.Name,
                           Protocol = service.Protocol,
                           Port = service.Port,
                           Addresses = new List<IPAddress>(service.Addresses).ConvertAll(a => a.ToString()),
                           Attributes = new List<IServiceAttribute>(service.Attributes)
                               .ConvertAll(a => new ServiceAttribute(a.Name, a.Value))
                       };
        }

        private class ServiceQuery<T> : ServiceQueryBase<T>
            where T : class, IVdv301Service
        {
            private readonly DnsSdIbisServiceLocator owner;

            private IQuery query;

            public ServiceQuery(DnsSdIbisServiceLocator owner)
                : base(owner)
            {
                this.owner = owner;
            }

            public override void Start()
            {
                if (this.query != null)
                {
                    return;
                }

                this.query = this.owner.IsHttpService<T>() ? this.owner.httpQuery : this.owner.udpQuery;
                this.query.ServicesChanged += this.QueryOnServicesChanged;
                this.UpdateServices();
                lock (this.owner.queries)
                {
                    this.owner.queries.Add(this);
                }
            }

            public override void Stop()
            {
                if (this.query == null)
                {
                    return;
                }

                lock (this.owner.queries)
                {
                    this.owner.queries.Remove(this);
                }

                // this query is shared, so don't dispose it!
                this.query.ServicesChanged -= this.QueryOnServicesChanged;
                this.query = null;
            }

            protected override IEnumerable<IServiceInfo> GetFoundServices()
            {
                return this.query.Services;
            }

            private void QueryOnServicesChanged(object sender, EventArgs e)
            {
                this.UpdateServices();
            }
        }
    }
}