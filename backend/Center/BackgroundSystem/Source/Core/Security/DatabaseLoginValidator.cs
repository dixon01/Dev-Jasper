// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseLoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DatabaseLoginValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    using System;
    using System.Security;

    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Login validator that just verifies the password stored in the database.
    /// </summary>
    public class DatabaseLoginValidator : LoginValidatorBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            var hashedPassword = SecurityUtility.Md5(password);
            Logger.Trace(
                "Validating username '{0}' with (hashed) password '{1}' from database",
                databaseUser.Username,
                hashedPassword);
            if (!databaseUser.HashedPassword.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityException("Login not authenticated by the membership service");
            }

            Logger.Trace("Login validated");
        }
    }
}