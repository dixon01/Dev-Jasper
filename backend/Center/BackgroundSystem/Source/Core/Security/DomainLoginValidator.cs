// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainLoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DomainLoginValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Security;

    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Defines a validator for the login.
    /// </summary>
    public class DomainLoginValidator : DatabaseLoginValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan RevalidationTimeout = TimeSpan.FromMinutes(5);

        private readonly Dictionary<int, Tuple<string, DateTime>> validatedUsers =
            new Dictionary<int, Tuple<string, DateTime>>();

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
            if (!string.IsNullOrEmpty(databaseUser.Domain))
            {
                this.ValidateOnDomain(password, databaseUser);
                return;
            }

            base.Validate(databaseUser, password);
        }

        private void ValidateOnDomain(string password, User databaseUser)
        {
            Logger.Trace("Validating username '{0}' on domain '{1}'", databaseUser.Username, databaseUser.Domain);

            Tuple<string, DateTime> validation;
            lock (this.validatedUsers)
            {
                this.validatedUsers.TryGetValue(databaseUser.Id, out validation);
            }

            var hashedPassword = SecurityUtility.Md5(password);
            if (validation != null && validation.Item1.Equals(hashedPassword)
                && validation.Item2 + RevalidationTimeout >= TimeProvider.Current.UtcNow)
            {
                Logger.Trace("Login validated against cached password");
                return;
            }

            bool result;
            try
            {
                Logger.Debug("Validating username '{0}' against '{1}'", databaseUser.Username, databaseUser.Domain);
                using (var pc = new PrincipalContext(ContextType.Domain, databaseUser.Domain))
                {
                    result = pc.ValidateCredentials(databaseUser.Username, password);
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Error during credential validation {0}", e);
                throw new SecurityException("Error during login. Please check the domain of the user.", e);
            }

            if (!result)
            {
                throw new SecurityException("Login not authenticated by the Active Directory");
            }

            Logger.Debug("Login validated against domain '{0}'", databaseUser.Domain);
            lock (this.validatedUsers)
            {
                this.validatedUsers[databaseUser.Id] = new Tuple<string, DateTime>(
                    hashedPassword,
                    TimeProvider.Current.UtcNow);
            }
        }
    }
}