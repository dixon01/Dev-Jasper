// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisLineSpecialText.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the stop line notice type.
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
    /// Define DisLineSpecialText (AZBLinienspezialtextType).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DisLineSpecialText, Namespace = "", IsNullable = false)]
    public class DisLineSpecialText : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DisLineSpecialText class.
        /// </summary>
        public DisLineSpecialText()
        {
            this.TimeStamp = DateTime.MinValue;
            this.ValidUntilTimeStamp = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the recorded at timestamp (Zst).
        /// </summary>
        [XmlAttribute(Vdv453Constants.TimeStamp)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the valid until time (VerfallZst).
        /// </summary>
        [XmlAttribute(Vdv453Constants.ValidUntilTimeStamp)]
        public DateTime ValidUntilTimeStamp { get; set; }

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
        /// Gets or sets the line text (LinienText).
        /// </summary>
        [XmlElement(Vdv453Constants.LineText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LineText { get; set; }

        /// <summary>
        /// Gets or sets the direction identifier (RichtungsID).
        /// </summary>
        [XmlElement(Vdv453Constants.DirectionId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DirectionId { get; set; }

        /// <summary>
        /// Gets or sets the line notice (Linienspezialtext).
        /// </summary>
        [XmlElement(Vdv453Constants.LineSpecialText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LineSpecialText { get; set; }

        /// <summary>
        /// Gets or sets the priority (Prioritaet).
        /// </summary>
        [XmlElement(Vdv453Constants.Priority, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the speech output (Sprachausgabe).
        /// </summary>
        [XmlElement(Vdv453Constants.SpeechOutput, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] SpeechOutput { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DisLineSpecialText)base.Clone();
            clone.SpeechOutput = CloneMember(this.SpeechOutput);
            return clone;
        }
    }
}