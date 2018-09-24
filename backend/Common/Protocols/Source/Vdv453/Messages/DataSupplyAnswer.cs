// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSupplyAnswer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the service delivery message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the DataSupplyAnswer message (DatenAbrufenAntwort).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.DataSupplyAnswer, Namespace = "", IsNullable = false)]
    public class DataSupplyAnswer : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DataSupplyAnswer class.
        /// </summary>
        public DataSupplyAnswer()
        {
            this.PendingData = false;
            this.Messages = new List<DisMessage>();
        }

        /// <summary>
        /// Gets or sets the response status (Bestaetigung).
        /// </summary>
        [XmlElement(Vdv453Constants.Acknowledge, Form = XmlSchemaForm.Unqualified)]
        public Acknowledge Acknowledge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the PendingData flag is set or not (WeitereDaten).
        /// </summary>
        [XmlElement(Vdv453Constants.PendingData, Form = XmlSchemaForm.Unqualified)]
        public bool PendingData { get; set; }

        /// <summary>
        /// Gets or sets the list of DIS Messages contained in this message.
        /// </summary>
        [XmlElement(Vdv453Constants.DisMessage, Form = XmlSchemaForm.Unqualified)]
        public List<DisMessage> Messages { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DataSupplyAnswer)base.Clone();
            clone.Acknowledge = CloneMember(this.Acknowledge);
            clone.Messages = CloneListMember(this.Messages);
            return clone;
        }
    }
}