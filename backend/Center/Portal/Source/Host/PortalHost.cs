// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host
{
    using System;

    using Microsoft.Owin.Hosting;

    using NLog;

    /// <summary>
    /// Defines a host for the Portal.
    /// </summary>
    public class PortalHost : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable webApp;

        private PortalHost(IDisposable webApp)
        {
            if (webApp == null)
            {
                throw new ArgumentNullException("webApp");
            }

            this.webApp = webApp;
        }

        /// <summary>
        /// Creates a host for the Portal listening on all available local addresses on the specified
        /// <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The <see cref="PortalHost"/>.
        /// </returns>
        public static PortalHost Create(string uri)
        {
            try
            {
                Logger.Info("Web server on {0} starting.", uri);
                var app = WebApp.Start<Startup>(uri);
                return new PortalHost(app);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error while starting the web application on uri '{0}'", uri));
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.webApp == null)
            {
                return;
            }

            this.webApp.Dispose();
        }
    }
}