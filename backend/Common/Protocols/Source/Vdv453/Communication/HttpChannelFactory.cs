// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpChannelFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Class used to create objects of type <see cref="IHttpChannel"/>.
    /// </summary>
    public abstract class HttpChannelFactory
    {
        private static HttpChannelFactory current = DefaultHttpChannelFactory.Instance;

        /// <summary>
        /// Gets the current.
        /// </summary>
        public static HttpChannelFactory Current
        {
            get
            {
                return HttpChannelFactory.current;
            }

            private set
            {
                HttpChannelFactory.current = value;
            }
        }

        /// <summary>
        /// Overrides the default factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void OverrideDefault(HttpChannelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "The factory can't be null");
            }

            HttpChannelFactory.Current = factory;
        }

        /// <summary>
        /// Resets to the default factory.
        /// </summary>
        public static void ResetToDefault()
        {
            HttpChannelFactory.current = DefaultHttpChannelFactory.Instance;
        }

        /// <summary>
        /// Creates a new <see cref="IHttpChannel"/>.
        /// </summary>
        /// <returns>A new instance of an http channel.</returns>
        public abstract IHttpChannel Create();

        private sealed class DefaultHttpChannelFactory : HttpChannelFactory
        {
            private static DefaultHttpChannelFactory instance;

            public static DefaultHttpChannelFactory Instance
            {
                get
                {
                    if (DefaultHttpChannelFactory.instance == null)
                    {
                        DefaultHttpChannelFactory.instance = new DefaultHttpChannelFactory();
                    }

                    return DefaultHttpChannelFactory.instance;
                }
            }

            public override IHttpChannel Create()
            {
                return new HttpChannel();
            }
        }
    }
}
