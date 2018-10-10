// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The azure login validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Host
{
    using System;
    using System.IdentityModel.Selectors;
    using System.Security;

    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    using NLog;

    /// <summary>
    /// The azure login validator.
    /// </summary>
    public class AzureLoginValidator : UserNamePasswordValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // This value is always the same.
        private static string aadInstance = "https://login.windows.net/{0}";

        // This value is the GUID part of the Endpoints shown after registering a native client app to the AAD
        private static string tenant = "0c166dd7-6663-4b13-a08b-759dcdb2df1f";

        // This value is the ClientId of the registered app (Under AAD/Applications/)
        private static string clientId = "a7156349-63a7-49d0-9d85-c145d0f78e61";

        private static string authority = string.Format(aadInstance, tenant);

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        public override void Validate(string userName, string password)
        {
            Logger.Info("Validating user");
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new SecurityException("Login validation failed");
            }

            var userCredentials = new UserCredential(userName, password);

            // validate against aad
            var authenticationContext = new AuthenticationContext(authority);
            try
            {
                // To be able to access the management core, the app must be configured to have permissions to
                // Windows Azure Service Management API
                var result = authenticationContext.AcquireToken(
                    "https://management.core.windows.net/",
                    clientId,
                    userCredentials);

                // Clear the token cache, otherwise as long as the token is cached, the password is not checked anymore.
                authenticationContext.TokenCache.Clear();
            }
            catch (Exception ex)
            {
                var message = string.Format("AcquireToken failed: {0}", ex.Message);
                Logger.ErrorException("AcquireToken failed: ", ex);
                throw new SecurityException(message);
            }

            Logger.Info("Login validated");
        }
    }
}
