// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWebDeploymentVariablesEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetWebDeploymentVariablesEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Defines the business logic of the <see cref="GetWebDeploymentVariables"/> activity.
    /// </summary>
    public class GetWebDeploymentVariablesEngine
    {
        /// <summary>
        /// Executes the specified deployment base address.
        /// </summary>
        /// <param name="deployServiceUrl">The deployment base address.</param>
        /// <param name="projectFile">The project file.</param>
        /// <param name="publishedWebsitesConfigurationPath">The published websites configuration path.</param>
        /// <returns>The web deployment variables object.</returns>
        public WebDeploymentVariables Execute(
            string deployServiceUrl, string projectFile, string publishedWebsitesConfigurationPath)
        {
            Contract.Requires(
                !string.IsNullOrEmpty(deployServiceUrl), "The deploy service url can't be null or empty.");
            Contract.Requires(!string.IsNullOrEmpty(projectFile), "The project file can't be null or empty.");
            Contract.Requires(
                !string.IsNullOrEmpty(publishedWebsitesConfigurationPath),
                "The published websites configuration path can't be null or empty.");
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            WebDeploymentVariables variables;
            using (var reader = XmlReader.Create(publishedWebsitesConfigurationPath))
            {
                var provider = new PublishedWebsitesConfigurationReader(reader);
                variables = provider.Read(projectName);
            }

            variables.DeployServiceUrl = deployServiceUrl;
            return variables;
        }
    }
}
