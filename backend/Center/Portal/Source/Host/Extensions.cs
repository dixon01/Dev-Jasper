// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host
{
    using Gorba.Center.Portal.Host.Middlewares;

    using Microsoft.Owin;

    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Verifies that the current request is authenticated.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// <c>true</c> if the request is authenticated; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAuthenticated(this IOwinContext context)
        {
            return context.Authentication.User != null && context.Authentication.User.Identity != null
                   && context.Authentication.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Gets the <see cref="RequestContext"/>.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="RequestContext"/>.
        /// </returns>
        public static RequestContext GetRequestContext(this IOwinContext context)
        {
            return context.Get<RequestContext>(RequestContext.Key);
        }
    }
}