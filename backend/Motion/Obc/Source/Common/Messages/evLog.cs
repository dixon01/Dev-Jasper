// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evLog.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evLog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The log event.
    /// </summary>
    public class evLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evLog"/> class.
        /// </summary>
        public evLog()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evLog"/> class.
        /// </summary>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="logUnit">
        /// The log unit.
        /// </param>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        public evLog(int logLevel, string logUnit, string logMessage)
        {
            this.LogLevel = logLevel;
            this.LogUnit = logUnit;
            this.LogMessage = logMessage;
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public int LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the log unit.
        /// </summary>
        public string LogUnit { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string LogMessage { get; set; }
    }
}