// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System;
    using System.IdentityModel.Selectors;
    using System.Security;

    using WcfAuthenticatedSession.ServiceModel;

    /// <summary>
    /// Defines a validator for the login.
    /// </summary>
    public class LoginValidator : UserNamePasswordValidator
    {
        /// <summary>
        /// When overridden in a derived class, validates the specified username and password.
        /// </summary>
        /// <param name="userName">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        public override void Validate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new SecurityException("Login validation failed");
            }

            Users value;
            if (!Enum.TryParse(userName, out value))
            {
                throw new SecurityException("Login not authenticated by the membership service");
            }

            switch (value)
            {
                case Users.Undefined:
                    break;
                case Users.Reader:
                case Users.Writer:
                case Users.God:
                    return;
                case Users.Unauthorized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new SecurityException("Login not authenticated by the membership service");
        }
    }
}