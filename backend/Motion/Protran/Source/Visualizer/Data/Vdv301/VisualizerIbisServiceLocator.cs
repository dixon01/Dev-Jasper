// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerIbisServiceLocator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerIbisServiceLocator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Vdv301
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Common.IbisIP;
    using Gorba.Motion.Common.IbisIP.Discovery;

    /// <summary>
    /// Implementation of <see cref="IIbisServiceLocator"/> that provides services for VDV 301 visualization.
    /// </summary>
    public class VisualizerIbisServiceLocator : IIbisServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerIbisServiceLocator"/> class.
        /// </summary>
        public VisualizerIbisServiceLocator()
        {
            this.CustomerInformationService = new CustomerInformationServiceMock();
        }

        /// <summary>
        /// Gets the customer information service mock.
        /// </summary>
        public CustomerInformationServiceMock CustomerInformationService { get; private set; }

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
        public void RegisterService<TService>(TService service) where TService : class, IVdv301Service
        {
            throw new NotSupportedException("Registering services is not supported in Protran Visualizer");
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
        public IServiceQuery<TService> QueryServices<TService>() where TService : class, IVdv301Service
        {
            return new ServiceQuery<TService>(this.GetService<TService>());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        private T GetService<T>()
            where T : class, IVdv301Service
        {
            if (typeof(T) == typeof(ICustomerInformationService))
            {
                return (T)(object)this.CustomerInformationService;
            }

            throw new NotSupportedException("Unknown service type " + typeof(T).Name);
        }

        private class ServiceQuery<T> : IServiceQuery<T>
            where T : class, IVdv301Service
        {
            private readonly T service;

            public ServiceQuery(T service)
            {
                this.service = service;
            }

            public event EventHandler ServicesChanged;

            public T[] Services { get; private set; }

            public void Start()
            {
                this.Services = new[] { this.service };
                this.RaiseServicesChanged();
            }

            public void Stop()
            {
            }

            public void Dispose()
            {
                this.Stop();
            }

            private void RaiseServicesChanged()
            {
                var handler = this.ServicesChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}