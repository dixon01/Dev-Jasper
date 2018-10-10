// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisSubscription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the stop monitoring request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Define DisSubscription (AboAZB).
    /// For more information see VDV specification 453, 03/08, Version 2.3
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [DebuggerStepThrough]
    [Serializable]
    public class DisSubscription : Vdv453Message, IEquatable<DisSubscription>
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.LineId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LineId { get; set; }

        /// <summary>
        /// Gets or sets the direction identifier (RichtungsID).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.DirectionId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DirectionId { get; set; }

        /// <summary>
        /// Gets or sets the preview interval in minutes (Vorschauzeit).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.PreviewTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint PreviewTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of drives (MaxAnzahlFahrten).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.MaxNumOfTrips, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint MaxNumOfTrips { get; set; }

        /// <summary>
        /// Gets or sets the hysteresis (Hysterese).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.Hysteresis, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public uint Hysteresis { get; set; }

        /// <summary>
        /// Gets or sets the maximum text length (MaxTextLaenge).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."),
            XmlElement(Vdv453Constants.MaxTextLength, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint MaxTextLength { get; set; }

        /// <summary>
        /// Gets or sets the valid until time [string] (VerfallZst).
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."), XmlAttribute(Vdv453Constants.ValidUntilTimeStamp)]
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here."), XmlIgnore]
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
            sb.AppendFormat("[DisSubscription Id: {0}, ", this.SubscriptionId);
            sb.AppendFormat("DisId: {0}, LineId: {1}, ", this.DisId, this.LineId);
            sb.AppendFormat("DirectionId: {0}, Hysteresis: {1}, ", this.DirectionId, this.Hysteresis);
            sb.AppendFormat("PreviewTime: {0}, MaxNumOfTrips: {1}, ", this.PreviewTime, this.MaxNumOfTrips);
            sb.AppendFormat("MaxTextLength: {0}, ValidUntil: {1}]", this.MaxTextLength, this.ValidUntilTimeStamp);
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
                hashCode = (hashCode * 397) ^ this.Hysteresis.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.LineId != null ? this.LineId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.MaxNumOfTrips.GetHashCode();
                hashCode = (hashCode * 397) ^ this.MaxTextLength.GetHashCode();
                hashCode = (hashCode * 397) ^ this.PreviewTime.GetHashCode();
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

            return this.Equals((DisSubscription)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DisSubscription other)
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
                   && this.Hysteresis == other.Hysteresis && this.LineId == other.LineId
                   && this.MaxNumOfTrips == other.MaxNumOfTrips && this.MaxTextLength == other.MaxTextLength
                   && this.PreviewTime == other.PreviewTime && this.SubscriptionId == other.SubscriptionId;
        }
    }
}