// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSubscription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VDV453 delete subscription request (AboLoeschen).
//   For more information see VDVSchriften 453, 03/08, Version 2.3
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the VDV453 delete subscription request (AboLoeschen).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.DeleteSubscription, Namespace = "", IsNullable = false)]
    public class DeleteSubscription : Vdv453Message, IEquatable<DeleteSubscription>
    {
        /// <summary>
        /// Gets or sets the subscription identifier (AboID) as a string.
        /// </summary>
        [XmlText]
        public string SubscriptionIdText
        {
            get
            {
                return this.SubscriptionId.ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                this.SubscriptionId = uint.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets the subscription identifier (AboID).
        /// </summary>
        [XmlIgnore]
        public uint SubscriptionId { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[DeleteSubscription Id: {0}]", this.SubscriptionId);
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
                hashCode = (hashCode * 397) ^ (this.SubscriptionIdText != null ?
                    this.SubscriptionIdText.GetHashCode() : 0);
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

            return this.Equals((DeleteSubscription)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DeleteSubscription other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.SubscriptionId == other.SubscriptionId && this.SubscriptionIdText == other.SubscriptionIdText;
        }
    }
}