// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SessionMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    using Gorba.Common.Utility.Core;

    using Microsoft.Owin;

    using NLog;

    /// <summary>
    /// The session middleware.
    /// </summary>
    public class SessionMiddleware : PortalMiddlewareBase
    {
        /// <summary>
        /// The session time out.
        /// </summary>
        public static readonly TimeSpan SessionTimeOut = TimeSpan.FromDays(7);

        /// <summary>
        /// The applications key.
        /// </summary>
        public static readonly string ApplicationsKey = "Applications";

        /// <summary>
        /// The session key.
        /// </summary>
        public static readonly string SessionKey = "Session";

        /// <summary>
        /// The system id key.
        /// </summary>
        public static readonly string SystemIdKey = "SystemId";

        /// <summary>
        /// The system id.
        /// </summary>
        internal static readonly string SystemId;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly MemoryCache Sessions = new MemoryCache("Sessions");

        static SessionMiddleware()
        {
            SystemId = Guid.NewGuid().ToString();
            Logger.Info("Created session system id {0}", SystemId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        public SessionMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Gets the cookie options with expire time.
        /// </summary>
        /// <returns>
        /// The <see cref="CookieOptions"/>.
        /// </returns>
        public static CookieOptions GetCookieOptions()
        {
            return new CookieOptions { Expires = TimeProvider.Current.UtcNow.Add(SessionTimeOut) };
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            var systemId = context.Request.Cookies.SingleOrDefault(c => c.Key == SystemIdKey).Value;
            if (string.IsNullOrEmpty(systemId))
            {
                // It's the first time this client accesses this server
                this.AddCookie(context, SystemIdKey, SystemId);
            }
            else if (!string.Equals(SystemId, systemId, StringComparison.InvariantCulture))
            {
                // The system was restarted. Forcing to logoff
                this.ResetCookies(context);
                this.ResetApplications(context);
                this.ResetSession(context);
                if (!context.Request.Path.HasValue)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    // very bad situation. Returning an error to avoid infinite loop
                    Logger.Error("Request without path");
                    return;
                }

                context.Response.Redirect(context.Request.Path.Value);
                Logger.Info("Client connected to a different system.");
                return;
            }

            var sessionId = context.Request.Cookies.SingleOrDefault(c => c.Key == SessionKey).Value;
            if (sessionId == null)
            {
                // Session expired or never started
                sessionId = Guid.NewGuid().ToString();
                Logger.Info("Created session {0}", sessionId);
                this.AddCookie(context, SessionKey, sessionId);
            }

            Dictionary<string, string> session;
            if (Sessions.Contains(sessionId))
            {
                // session found. restoring it
                session = (Dictionary<string, string>)Sessions[sessionId];
                Logger.Trace("Restored session {0}", sessionId);
            }
            else
            {
                // creating the new session object
                session = new Dictionary<string, string>();
                Sessions.Add(sessionId, session, new CacheItemPolicy { SlidingExpiration = SessionTimeOut });
                Logger.Debug(
                    "Initialized session {0}. Request from address '{1}:{2}'",
                    sessionId,
                    context.Request.RemoteIpAddress,
                    context.Request.RemotePort);
            }

            context.Set(RequestContext.Key, new RequestContext(session));
            await this.Next.Invoke(context);
        }
    }
}