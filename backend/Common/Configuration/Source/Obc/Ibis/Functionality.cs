// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Functionality.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Functionality type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Obc.Common;

    /// <summary>
    /// The IBIS functionality configuration.
    /// </summary>
    [Serializable]
    public class Functionality
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Functionality"/> class.
        /// </summary>
        public Functionality()
        {
            this.DefaultTextStop = "Ende des Dienstes/Fin de Service";
            this.DefaultTextDestination = "Depot";
            this.DefaultLineNumber = 999;
            this.DefaultZoneNumber = 110;

            const string Description = "Destination message will be activated when the door opens. "
                                       + "Transmits a DS036 message with the destination code";
            this.DestinationAnnouncement = new ConfigItem<bool>(false, Description);
        }

        /// <summary>
        /// Gets or sets the destination announcement.
        /// </summary>
        [XmlElement(ElementName = "DestinationAnnouncement")]
        public ConfigItem<bool> DestinationAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent sending date time before synchronization.
        /// As long as the onboard computer hasn’t synchronized its internal clock,
        /// it mustn’t send the vehicle bus telegrams DS005 (time) and DS006 (date).
        /// After the synchronization they are sent again in the common manner.
        /// true=prevent, false=send always
        /// </summary>
        [XmlElement(ElementName = "PreventSendingDateTimeBeforeSynch")]
        public bool PreventSendingDateTimeBeforeSynch { get; set; }

        /// <summary>
        /// Gets or sets the default text that is shown as first stop if no service is active
        /// </summary>
        [XmlElement(ElementName = "DefaultTextStop")]
        public string DefaultTextStop { get; set; }

        /// <summary>
        /// Gets or sets the default text that is shown as destination if no service is active
        /// </summary>
        [XmlElement(ElementName = "DefaultTextDestination")]
        public string DefaultTextDestination { get; set; }

        /// <summary>
        /// Gets or sets the default line number which will be used until a service is loaded
        /// </summary>
        [XmlElement(ElementName = "DefaultLineNumber")]
        public int DefaultLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the default zone number which will be used until a zone number is available
        /// </summary>
        [XmlElement(ElementName = "DefaultZoneNumber")]
        public int DefaultZoneNumber { get; set; }
    }
}