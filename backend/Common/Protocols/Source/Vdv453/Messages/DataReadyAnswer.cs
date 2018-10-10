// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataReadyAnswer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the data ready acknowledge.
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
    /// Define DataReadyAnswer (DatenBereitAntwort).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DataReadyAnswer, Namespace = "", IsNullable = false)]
    public class DataReadyAnswer : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DataReadyAnswer class.
        /// </summary>
        public DataReadyAnswer()
        {
            this.Acknowledge = new Acknowledge();
        }

        /// <summary>
        /// Gets or sets the response status (Bestaetigung).
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
            var clone = (DataReadyAnswer)base.Clone();
            clone.Acknowledge = CloneMember(this.Acknowledge);
            return clone;
        }
    }
}