// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IntegratedLoggingTest.Config
{
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all objects for logging configuration file
    /// </summary>
    [XmlRoot("Logging")]
    public class LoggingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingConfig"/> class.
        /// </summary>
        public LoggingConfig()
        {
            this.LogLevel = LogLevels.Low;
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevels LogLevel { get; set; }
    }
}
