// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisRefSubscription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the stop timetable request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines DisRefSubscription (AboAZBRef)
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), GeneratedCode("xsd", "2.0.50727.42")]
    [DebuggerStepThrough]
    [Serializable]
    public class DisRefSubscription : Vdv453Message, IEquatable<DisRefSubscription>
    {
        /// <summary>
        /// Gets or sets the subscription identifier (AboID).
        /// </summary>
        [XmlAttribute(Vdv453Constants.SubscriptionId)]
        public uint SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the station identifier (AZBID).
        /// </summary>
        [XmlElement(Vdv453Constants.DisId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DisId { get; set; }

        /// <summary>
        /// Gets or sets the line identifier (LinienID).
        /// </summary>
        [XmlElement(Vdv453Constants.LineId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LineId { get; set; }

        /// <summary>
        /// Gets or sets the direction identifier (RichtungsID).
        /// </summary>
        [XmlElement(Vdv453Constants.DirectionId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DirectionId { get; set; }

        /// <summary>
        /// Gets or sets the earliest departure time (FruehesteAbfahrtszeit).
        /// </summary>
        [XmlElement(Vdv453Constants.EarliestDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EarliestDepartureTimeStr
        {
            get
            {
                return this.EarliestDepartureTime.ToString(
                    Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.EarliestDepartureTime = DateTime.ParseExact(
                    value, Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the earliest departure time (FruehesteAbfahrtszeit).
        /// </summary>
        [XmlIgnore]
        public DateTime EarliestDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the latest departure time (SpaetesteAbfahrtszeit).
        /// </summary>
        [XmlElement(Vdv453Constants.LatestDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LatestDepartureTimeStr
        {
            get
            {
                return this.LatestDepartureTime.ToString(Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.LatestDepartureTime = DateTime.ParseExact(
                    value, Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the latest departure time (SpaetesteAbfahrtszeit).
        /// </summary>
        [XmlIgnore]
        public DateTime LatestDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the valid until time [string] (VerfallZst).
        /// </summary>
        [XmlAttribute(Vdv453Constants.ValidUntilTimeStamp)]
        public string ValidUntilTimeStampStr
        {
            get
            {
                return this.ValidUntilTimeStamp.ToString(Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.ValidUntilTimeStamp = DateTime.ParseExact(
                    value, Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the valid until time (VerfallZst).
        /// </summary>
        [XmlIgnore]
        public DateTime ValidUntilTimeStamp { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[DisRefSubscription Id: {0}, DisId: {1}, ", this.SubscriptionId, this.DisId);
            sb.AppendFormat("LineId: {0}, DirectionId: {1}, ", this.LineId, this.DirectionId);
            sb.AppendFormat("EarliestDeparture: {0}, ", this.EarliestDepartureTime.TimeOfDay);
            sb.AppendFormat("LatestDeparture: {0}, ", this.LatestDepartureTime.TimeOfDay);
            sb.AppendFormat("ValidUntil: {0}]", this.ValidUntilTimeStamp);
            return sb.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.SubscriptionId.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.DirectionId != null ? this.DirectionId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.DisId != null ? this.DisId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.EarliestDepartureTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.LineId != null ? this.LineId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.LatestDepartureTime.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ValidUntilTimeStamp.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((DisRefSubscription)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DisRefSubscription other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.DirectionId == other.DirectionId && this.DisId == other.DisId
                   && this.LineId == other.LineId
                   && this.SubscriptionId == other.SubscriptionId;
        }
    }
}