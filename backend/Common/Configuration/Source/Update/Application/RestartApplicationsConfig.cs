// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestartApplicationsConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for all applications to be restarted based on configured folder updates
    /// </summary>
    [Serializable]
    public class RestartApplicationsConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestartApplicationsConfig"/> class.
        /// </summary>
        public RestartApplicationsConfig()
        {
            this.Dependencies = new List<DependencyConfig>();
        }

        /// <summary>
        /// Gets or sets the list of folder/file updates which triggers a restart of the applications
        /// configured under it.
        /// </summary>
        [XmlElement("Dependency")]
        public List<DependencyConfig> Dependencies { get; set; }
    }
}
