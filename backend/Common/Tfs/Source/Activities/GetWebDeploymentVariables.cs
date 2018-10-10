// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWebDeploymentVariables.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetWebDeploymentVariables type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System.Activities;

    using Microsoft.TeamFoundation.Build.Client;

    /// <summary>
    /// Gets the variables needed for the deployment of a web application from the configuration file.
    /// </summary>
    [BuildActivity(HostEnvironmentOption.Agent)]
    public sealed class GetWebDeploymentVariables : CodeActivity<WebDeploymentVariables>
    {
        /// <summary>
        /// Gets or sets the deployment base address.
        /// </summary>
        /// <value>
        /// The deployment base address.
        /// </value>
        [RequiredArgument]
        public InArgument<string> DeployServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the path of the project file.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> ProjectFile { get; set; }

        /// <summary>
        /// Gets or sets the path to the xml configuration document.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> PublishedWebsitesConfigurationPath { get; set; }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The web deployment variables.</returns>
        protected override WebDeploymentVariables Execute(CodeActivityContext context)
        {
            var deployServiceUrl = this.DeployServiceUrl.Get(context);
            var projectFile = this.ProjectFile.Get(context);
            var publishedWebsitesConfigurationPath = this.PublishedWebsitesConfigurationPath.Get(context);
            var engine = new GetWebDeploymentVariablesEngine();
            var result = engine.Execute(deployServiceUrl, projectFile, publishedWebsitesConfigurationPath);
            return result;
        }
    }
}
