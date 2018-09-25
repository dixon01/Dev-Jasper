// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStateInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStateInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// A feedback for the current state of an update.
    /// </summary>
    public class UpdateStateInfo
    {
        /// <summary>
        /// The current <see cref="Version"/>.
        /// </summary>
        public static readonly Version CurrentVersion = new Version(2, 2);

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStateInfo"/> class.
        /// </summary>
        public UpdateStateInfo()
        {
            this.Version = CurrentVersion;
            this.UpdateId = new UpdateId();
            this.Folders = new List<FolderUpdateInfo>();
        }

        /// <summary>
        /// Gets or sets the version of the update state info in an XML serializable format.
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
        /// Gets or sets the version of the update state info, currently only version 2.0 is supported.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the update identifier.
        /// </summary>
        public UpdateId UpdateId { get; set; }

        /// <summary>
        /// Gets or sets the unit ID from which this info comes.
        /// </summary>
        public UnitId UnitId { get; set; }

        /// <summary>
        /// Gets or sets the UTC time stamp when the <see cref="UpdateStateInfo"/> was created on the unit.
        /// </summary>
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the UTC activation time as an XML serializable string.
        /// </summary>
        [XmlElement("TimeStamp")]
        public string TimeStampString
        {
            get
            {
                return XmlConvert.ToString(this.TimeStamp, XmlDateTimeSerializationMode.RoundtripKind);
            }

            set
            {
                this.TimeStamp = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
            }
        }

        /// <summary>
        /// Gets or sets the current state of the update on the unit.
        /// </summary>
        public UpdateState State { get; set; }

        /// <summary>
        /// Gets or sets a human readable text saying where the update was received from.
        /// </summary>
        public string UpdateSource { get; set; }

        /// <summary>
        /// Gets or sets a human readable explanation for the state.
        /// DO NOT USE THIS!
        /// Use <see cref="Description"/> instead; this is only available for compatibility with Update 2.0.
        /// </summary>
        public string ErrorReason
        {
            get
            {
                return this.Description;
            }

            set
            {
                this.Description = value;
            }
        }

        /// <summary>
        /// Gets or sets a human readable description of the state.
        /// This is only available if the <see cref="State"/> is one of:
        /// - <see cref="UpdateState.Transferring"/> (only present if partial transfer reporting is available)
        /// - <see cref="UpdateState.Ignored"/>
        /// - <see cref="UpdateState.PartiallyInstalled"/>
        /// - <see cref="UpdateState.TransferFailed"/>
        /// - <see cref="UpdateState.InstallationFailed"/>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the update information about all folders and files currently on the unit.
        /// This is only available if the <see cref="State"/> is one of:
        /// - <see cref="UpdateState.Installed"/>
        /// - <see cref="UpdateState.Ignored"/>
        /// - <see cref="UpdateState.PartiallyInstalled"/>
        /// - <see cref="UpdateState.InstallationFailed"/>
        /// </summary>
        [XmlElement("Folder")]
        public List<FolderUpdateInfo> Folders { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="ErrorReason"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeErrorReason()
        {
            // error reason should never be serialized since it is just a copy of Description
            return false;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0} of {1}/{2} for {3} at {4}",
                this.State,
                this.UpdateId.BackgroundSystemGuid,
                this.UpdateId.UpdateIndex,
                this.UnitId.UnitName,
                this.TimeStampString);
        }
    }
}
