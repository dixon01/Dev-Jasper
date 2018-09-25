// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    using System;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Object sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// </summary>
    public class ApplicationInfo
    {
        /// <summary>
        /// Gets or sets the id of the application.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application given in the System Manager config.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the application.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the path of the application (the path to the DLL or EXE).
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the state of this application.
        /// </summary>
        public ApplicationState State { get; set; }

        /// <summary>
        /// Gets or sets the amount of RAM used in bytes.
        /// </summary>
        public long RamBytes { get; set; }

        /// <summary>
        /// Gets or sets the CPU usage in percent.
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the window state of the main window.
        /// </summary>
        public ProcessWindowStyle WindowState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the main window has the focus.
        /// </summary>
        public bool HasFocus { get; set; }

        /// <summary>
        /// Gets or sets the last launch reason.
        /// </summary>
        public ApplicationReasonInfo LastLaunchReason { get; set; }

        /// <summary>
        /// Gets or sets the last exit reason.
        /// </summary>
        public ApplicationReasonInfo LastExitReason { get; set; }

        /// <summary>
        /// Gets or sets the start time as an XML serializable string.
        /// </summary>
        [XmlElement("StartTimeUtc")]
        public string StartTimeUtcString
        {
            get
            {
                return this.StartTimeUtc == DateTime.MinValue
                           ? null
                           : XmlConvert.ToString(this.StartTimeUtc, XmlDateTimeSerializationMode.Utc);
            }

            set
            {
                this.StartTimeUtc = string.IsNullOrEmpty(value)
                                        ? DateTime.MinValue
                                        : XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the start time in UTC time.
        /// </summary>
        [XmlIgnore]
        public DateTime StartTimeUtc { get; set; }
    }
}