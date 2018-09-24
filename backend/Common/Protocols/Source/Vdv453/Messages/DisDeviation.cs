// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisDeviation.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the dis deviation message.
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
    /// Define DisDeviation Type (AZBFahrplanlage).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression " + "is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DisDeviation, Namespace = "", IsNullable = false)]
    public class DisDeviation : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DisDeviation class.
        /// </summary>
        public DisDeviation()
        {
            this.AtDisPoint = false;
            this.QueueIndicator = false;
            this.TripStatus = TripStatus.Soll;
            this.ScheduledDisArrivalTime = DateTime.MinValue;
            this.ExpectedDisArrivalTime = DateTime.MinValue;
            this.ScheduledDisDepartureTime = DateTime.MinValue;
            this.ExpectedDisDepartureTime = DateTime.MinValue;
            this.AimedDisDepartureTime = DateTime.MinValue;
            this.TimeStamp = DateTime.MinValue;
            this.ValidUntilTimeStamp = DateTime.MinValue;
            this.TripInfo = new TripInfo();
        }

        /// <summary>
        /// Gets or sets the station identifier (AZBID).
        /// </summary>
        [XmlElement(Vdv453Constants.DisId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DisId { get; set; }

        /// <summary>
        /// Gets or sets the trip identifier (FahrtID).
        /// </summary>
        [XmlElement(Vdv453Constants.TripId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TripId TripId { get; set; }

        /// <summary>
        /// Gets or sets the stop sequence counter (HstSeqZaehler).
        /// </summary>
        [XmlElement(Vdv453Constants.StopSeqCount, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StopSeqCount { get; set; }

        /// <summary>
        /// Gets or sets the trainset (Traktion).
        /// </summary>
        [XmlElement(Vdv453Constants.Trainset, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Trainset Trainset { get; set; }

        /// <summary>
        /// Gets or sets the operational vehicle reference (BetrieblicheFahrzeugnummer).
        /// </summary>
        [XmlElement(Vdv453Constants.VehicleNumber, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] VehicleNumber { get; set; }

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
        /// Gets or sets the destination short name (ZielHst).
        /// </summary>
        [XmlElement(Vdv453Constants.DestinationStop, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DestinationStop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle at stop flag is set or not (AufAZB).
        /// </summary>
        [XmlElement(Vdv453Constants.AtDisPoint, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(false)]
        public bool AtDisPoint { get; set; }

        /// <summary>
        /// Gets or sets the via name #1 (ViaHst1Lang).
        /// </summary>
        [XmlElement(Vdv453Constants.ViaStop1, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ViaStop1 { get; set; }

        /// <summary>
        /// Gets or sets the via name #2 (ViaHst2Lang).
        /// </summary>
        [XmlElement(Vdv453Constants.ViaStop2, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ViaStop2 { get; set; }

        /// <summary>
        /// Gets or sets the via name #3 (ViaHst3Lang).
        /// </summary>
        [XmlElement(Vdv453Constants.ViaStop3, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ViaStop3 { get; set; }

        /// <summary>
        /// Gets or sets the monitored value (FahrtStatus).
        /// </summary>
        [XmlElement(Vdv453Constants.TripStatus, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TripStatus TripStatus { get; set; }

        /// <summary>
        /// Gets or sets the aimed arrival time (AnkunftszeitAZBPlan).
        /// </summary>
        [XmlElement(Vdv453Constants.ScheduledDisArrivalTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ScheduledDisArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the expected arrival time (AnkunftszeitAZBPrognose).
        /// </summary>
        [XmlElement(Vdv453Constants.ExpectedDisArrivalTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ExpectedDisArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the aimed departure time (AbfahrtszeitAZBPlan).
        /// </summary>
        [XmlElement(Vdv453Constants.ScheduledDisDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ScheduledDisDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the expected departure time (AbfahrtszeitAZBPrognose).
        /// </summary>
        [XmlElement(Vdv453Constants.ExpectedDisDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ExpectedDisDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the disposition departure time (AbfahrtszeitAZBDisposition).
        /// </summary>
        [XmlElement(Vdv453Constants.AimedDisDepartureTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime AimedDisDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the stop visit notice (Fahrtspezialtext).
        /// </summary>
        [XmlElement(Vdv453Constants.TripSpecialText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TripSpecialText { get; set; }

        /// <summary>
        /// Gets or sets the speech output (Sprachausgabe).
        /// </summary>
        [XmlElement(Vdv453Constants.SpeechOutput, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] SpeechOutput { get; set; }

        /// <summary>
        /// Gets or sets the stop point reference (HaltID).
        /// </summary>
        [XmlElement(Vdv453Constants.StopId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StopId { get; set; }

        /// <summary>
        /// Gets or sets the change notice (Haltepositionstext).
        /// </summary>
        [XmlElement(Vdv453Constants.StopPositionText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StopPositionText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the congestion flag is set or not (Stauindikator).
        /// </summary>
        [XmlElement(Vdv453Constants.QueueIndicator, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(false)]
        public bool QueueIndicator { get; set; }

        /// <summary>
        /// Gets or sets the trip info (FahrtInfo).
        /// </summary>
        [XmlElement(Vdv453Constants.TripInfo, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TripInfo TripInfo { get; set; }

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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (DisDeviation)base.Clone();
            clone.TripId = Vdv453Message.CloneMember(this.TripId);
            clone.Trainset = Vdv453Message.CloneMember(this.Trainset);
            clone.VehicleNumber = Vdv453Message.CloneMember(this.VehicleNumber);
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
            const string Format = "[DisDeviation LineId:'{0}', LineText:'{1}', DirectionId:'{2}', DirectionText:'{3}',"
                                  + " DisId:'{4}', TripId:'{5}', AtDisPoint:{6}, DepartureNoticeId:'{7}',"
                                  + " DestinationStop:'{8}', QueueIndicator:{9}, ScheduledDisArrivalTime:{10},"
                                  + " ScheduledDisDepartureTime:{11}, ExpectedDisArrivalTime:{12}"
                                  + ", ExpectedDisDepartureTime:{13}, TripStatus:{14}, StopSeqCount:{15}]";
            return string.Format(
                Format,
                this.LineId,
                this.LineText,
                this.DirectionId,
                this.DirectionText,
                this.DisId,
                this.TripId,
                this.AtDisPoint,
                this.DepartureNoticeId,
                this.DestinationStop,
                this.QueueIndicator,
                this.ScheduledDisArrivalTime,
                this.ScheduledDisDepartureTime,
                this.ExpectedDisArrivalTime,
                this.ExpectedDisDepartureTime,
                this.TripStatus,
                this.StopSeqCount);
        }
    }
}