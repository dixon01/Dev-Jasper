// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpsRedirectMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpsRedirectMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Portal.Host.Settings;

    using Microsoft.Owin;

    /// <summary>
    /// Redirects all http requests to https (if enabled).
    /// </summary>
    public class HttpsRedirectMiddleware : PortalMiddlewareBase
    {
        private readonly PortalSettings portalSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpsRedirectMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        public HttpsRedirectMiddleware(OwinMiddleware next)
            : base(next)
        {
            this.portalSettings = PortalSettingsProvider.Current.GetSettings();
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The <see cref="Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Uri.Scheme == "http" && this.portalSettings.EnableHttps)
            {
                var redirect = context.Request.Uri.ToString().Replace("http://", "https://");
                var uri = new UriBuilder(redirect) { Port = this.portalSettings.HttpsPort };
                context.Response.Redirect(uri.ToString());
                return;
            }

            await this.Next.Invoke(context);
        }
    }
}
