// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadSafeBonjourProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ThreadSafeBonjourProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// Wrapper around <see cref="BonjourProvider"/> that executes everything in a single STA thread.
    /// Use this wrapper if you are unsure if you will be calling the DNS-SD provider from different threads.
    /// </summary>
    public partial class ThreadSafeBonjourProvider : IDnsSdProvider
    {
        private readonly IProducerConsumerQueue<ThreadStart> queue;

        private BonjourProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeBonjourProvider"/> class.
        /// </summary>
        public ThreadSafeBonjourProvider()
        {
            this.queue = new MessageLoopProducerConsumerQueue<ThreadStart>(this.Consume);
        }

        private delegate T Method<out T>();

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public void Start()
        {
            if (this.provider != null)
            {
                return;
            }

            this.queue.StartConsumer();
            this.Execute(
                () =>
                    {
                        this.provider = new BonjourProvider();
                        this.provider.Start();
                    });
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public void Stop()
        {
            if (this.provider == null)
            {
                return;
            }

            this.Execute(this.provider.Stop);
            this.provider = null;

            this.queue.StopConsumer();
        }

        /// <summary>
        /// Registers a new service with the given identity.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="protocol">
        /// The protocol name (without ".local").
        /// </param>
        /// <param name="port">
        /// The port number.
        /// </param>
        /// <param name="attributes">
        /// The service attributes.
        /// </param>
        /// <returns>
        /// The service registration to be used with <see cref="IDnsSdProvider.DeregisterService"/>.
        /// </returns>
        public IServiceRegistration RegisterService(
            string serviceName, string protocol, int port, IDictionary<string, string> attributes)
        {
            return this.Execute(() => this.provider.RegisterService(serviceName, protocol, port, attributes));
        }

        /// <summary>
        /// Deregisters a previously registered service.
        /// <seealso cref="IDnsSdProvider.RegisterService"/>
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="registration"/> didn't come from <see cref="IDnsSdProvider.RegisterService"/>.
        /// </exception>
        public void DeregisterService(IServiceRegistration registration)
        {
            this.Execute(() => this.provider.DeregisterService(registration));
        }

        /// <summary>
        /// Creates a query for all services of a protocol.
        /// You need to call <see cref="IQuery.Start"/> on the returned object to start getting services.
        /// </summary>
        /// <param name="protocol">
        /// The protocol name without the ".local".
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/> that can be used to get the results.
        /// </returns>
        public IQuery CreateQuery(string protocol)
        {
            return this.Execute(() => new ThreadSafeQuery(this.provider.CreateQuery(protocol), this));
        }

        private void Consume(ThreadStart method)
        {
            method();
        }

        private void Execute(ThreadStart method)
        {
            this.Execute(
                () =>
                    {
                        method();
                        return 0;
                    });
        }

        private T Execute<T>(Method<T> method)
        {
            var result = this.BeginExecute(method, null, null);
            return this.EndExecute<T>(result);
        }

        private IAsyncResult BeginExecute<T>(Method<T> method, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<T>(callback, state);
            this.queue.Enqueue(
                () =>
                    {
                        try
                        {
                            result.Complete(method(), false);
                        }
                        catch (Exception ex)
                        {
                            result.CompleteException(ex, false);
                        }
                    });
            return result;
        }

        private T EndExecute<T>(IAsyncResult ar)
        {
            var result = (SimpleAsyncResult<T>)ar;
            return result.Value;
        }

        private class ThreadSafeQuery : IQuery
        {
            private readonly IQuery query;

            private readonly ThreadSafeBonjourProvider owner;

            public ThreadSafeQuery(IQuery query, ThreadSafeBonjourProvider owner)
            {
                this.query = query;
                this.owner = owner;

                this.query.ServicesChanged += this.QueryOnServicesChanged;
            }

            public event EventHandler ServicesChanged;

            public IServiceInfo[] Services
            {
                get
                {
                    return this.query.Services;
                }
            }

            public void Start()
            {
                this.owner.Execute(this.query.Start);
            }

            public void Stop()
            {
                this.owner.Execute(this.query.Stop);
            }

            public void Dispose()
            {
                this.query.ServicesChanged -= this.QueryOnServicesChanged;
                this.owner.Execute(() => this.query.Dispose());
            }

            private void QueryOnServicesChanged(object sender, EventArgs e)
            {
                var handler = this.ServicesChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}
