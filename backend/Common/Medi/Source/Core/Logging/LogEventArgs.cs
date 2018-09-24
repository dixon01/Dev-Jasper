// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System;
    using System.Xml.Serialization;

    using NLog;

    /// <summary>
    /// Event argument for NLog logging reported by <see cref="ILogObserver"/>.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the timestamp value as a long.
        /// </summary>
        public long TimestampValue
        {
            get
            {
                return this.Timestamp.ToUniversalTime().Ticks;
            }

            set
            {
                this.Timestamp = new DateTime(value, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the timestamp in UTC.
        /// </summary>
        [XmlIgnore]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the log level as a string.
        /// This property is only used for serialization.
        /// </summary>
        public string LevelName
        {
            get
            {
                return this.Level == null ? null : this.Level.Name;
            }

            set
            {
                this.Level = LogLevel.FromString(value);
            }
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        [XmlIgnore]
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// Gets or sets the complete formatted log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception information, this can be null.
        /// </summary>
        public string Exception { get; set; }
    }
}