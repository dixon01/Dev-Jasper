// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisSchedule.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisSchedule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    /// <summary>
    /// Define DisSchedule Type (AZBFahrplan)
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression " + "is OK here.")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.DisSchedule, Namespace = "", IsNullable = false)]
    public class DisSchedule : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DisSchedule class.
        /// </summary>
        public DisSchedule()
        {
            this.TimeStamp = DateTime.MinValue;
            this.ScheduledDisArrivalTime = DateTime.MinValue;
            this.ScheduledDisDepartureTime = DateTime.MinValue;
            this.TripInfo = new TripInfo();
        }

        /// <summary>
        /// Gets or sets the recorded at time stamp (Zst).
        /// </summary>
        [XmlAttribute(Vdv453Constants.TimeStamp)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the station identifier (AZBID).
        /// </summary>
        [XmlElement(Vdv453Constants.DisId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DisId { get; set; }

        /// <summary>
        /// Gets or sets the journey identifier (FahrtID).
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
        /// Gets or sets the aimed arrival time (AnkunftszeitAZBPlan).
        /// </summary>
        [XmlElement(Vdv453Constants.ScheduledDisArrivalTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ScheduledDisArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the aimed departure time (AbfahrtszeitAZBPlan).
        /// </summary>
        [XmlElement(Vdv453Constants.ScheduledDisDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ScheduledDisDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the vehicle journey info (FahrtInfo).
        /// </summary>
        [XmlElement(Vdv453Constants.TripInfo, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TripInfo TripInfo { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DisSchedule)base.Clone();
            clone.TripId = Vdv453Message.CloneMember(this.TripId);
            clone.TripInfo = Vdv453Message.CloneMember(this.TripInfo);
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
            const string Format =
                "[DisSchedule LineId:'{0}', LineText:'{1}', DirectionId:'{2}', DirectionText:'{3}',"
                + " DisId:{4}, StopSeqCount:{5}, ScheduledDisArrivalTime:{6}, ScheduledDisDepartureTime:{7}, TimeStamp:{8}]";
            return string.Format(
                Format,
                this.LineId,
                this.LineText,
                this.DirectionId,
                this.DirectionText,
                this.DisId,
                this.StopSeqCount,
                this.ScheduledDisArrivalTime,
                this.ScheduledDisDepartureTime,
                this.TimeStamp);
        }
    }
}