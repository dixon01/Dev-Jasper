// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpMessageHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpMessageHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// Factory to create <see cref="HttpClientHandler"/>s.
    /// </summary>
    /// <remarks>
    /// The default factory returns a new <see cref="HttpClientHandler"/>.
    /// </remarks>
    public abstract class HttpMessageHandlerFactory
    {
        static HttpMessageHandlerFactory()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static HttpMessageHandlerFactory Current { get; private set; }

        /// <summary>
        /// Sets the given instance as current factory.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void SetCurrent(HttpMessageHandlerFactory instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Resets the current factory to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultHttpMessageHandlerFactory.Instance);
        }

        /// <summary>
        /// Creates a new <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpMessageHandler"/>.
        /// </returns>
        public abstract HttpMessageHandler Create();

        private sealed class DefaultHttpMessageHandlerFactory : HttpMessageHandlerFactory
        {
            static DefaultHttpMessageHandlerFactory()
            {
                Instance = new DefaultHttpMessageHandlerFactory();
            }

            public static DefaultHttpMessageHandlerFactory Instance { get; private set; }

            public override HttpMessageHandler Create()
            {
                return new HttpClientHandler();
            }
        }
    }
}