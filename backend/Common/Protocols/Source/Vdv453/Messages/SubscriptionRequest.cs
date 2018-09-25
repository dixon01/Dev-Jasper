// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the VDV453 subscription request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the VDV453 subscription request (AboAnfrage).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.SubscriptionRequest, Namespace = "", IsNullable = false)]
    public class SubscriptionRequest : Vdv453Message
    {
        /// <summary>
        /// Subscription items
        /// </summary>
        private List<Vdv453Message> items;

        /// <summary>
        /// Initializes a new instance of the SubscriptionRequest class.
        /// </summary>
        public SubscriptionRequest()
        {
            this.items = new List<Vdv453Message>();
        }

        /// <summary>
        /// Gets subscription items.
        /// </summary>
        ////AboAZB
        [XmlElement(Vdv453Constants.DisSubscription, typeof(DisSubscription),
            Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        ////AboAZBRef
        [XmlElement(Vdv453Constants.DisRefSubscription, typeof(DisRefSubscription),
            Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        ////AboLoeschen
        [XmlElement(Vdv453Constants.DeleteSubscription, typeof(DeleteSubscription),
            Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        ////AboLoeschenAlle
        [XmlElement(Vdv453Constants.DeleteSubscriptionsAll, typeof(DeleteSubscriptionsAll),
            Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<Vdv453Message> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets subscription sender attribute.
        /// </summary>
        [XmlAttribute(Vdv453Constants.Sender)]
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets recorded at time attribute [DateTime]
        /// </summary>
        [XmlAttribute(Vdv453Constants.TimeStamp)]
        public string TimeStampStr
        {
            get
            {
                return this.TimeStamp.ToString(Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.TimeStamp = DateTime.ParseExact(
                    value, Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets recorded at time attribute [DateTime]
        /// </summary>
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (SubscriptionRequest)base.Clone();
            clone.items = Vdv453Message.CloneListMember(this.Items);
            return clone;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[SubscriptionRequest Sender: {0}, Items: [", this.Sender);
            foreach (var vdv453Message in this.items)
            {
                sb.AppendFormat("{0}, ", vdv453Message);
            }

            sb.AppendFormat("], TimeStamp: {0}]", this.TimeStamp);
            return sb.ToString();
        }
    }
}