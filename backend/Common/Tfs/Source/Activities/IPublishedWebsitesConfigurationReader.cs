// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPublishedWebsitesConfigurationReader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPublishedWebsitesConfigurationReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    /// <summary>
    /// Defines an object to read the configuration for published websites.
    /// </summary>
    public interface IPublishedWebsitesConfigurationReader
    {
        /// <summary>
        /// Reads the variables for the given website name.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>
        /// The <see cref="WebDeploymentVariables"/>.
        /// </returns>
        WebDeploymentVariables Read(string projectName);
    }
}
