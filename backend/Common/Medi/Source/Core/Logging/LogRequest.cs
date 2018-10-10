// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System.Xml.Serialization;

    using NLog;

    /// <summary>
    /// Log request.
    /// Don't use this class outside this assembly, it is only public to support
    /// XML serialization.
    /// </summary>
    public class LogRequest
    {
        /// <summary>
        /// Gets or sets the minimum log level as a string.
        /// This property is only used for serialization.
        /// </summary>
        [XmlElement("MinLevel")]
        public string MinLevelString
        {
            get
            {
                return this.MinLevel == null ? null : this.MinLevel.Name;
            }

            set
            {
                this.MinLevel = LogLevel.FromString(value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        [XmlIgnore]
        public LogLevel MinLevel { get; set; }
    }
}