// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisLineSpecialTextDelete.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the stop line notice cancellation type.
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
    /// Define stop line notice cancellation (AZBLinienspezialtextLoeschen).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DisLineSpecialTextDelete, Namespace = "", IsNullable = false)]
    public class DisLineSpecialTextDelete : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DisLineSpecialTextDelete class.
        /// </summary>
        public DisLineSpecialTextDelete()
        {
            this.TimeStamp = DateTime.MinValue;
        }

        /// <summary>
        /// Gets or sets the recorded at timestamp (Zst).
        /// </summary>
        [XmlAttribute(Vdv453Constants.TimeStamp)]
        public DateTime TimeStamp { get; set; }

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
    }
}