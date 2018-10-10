// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalMiddlewareBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalMiddlewareBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using Microsoft.Owin;

    /// <summary>
    /// The portal middleware base.
    /// </summary>
    public abstract class PortalMiddlewareBase : OwinMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortalMiddlewareBase"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        protected PortalMiddlewareBase(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Resets the context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected void Reset(IOwinContext context)
        {
            this.ResetApplications(context);
            this.ResetSession(context);
            this.ResetCookies(context);
            context.Authentication.SignOut();
        }

        /// <summary>
        /// Resets the cookies.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected void ResetCookies(IOwinContext context)
        {
            context.Response.Cookies.Delete(SessionMiddleware.SystemIdKey);
            context.Response.Cookies.Delete(SessionMiddleware.ApplicationsKey);
            context.Response.Cookies.Delete(SessionMiddleware.SessionKey);
        }

        /// <summary>
        /// Resets the session.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected void ResetSession(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            if (requestContext == null)
            {
                return;
            }

            requestContext.Session.Clear();
        }

        /// <summary>
        /// Resets the applications.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected void ResetApplications(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            if (requestContext == null)
            {
                return;
            }

            requestContext.Session[SessionMiddleware.ApplicationsKey] = null;
        }

        /// <summary>
        /// Adds a cookie.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        protected void AddCookie(IOwinContext context, string key, string value)
        {
            context.Response.Cookies.Append(key, value, SessionMiddleware.GetCookieOptions());
        }
    }
}