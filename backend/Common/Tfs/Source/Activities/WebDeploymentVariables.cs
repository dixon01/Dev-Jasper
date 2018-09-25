// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebDeploymentVariables.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the web deployment variables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    /// <summary>
    /// Defines the web deployment variables.
    /// </summary>
    public class WebDeploymentVariables
    {
        /// <summary>
        /// Gets or sets the path in IIS.
        /// </summary>
        public string DeployIisAppPath { get; set; }

        /// <summary>
        /// Gets or sets the deploy service url.
        /// </summary>
        public string DeployServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            const string Format = "IIS path: {0}";
            var s = string.Format(Format, this.DeployIisAppPath);
            return s;
        }
    }
}
