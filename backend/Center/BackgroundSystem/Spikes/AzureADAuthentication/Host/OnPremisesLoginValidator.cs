// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnPremisesLoginValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OnPremisesLoginValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Host
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.IdentityModel.Selectors;
    using System.Security;

    using NLog;

    /// <summary>
    /// The on premises login validator.
    /// </summary>
    public class OnPremisesLoginValidator : UserNamePasswordValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain { get; set; }

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
            Logger.Info("Validating user on domain {0}", this.Domain);
            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain, this.Domain))
                {
                    var result = pc.ValidateCredentials(userName, password);
                    if (result)
                    {
                        Logger.Info("Login validated");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error during login.", e);
                throw new SecurityException(string.Format("Login failed. StackTrace: {0}", e.StackTrace));
            }

            // Login failed
            Logger.Error("Wrong username or password.");
            throw new SecurityException("Wrong username or password.");
        }
    }
}