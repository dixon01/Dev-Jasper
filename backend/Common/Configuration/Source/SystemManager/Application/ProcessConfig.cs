// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Application
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Limits;

    /// <summary>
    /// Configuration to launch a process.
    /// </summary>
    [Serializable]
    public class ProcessConfig : ApplicationConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessConfig"/> class.
        /// </summary>
        public ProcessConfig()
        {
            this.KillIfRunning = true;
            this.ExitTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Gets or sets the executable path (location where to find the EXE).
        /// </summary>
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// If this is null, the directory of <see cref="ExecutablePath"/> is taken.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the command line arguments provided to the process.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the window mode in which to start the application.
        /// </summary>
        public ProcessWindowStyle? WindowMode { get; set; }

        /// <summary>
        /// Gets or sets the priority at which to run the process.
        /// </summary>
        public ProcessPriorityClass? Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to kill any existing process with the
        /// same <see cref="ExecutablePath"/> before starting this process.
        /// </summary>
        [DefaultValue(true)]
        public bool KillIfRunning { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before a process is killed when not
        /// responding to the exit command (or closing the main window).
        /// </summary>
        [XmlIgnore]
        public TimeSpan ExitTimeout { get; set; }

        /// <summary>
        /// Gets or sets the exit timeout as an XML serializable string.
        /// </summary>
        [XmlElement("ExitTimeout", DataType = "duration")]
        public string ExitTimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.ExitTimeout);
            }

            set
            {
                this.ExitTimeout = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the RAM limit configuration.
        /// </summary>
        public ApplicationRamLimitConfig RamLimit { get; set; }

        /// <summary>
        /// Gets or sets the CPU limit configuration.
        /// </summary>
        public CpuLimitConfig CpuLimit { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="WindowMode"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeWindowMode()
        {
            return this.WindowMode.HasValue;
        }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Priority"/> should be serialized.
        /// </returns>
        public bool ShouldSerializePriority()
        {
            return this.Priority.HasValue;
        }
    }
}