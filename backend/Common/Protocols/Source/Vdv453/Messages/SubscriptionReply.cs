// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionReply.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the VDV453 subscription response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the VDV453 subscription response (AboAntwort).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [XmlRoot(Vdv453Constants.SubscriptionReply, Namespace = "", IsNullable = false)]
    public class SubscriptionReply : Vdv453Message
    {
        /// <summary>
        /// Gets or sets the response status (Bestaetigung)
        /// </summary>
        [XmlElement(Vdv453Constants.Acknowledge, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Acknowledge Acknowledge { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (SubscriptionReply)base.Clone();
            clone.Acknowledge = CloneMember(this.Acknowledge);
            return clone;
        }
    }
}