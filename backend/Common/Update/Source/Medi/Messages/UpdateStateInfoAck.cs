// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStateInfoAck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStateInfoAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi.Messages
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// The update state information acknowledge message.
    /// This message is sent from the provider to the client to tell that
    /// a certain state has been processed in the provider.
    /// </summary>
    public class UpdateStateInfoAck
    {
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
        /// Gets or sets the UTC time stamp as an XML serializable string.
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