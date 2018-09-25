// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableServiceHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisposableServiceHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// Defines a service host based on a disposable service.
    /// </summary>
    public class DisposableServiceHost : ServiceHostWrapper
    {
        private readonly IDisposable service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableServiceHost"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="serviceHost">
        /// The service host.
        /// </param>
        public DisposableServiceHost(string name, IDisposable service, ServiceHost serviceHost)
            : base(name, serviceHost)
        {
            this.service = service;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="isDisposing">
        /// A flag indicating if we are being disposed of (true) or finalized (false).
        /// </param>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose unmanaged resources here
            }

            this.Close();

            if (this.service != null)
            {
                this.service.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}