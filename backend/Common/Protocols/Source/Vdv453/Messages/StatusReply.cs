// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusReply.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the StatusReply message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// Define "StatusAntwort" 
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.StatusReply, Namespace = "", IsNullable = false)]
    public class StatusReply : Vdv453Message
    {
        /// <summary>
        /// Gets or sets the status field
        /// </summary>
        [XmlElement(Vdv453Constants.Status, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data ready flag is set (DatenBereit).
        /// </summary>
        [XmlElement(Vdv453Constants.DataAvailable, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool DataAvailable { get; set; }

        /// <summary>
        /// Gets or sets the service started time field (StartDienstZst)
        /// </summary>
        [XmlElement(Vdv453Constants.StartServiceTimeStamp, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime StartServiceTimeStamp { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (StatusReply)base.Clone();
            clone.Status = CloneMember(this.Status);
            return clone;
        }
    }
}