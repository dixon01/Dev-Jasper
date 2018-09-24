// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Authentication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Common
{
    using System;

    /// <summary>
    /// Container of all the required information to
    /// perform an authentication process with a remote server.
    /// </summary>
    [Serializable]
    public class AuthenticationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationConfig"/> class.
        /// </summary>
        public AuthenticationConfig()
        {
            this.Login = string.Empty;
            this.Password = string.Empty;
        }

        /// <summary>
        /// Gets or sets the user's login for the
        /// authentication process.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the user's password for the
        /// authentication process.
        /// </summary>
        public string Password { get; set; }
    }
}
