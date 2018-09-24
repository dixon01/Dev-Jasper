// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The login validator used on Azure systems. It validates the user name and password against the Azure
//   Active Directory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    using Gorba.Center.BackgroundSystem.Core.Security;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Common.Utility.Core;

    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    using NLog;

    /// <summary>
    /// The login validator used on Azure systems. It validates the user name and password against the Azure
    /// Active Directory.
    /// </summary>
    public class AzureLoginValidator : DatabaseLoginValidator
    {
        private const string LoginPathFormat = "https://login.windows.net/{0}";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan RevalidationTimeout = TimeSpan.FromMinutes(5);
        private readonly string clientId;

        private readonly string resourceUrl;
        private readonly AuthenticationContext authenticationContext;

        private readonly Dictionary<string, CachedAuthorizationToken> validationCache;

        private readonly bool useActiveDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureLoginValidator"/> class.
        /// </summary>
        /// <param name="tenant">
        /// The active directory tenant.
        /// </param>
        /// <param name="clientId">
        /// The client id.
        /// </param>
        /// <param name="resourceUrl">
        /// The resource Url. This should point to the second application defined within the active directory.
        /// </param>
        public AzureLoginValidator(string tenant, string clientId, string resourceUrl)
        {
            this.validationCache = new Dictionary<string, CachedAuthorizationToken>();
            this.clientId = clientId;
            this.resourceUrl = resourceUrl;
            if (!string.IsNullOrEmpty(tenant) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(resourceUrl))
            {
                Logger.Debug("Using Active Directory for authentication");
                this.useActiveDirectory = true;
                var authority = string.Format(LoginPathFormat, tenant);
                this.authenticationContext = new AuthenticationContext(authority);
            }
            else
            {
                Logger.Warn(
                    "ClientId and/or Tenant are not set to access the Active Directory."
                    + " Only using database authentication.");
            }
        }

        /// <summary>
        /// Validates the specified user with the given password.
        /// </summary>
        /// <param name="databaseUser">
        /// The user from the database.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        protected override void Validate(User databaseUser, string password)
        {
            if (!string.IsNullOrEmpty(databaseUser.Domain) && this.useActiveDirectory)
            {
                this.ValidateOnDomain(databaseUser, password);
                return;
            }

            base.Validate(databaseUser, password);
        }

        private void ValidateOnDomain(User databaseUser, string password)
        {
            Logger.Trace("Validating username '{0}' on domain '{1}'", databaseUser.Username, databaseUser.Domain);
            try
            {
                var username = string.Format("{0}@{1}", databaseUser.Username, databaseUser.Domain);
                var userCredentials = new UserCredential(username, password);
                CachedAuthorizationToken cachedItem;
                lock (this.validationCache)
                {
                    this.validationCache.TryGetValue(databaseUser.Username, out cachedItem);
                }

                if (cachedItem != null)
                {
                    // Password changed since the last request, so delete existing cached item.
                    if (cachedItem.Password.Equals(password)
                        && (cachedItem.CreatedOn + RevalidationTimeout >= TimeProvider.Current.UtcNow))
                    {
                        Logger.Trace("Login validated against cached password");
                        return;
                    }

                    var tokenCacheItems = this.authenticationContext.TokenCache.ReadItems();

                    var currentToken =
                        tokenCacheItems.FirstOrDefault(
                            item => item.AccessToken == cachedItem.CachedToken.AccessToken);
                    if (currentToken != null)
                    {
                        this.authenticationContext.TokenCache.DeleteItem(currentToken);
                    }

                    this.validationCache.Remove(databaseUser.Username);
                }

                // Get a token
                var result = this.authenticationContext.AcquireToken(
                    this.resourceUrl,
                    this.clientId,
                    userCredentials);

                // Update local validation cache
                bool cacheContainsUser;
                lock (this.validationCache)
                {
                    cacheContainsUser = this.validationCache.ContainsKey(databaseUser.Username);
                }

                if (!cacheContainsUser)
                {
                    var cachedToken =
                        this.authenticationContext.TokenCache.ReadItems()
                            .FirstOrDefault(token => token.AccessToken.Equals(result.AccessToken));
                    if (cachedToken != null)
                    {
                        lock (this.validationCache)
                        {
                            this.validationCache[databaseUser.Username] = new CachedAuthorizationToken(
                                password,
                                cachedToken);
                        }
                    }
                }

                Logger.Trace("Login validated");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error during login.");
                throw new SecurityException("Error during login. Please check the domain of the user.");
            }
        }

        private class CachedAuthorizationToken
        {
            public CachedAuthorizationToken(string password, TokenCacheItem item)
            {
                this.Password = password;
                this.CachedToken = item;
                this.CreatedOn = TimeProvider.Current.UtcNow;
            }

            public string Password { get; set; }

            public TokenCacheItem CachedToken { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
