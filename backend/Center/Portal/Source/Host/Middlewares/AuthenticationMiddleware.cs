// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigurationContentMiddlewareAuthenticationMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Faults;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Portal.Host.Configuration;

    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;

    using NLog;

    /// <summary>
    /// The authentication middleware.
    /// Handles Login/Logoff operations and requires authentication for downloading applications.
    /// </summary>
    public class AuthenticationMiddleware : SecuredMiddlewareBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        public AuthenticationMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            Logger.Trace("Invoking AuthenticationMiddleware for request {0}", requestContext.Id);
            try
            {
                if (!context.Request.Path.HasValue)
                {
                    await this.Next.Invoke(context);
                    return;
                }

                switch (context.Request.Path.Value.ToLower())
                {
                    case "/logoff":
                        this.ResetSession(context);
                        this.ResetCookies(context);
                        context.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                        context.Response.Redirect("/");
                        return;
                    case "/login":
                        if (context.Request.Method.ToUpper() == "POST")
                        {
                            await this.HandleLoginPath(context);
                            return;
                        }

                        break;
                    case "/applications":
                        if (!context.IsAuthenticated())
                        {
                            context.Authentication.Challenge(CookieAuthenticationDefaults.AuthenticationType);
                            return;
                        }

                        var applications = requestContext.Session.ContainsKey(SessionMiddleware.ApplicationsKey)
                                               ? requestContext.Session[SessionMiddleware.ApplicationsKey]
                                               : null;
                        var userCredentials = requestContext.GetUserCredentials();
                        if (userCredentials == null)
                        {
                            context.Response.Redirect("/Login?ReturnUrl=%2FApplications");
                            return;
                        }

                        var user =
                            await
                            this.AuthenticateAsync(
                                context,
                                userCredentials,
                                applications == null);
                        if (user == null)
                        {
                            return;
                        }

                        break;
                }

                await this.Next.Invoke(context);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                HandleError(context, exception);
            }
        }

        private static void HandleError(IOwinContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (exception == null)
            {
                return;
            }

            Logger.Error(exception,string.Format("Error while handling the request '{0}'", context.Request.Path));
        }

        private async Task<User> AuthenticateAsync(
            IOwinContext context,
            UserCredentials userCredentials,
            bool loadApplications)
        {
            try
            {
                User user;
                using (var channelScope = ChannelScopeFactory<IMembershipService>.Current.Create(userCredentials))
                {
                    user = await channelScope.Channel.AuthenticateUserAsync();
                    if (user == null)
                    {
                        this.Reset(context);
                        context.Response.Redirect("/Login");
                        return null;
                    }
                }

                if (loadApplications)
                {
                    await this.GetApplicationsAsync(context, userCredentials, user);
                }

                return user;
            }
            catch (InvalidBackgroundSystemConfigurationException exception)
            {
                Logger.Error("Invalid configuration", exception);
                context.Response.Redirect("/Error?Details=Invalid%20configuration");
                this.ResetApplications(context);
            }
            catch (MessageSecurityException exception)
            {
                this.HandleMessageSecurityException(context, exception);
                this.ResetApplications(context);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during login");
                context.Response.Redirect("/Login?Error=true");
                this.ResetApplications(context);
            }

            return null;
        }

        private async Task HandleLoginPath(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            var formData = await context.Request.ReadFormAsync();
            var password = formData["Password"];
            var username = formData["Username"];
            var userCredentials = new UserCredentials(username, password);
            var user = await this.AuthenticateAsync(context, userCredentials, true);
            if (user == null)
            {
                return;
            }

            var identity = new GenericIdentity(
                username,
                CookieAuthenticationDefaults.AuthenticationType);
            context.Authentication.SignIn(identity);
            requestContext.Session["Username"] = username;
            requestContext.Session["Password"] = password;
            context.Response.Redirect("/Applications");
        }

        private async Task GetApplicationsAsync(IOwinContext context, UserCredentials userCredentials, User user)
        {
            IEnumerable<AssociationTenantUserUserRole> associations;
            using (
                var scope =
                    ChannelScopeFactory<IAssociationTenantUserUserRoleDataService>.Current.Create(userCredentials))
            {
                associations =
                    (await
                     scope.Channel.QueryAsync(
                         AssociationTenantUserUserRoleQuery.Create()
                         .IncludeUserRole(UserRoleFilter.Create().IncludeAuthorizations())
                         .WithUser(user))).ToList();
            }

            var applicationScopes = new[] { DataScope.CenterAdmin, DataScope.CenterDiag, DataScope.CenterMedia };
            var allowedApplications =
                associations.SelectMany(a => a.UserRole.Authorizations)
                    .Where(a => a.Permission == Permission.Interact && applicationScopes.Contains(a.DataScope))
                    .Select(a => a.DataScope.ToString())
                    .Distinct()
                    .DefaultIfEmpty();
            var applications = allowedApplications.Aggregate((s1, s2) => s1 + "," + s2);
            Logger.Debug("Found the following application(s): {0}", applications);
            this.AddCookie(context, SessionMiddleware.ApplicationsKey, applications ?? string.Empty);
            var requestContext = context.GetRequestContext();
            requestContext.Session[SessionMiddleware.ApplicationsKey] = applications;
        }

        private void HandleMessageSecurityException(IOwinContext context, MessageSecurityException exception)
        {
            var inner = exception.InnerException as FaultException;
            string errorCode;
            string message = null;
            if (inner == null)
            {
                Logger.Warn(exception, "Couldn't login. Inner exception is null");
                errorCode = null;
            }
            else if (string.Equals(inner.Code.Name, FaultCodes.MaintenanceMode))
            {
                var redirectString = @"/Maintenance?Details=";
                redirectString = redirectString
                                 + (string.IsNullOrEmpty(inner.Message) ? "No reason found." : inner.Message);

                Logger.Warn(exception, "Couldn't login, system is in maintenance mode");
                context.Response.Redirect(redirectString);
                return;
            }
            else if (string.Equals(inner.Code.Name, FaultCodes.SystemNotAvailable))
            {
                Logger.Warn(exception, "Couldn't login, system is not available");
                errorCode = FaultCodes.SystemNotAvailable;
                message = "The system is not available.";
            }
            else
            {
                Logger.Warn(exception, "Couldn't login");
                errorCode = null;
            }

            this.HandleNonAuthenticatedRequest(context, errorCode, message);
        }
    }
}