// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostingSettings.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the hosting specific configuration of the BackgroundSystem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Settings
{
    /// <summary>
    /// Defines the hosting specific configuration of the BackgroundSystem.
    /// </summary>
    public class HostingSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostingSettings"/> class.
        /// </summary>
        /// <param name="resourcesPath">
        /// The resources path.
        /// </param>
        /// <param name="contentResourcesPath">
        /// The content Resources Path.
        /// </param>
        public HostingSettings(string resourcesPath, string contentResourcesPath)
        {
            this.ResourcesPath = resourcesPath;
            this.ContentResourcesPath = contentResourcesPath;
        }

        /// <summary>
        /// Gets the root path where resources are stored.
        /// </summary>
        public string ResourcesPath { get; private set; }

        /// <summary>
        /// Gets the root path where content resources are stored.
        /// </summary>
        public string ContentResourcesPath { get; private set; }
    }
}