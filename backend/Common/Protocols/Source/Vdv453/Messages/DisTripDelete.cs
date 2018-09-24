// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisTripDelete.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the dis delete trip message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    /// <summary>
    /// Define DisTripDelete Type (AZBFahrtLoeschen).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DisTripDelete, Namespace = "", IsNullable = false)]
    public class DisTripDelete : Vdv453Message
    {
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
        /// Gets or sets the drive identifier (FahrtID).
        /// </summary>
        [XmlElement(Vdv453Constants.TripId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TripId TripId { get; set; }

        /// <summary>
        /// Gets or sets the visit number (HstSeqZaehler).
        /// </summary>
        [XmlElement(Vdv453Constants.StopSeqCount, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StopSeqCount { get; set; }

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
        /// Gets or sets the direction text (RichtungsText).
        /// </summary>
        [XmlElement(Vdv453Constants.DirectionText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DirectionText { get; set; }

        /// <summary>
        /// Gets or sets the clear down reference(AbmeldeID).
        /// </summary>
        [XmlElement(Vdv453Constants.DepartureNoticeId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint DepartureNoticeId { get; set; }

        /// <summary>
        /// Gets or sets the reason for the cancellation (Ursache).
        /// </summary>
        [XmlElement(Vdv453Constants.Reason, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Reason { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DisTripDelete)base.Clone();
            clone.TripId = Vdv453Message.CloneMember(this.TripId);
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
            const string Format = "[DisTripDelete LineId:'{0}', LineText:'{1}', DirectionId:'{2}', DirectionText:'{3}', DisId:'{4}', StopSeqCount: '{5}', TripId:'{6}', TimeStamp:{7}]";
            return string.Format(
                Format,
                this.LineId,
                this.LineText,
                this.DirectionId,
                this.DirectionText,
                this.DisId,
                this.StopSeqCount,
                this.TripId,
                this.TimeStamp);
        }
    }
}