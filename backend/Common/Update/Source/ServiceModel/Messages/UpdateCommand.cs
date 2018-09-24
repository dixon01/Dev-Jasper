// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommand.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// A command sent from the application that created the update to a certain unit.
    /// This command tells the unit to perform an update.
    /// </summary>
    [Serializable]
    public class UpdateCommand
    {
        /// <summary>
        /// The current <see cref="Version"/>.
        /// </summary>
        public static readonly Version CurrentVersion = new Version(2, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand"/> class.
        /// </summary>
        public UpdateCommand()
        {
            this.Version = CurrentVersion;
            this.UpdateId = new UpdateId();
            this.Folders = new List<FolderUpdate>();
            this.InstallAfterBoot = false;
        }

        /// <summary>
        /// Gets or sets the version of the update command in an XML serializable format.
        /// </summary>
        [XmlAttribute("Version")]
        public string XmlVersion
        {
            get
            {
                return this.Version.ToString();
            }

            set
            {
                this.Version = new Version(value);
            }
        }

        /// <summary>
        /// Gets or sets the version of the update command, currently only version 2.0 is supported.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the update identifier.
        /// </summary>
        public UpdateId UpdateId { get; set; }

        /// <summary>
        /// Gets or sets the unit ID for which this update is destined.
        /// </summary>
        public UnitId UnitId { get; set; }

        /// <summary>
        /// Gets or sets the UTC time at which this update should be activated on the unit.
        /// </summary>
        [XmlIgnore]
        public DateTime ActivateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC activation time as an XML serializable string.
        /// </summary>
        [XmlElement("ActivateTime")]
        public string ActivateTimeString
        {
            get
            {
                return XmlConvert.ToString(this.ActivateTime, XmlDateTimeSerializationMode.RoundtripKind);
            }

            set
            {
                this.ActivateTime = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to install the command after reboot of the system if it is true.
        /// Default value is false
        /// </summary>
        public bool InstallAfterBoot { get; set; }

        /// <summary>
        /// Gets or sets the pre-installation commands.
        /// These commands are executed before the installation starts.
        /// </summary>
        public RunCommands PreInstallation { get; set; }

        /// <summary>
        /// Gets or sets the root folders of this update.
        /// </summary>
        [XmlElement("Folder")]
        public List<FolderUpdate> Folders { get; set; }

        /// <summary>
        /// Gets or sets the post-installation commands.
        /// These commands are executed after the installation completed successfully.
        /// </summary>
        public RunCommands PostInstallation { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}/{1} for {2}", this.UpdateId.BackgroundSystemGuid, this.UpdateId.UpdateIndex, this.UnitId.UnitName);
        }
    }
}