// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the vehicle journey info type.
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
    /// Define the vehicle journey info type (FahrtInfoType).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.TripInfo, Namespace = "", IsNullable = false)]
    public class TripInfo : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the TripInfo class.
        /// </summary>
        public TripInfo()
        {
            this.DepartureTimeStartStop = DateTime.MinValue;
            this.DepartureTimeStartStopSpecified = false;
            this.ArrivalTimeDestinationStop = DateTime.MinValue;
            this.ArrivalTimeDestinationStopSpecified = false;
        }

        /// <summary>
        /// Gets or sets the vehicle reference (FahrzeugID).
        /// </summary>
        [XmlElement(Vdv453Constants.VehicleId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the external line reference (LinienNr).
        /// </summary>
        [XmlElement(Vdv453Constants.LineNumber, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the block reference (UmlaufNr).
        /// </summary>
        [XmlElement(Vdv453Constants.BlockNumber, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint BlockNumber { get; set; }

        /// <summary>
        /// Gets or sets the course of journey reference (KursNr).
        /// </summary>
        [XmlElement(Vdv453Constants.RunNumber, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint RunNumber { get; set; }

        /// <summary>
        /// Gets or sets the origin name (StartHstLang).
        /// </summary>
        [XmlElement(Vdv453Constants.DepartureStopLong, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DepartureStopLong { get; set; }

        /// <summary>
        /// Gets or sets the origin short name (StartHst).
        /// </summary>
        [XmlElement(Vdv453Constants.DepartureStop, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DepartureStop { get; set; }

        /// <summary>
        /// Gets or sets the destination name (ZielHstLang).
        /// </summary>
        [XmlElement(Vdv453Constants.DestinationStopLong, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DestinationStopLong { get; set; }

        /// <summary>
        /// Gets or sets the destination short name (zielHst).
        /// </summary>
        [XmlElement(Vdv453Constants.DestinationStop, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DestinationStop { get; set; }

        /// <summary>
        /// Gets or sets the origin aimed departure time (AbfahrtszeitStartHst).
        /// </summary>
        [XmlElement(Vdv453Constants.DepartureTimeStartStop, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime DepartureTimeStartStop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the origin aimed departure time is specified (AbfahrtszeitStartHstSpecified).
        /// </summary>
        [XmlIgnore]
        public bool DepartureTimeStartStopSpecified { get; set; }

        /// <summary>
        /// Gets or sets the destination aimed arrival time (AnkunftszeitZielHst).
        /// </summary>
        [XmlElement(Vdv453Constants.ArrivalTimeDestinationStop, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ArrivalTimeDestinationStop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the destination aimed arrival time is specified (AnkunftszeitZielHstSpecified).
        /// </summary>
        [XmlIgnore]
        public bool ArrivalTimeDestinationStopSpecified { get; set; }

        /// <summary>
        /// Gets or sets the product category reference (ProduktID).
        /// </summary>
        [XmlElement(Vdv453Constants.ProductId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the operator reference (Betreiber).
        /// </summary>
        [XmlElement(Vdv453Constants.Operator, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the service feature reference (ServiceMerkmal).
        /// </summary>
        [XmlElement(Vdv453Constants.ServiceAttribute, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] ServiceAttribute { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var clone = (TripInfo)this.MemberwiseClone();
            clone.ServiceAttribute = (string[])this.ServiceAttribute.Clone();
            return clone;
        }
    }
}