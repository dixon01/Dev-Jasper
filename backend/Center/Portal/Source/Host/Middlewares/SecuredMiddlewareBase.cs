// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecuredMiddlewareBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SecuredMiddlewareBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using Gorba.Center.Common.ServiceModel.Faults;

    using Microsoft.Owin;

    /// <summary>
    /// Base class for any middleware that requires authentication.
    /// </summary>
    public abstract class SecuredMiddlewareBase : PortalMiddlewareBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecuredMiddlewareBase"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        protected SecuredMiddlewareBase(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Handles a not authenticated request.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="faultCode">
        /// The fault code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void HandleNonAuthenticatedRequest(IOwinContext context, string faultCode, string message)
        {
            var redirectString = @"/Login?Error=";
            if (string.Equals(faultCode, FaultCodes.MaintenanceMode)
                || string.Equals(faultCode, FaultCodes.SystemNotAvailable))
            {
                redirectString += faultCode + ";Details=" + (string.IsNullOrEmpty(message) ? "Login failed" : message);
            }
            else
            {
                redirectString += "true";
            }

            context.Response.Redirect(redirectString);
        }
    }
}