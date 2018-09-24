// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the VDV453 display user system message (AZBNachricht).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DisMessage, Namespace = "", IsNullable = false)]
    public class DisMessage : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisMessage"/> class.
        /// </summary>
        public DisMessage()
        {
            this.Messages = new List<Vdv453Message>();
        }

        /// <summary>
        /// Gets or sets the subscription ID.
        /// </summary>
        [XmlAttribute(Vdv453Constants.SubscriptionId)]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the list of sub-messages contained in this message.
        /// </summary>
        [XmlElement(Vdv453Constants.DisSchedule, typeof(DisSchedule), Form = XmlSchemaForm.Unqualified)]
        [XmlElement(Vdv453Constants.DisDeviation, typeof(DisDeviation), Form = XmlSchemaForm.Unqualified)]
        [XmlElement(Vdv453Constants.DisLineSpecialText, typeof(DisLineSpecialText), Form = XmlSchemaForm.Unqualified)]
        [XmlElement(Vdv453Constants.DisLineSpecialTextDelete, typeof(DisLineSpecialTextDelete), Form = XmlSchemaForm.Unqualified)]
        [XmlElement(Vdv453Constants.DisTripDelete, typeof(DisTripDelete), Form = XmlSchemaForm.Unqualified)]
        public List<Vdv453Message> Messages { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DisMessage)base.Clone();
            clone.Messages = CloneListMember(this.Messages);
            return clone;
        }
    }
}
