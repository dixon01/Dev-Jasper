// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AgentConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The update agent configuration.
    /// </summary>
    [Serializable]
    public class AgentConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentConfig"/> class.
        /// </summary>
        public AgentConfig()
        {
            this.Enabled = true;
            this.RestartApplications = new RestartApplicationsConfig();
            this.ShowVisualization = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the local Update Agent.
        /// If the agent is disabled, this application will only forward updates for other applications,
        /// but not actually update the local system.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the unit name. This allows to use a different unit name than the machine name.
        /// If this value is null or empty, the <see cref="Environment.MachineName"/> is used instead.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the installation root directory name. This should be left empty except for testing.
        /// If the value is not set, the value returned by <see cref="PathManager.GetInstallationRoot"/> is used.
        /// </summary>
        public string InstallationRoot { get; set; }

        /// <summary>
        /// Gets or sets the applications to be restarted for specific folder updates.
        /// Multiple applications can be restarted for each folder defined.
        /// </summary>
        public RestartApplicationsConfig RestartApplications { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the progress is to be shown by Update.
        /// Default value is true
        /// </summary>
        [XmlElement]
        public bool ShowVisualization { get; set; }
    }
}