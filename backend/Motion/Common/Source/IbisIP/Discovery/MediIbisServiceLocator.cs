// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediIbisServiceLocator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediIbisServiceLocator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.DnsServiceDiscovery;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Common.IbisIP.Discovery.Messages;

    /// <summary>
    /// <see cref="IIbisServiceLocator"/> that uses Medi to find the services.
    /// </summary>
    public class MediIbisServiceLocator : IbisServiceLocatorBase
    {
        private readonly List<ServiceQueryBase> queries = new List<ServiceQueryBase>();

        private readonly MediAddress broadcastAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediIbisServiceLocator"/> class.
        /// </summary>
        public MediIbisServiceLocator()
        {
            this.broadcastAddress = new MediAddress(MessageDispatcher.Instance.LocalAddress.Unit, "*");
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
            MessageDispatcher.Instance.Send(this.broadcastAddress, request);
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
            foreach (var query in this.queries.ToArray())
            {
                query.Stop();
            }

            base.Dispose();
        }

        private class ServiceQuery<T> : ServiceQueryBase<T>
            where T : class, IVdv301Service
        {
            private readonly MediIbisServiceLocator owner;

            private bool running;

            private ServiceListResponse response;

            public ServiceQuery(MediIbisServiceLocator owner)
                : base(owner)
            {
                this.owner = owner;
            }

            public override void Start()
            {
                if (this.running)
                {
                    return;
                }

                this.running = true;
                lock (this.owner.queries)
                {
                    this.owner.queries.Add(this);
                }

                MessageDispatcher.Instance.Subscribe<ServiceListResponse>(this.HandleServiceListResponse);
                MessageDispatcher.Instance.Send(this.owner.broadcastAddress, new ServiceListRequest());
            }

            public override void Stop()
            {
                if (!this.running)
                {
                    return;
                }

                this.running = false;
                lock (this.owner.queries)
                {
                    this.owner.queries.Remove(this);
                }

                MessageDispatcher.Instance.Unsubscribe<ServiceListResponse>(this.HandleServiceListResponse);
            }

            protected override IEnumerable<IServiceInfo> GetFoundServices()
            {
                return this.response.KnownServices.ConvertAll(i => (IServiceInfo)i);
            }

            private void HandleServiceListResponse(object sender, MessageEventArgs<ServiceListResponse> e)
            {
                this.response = e.Message;
                this.UpdateServices();
            }
        }
    }
}