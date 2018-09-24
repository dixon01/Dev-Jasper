// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceHostWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// The service host wrapper.
    /// </summary>
    public class ServiceHostWrapper : IServiceHost, IDisposable
    {
        private readonly ServiceHost serviceHost;

        private readonly object locker = new object();

        private volatile bool isOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHostWrapper"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="serviceHost">
        /// The service host.
        /// </param>
        public ServiceHostWrapper(string name, ServiceHost serviceHost)
        {
            this.Name = name;
            this.serviceHost = serviceHost;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Opens the host.
        /// </summary>
        public void Open()
        {
            if (this.isOpened)
            {
                return;
            }

            lock (this.locker)
            {
                if (this.isOpened)
                {
                    return;
                }

                this.isOpened = true;
            }

            this.serviceHost.Open();
        }

        /// <summary>
        /// Closes the host.
        /// </summary>
        public void Close()
        {
            if (!this.isOpened)
            {
                return;
            }

            lock (this.locker)
            {
                if (!this.isOpened)
                {
                    return;
                }

                this.isOpened = false;
            }

            this.serviceHost.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="isDisposing">
        /// A flag indicating if we are being disposed of (true) or finalized (false).
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose unmanaged resources here
            }

            this.serviceHost.Close();
        }
    }
}