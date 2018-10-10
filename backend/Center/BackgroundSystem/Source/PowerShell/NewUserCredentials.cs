// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewUserCredentials.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NewUserCredentials type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell
{
    using System.Management.Automation;

    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// Writes a new <see cref="UserCredentials"/> object to the pipeline.
    /// </summary>
    [Cmdlet(VerbsCommon.New, CmdletNouns.UserCredentials)]
    public class NewUserCredentials : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Password", Position = 2)]
        public string Password { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var userCredentials = new UserCredentials(this.Username, this.Password);
            this.WriteObject(userCredentials);
        }
    }
}