// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Application
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for launching applications (processes or components).
    /// </summary>
    [Serializable]
    public abstract class ApplicationConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigBase"/> class.
        /// </summary>
        protected ApplicationConfigBase()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the name used to identify the application.
        /// This name has to be unique.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the launch of this application is enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the watchdog.
        /// </summary>
        [XmlAttribute]
        public bool UseWatchdog { get; set; }

        /// <summary>
        /// Gets or sets the application to wait for before launching this application.
        /// If this is set to null, this application will launch immediately (or after <see cref="LaunchDelay"/>)
        /// </summary>
        public LaunchWaitForConfig LaunchWaitFor { get; set; }

        /// <summary>
        /// Gets or sets the launch delay.
        /// Default value is null.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? LaunchDelay { get; set; }

        /// <summary>
        /// Gets or sets the launch delay as an XML compatible string.
        /// </summary>
        [XmlElement("LaunchDelay", DataType = "duration")]
        public string LaunchDelayString
        {
            get
            {
                return this.LaunchDelay == null ? null : XmlConvert.ToString(this.LaunchDelay.Value);
            }

            set
            {
                this.LaunchDelay = value == null ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the re-launch delay.
        /// Default value is null.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? RelaunchDelay { get; set; }

        /// <summary>
        /// Gets or sets the re-launch delay as an XML compatible string.
        /// </summary>
        [XmlElement("RelaunchDelay", DataType = "duration")]
        public string RelaunchDelayString
        {
            get
            {
                return this.RelaunchDelay == null ? null : XmlConvert.ToString(this.RelaunchDelay.Value);
            }

            set
            {
                this.RelaunchDelay = value == null ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Enabled"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeEnabled()
        {
            // only serialize the enabled flag for applications, but not for defaults (they don't contain a name)
            return !string.IsNullOrEmpty(this.Name);
        }
    }
}