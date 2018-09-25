// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RequestContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// The request context.
    /// </summary>
    public class RequestContext
    {
        /// <summary>
        /// The key.
        /// </summary>
        internal static readonly string Key = "RequestContext";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestContext"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public RequestContext(Dictionary<string, string> session)
        {
            this.Id = Guid.NewGuid();
            this.Session = session;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public Dictionary<string, string> Session { get; private set; }

        /// <summary>
        /// The get user credentials.
        /// </summary>
        /// <returns>
        /// The <see cref="UserCredentials"/>.
        /// </returns>
        public UserCredentials GetUserCredentials()
        {
            if (!this.Session.ContainsKey("Username") || !this.Session.ContainsKey("Password"))
            {
                return null;
            }

            return new UserCredentials(this.Session["Username"], this.Session["Password"]);
        }
    }
}