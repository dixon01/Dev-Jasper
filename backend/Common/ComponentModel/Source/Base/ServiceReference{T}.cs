// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceReference{T}.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.ComponentModel.Base
{
    using System;

    /// <summary>
    /// Defines a wrapper for a reference to service used to add metadata information. Optionally, a context object can be provided.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public struct ServiceReference<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceReference&lt;T&gt;"/> struct.
        /// </summary>
        /// <remarks>This class can be used to pass service into a service locator object.</remarks>
        /// <param name="service">The service.</param>
        /// <param name="isLocal">if set to <c>true</c> [is local].</param>
        public ServiceReference(T service, bool isLocal)
            : this()
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            this.Service = service;
            this.IsLocal = isLocal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceReference&lt;T&gt;"/> struct.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceReference(T service)
            : this(service, false)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is local.
        /// </summary>
        /// <value><c>true</c> if this instance is local; otherwise, <c>false</c>.</value>
        public bool IsLocal { get; private set; }

        /// <summary>
        /// Gets the referenced service.
        /// </summary>
        /// <value>The referenced service.</value>
        public T Service { get; private set; }
    }
}
