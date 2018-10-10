// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSupplyRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the data supply request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Define DataSupplyRequest (DatenAbrufenAnfrage).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DataSupplyRequest, Namespace = "", IsNullable = false)]
    public class DataSupplyRequest : Vdv453Message
    {
        /// <summary>
        /// Initializes a new instance of the DataSupplyRequest class.
        /// </summary>
        public DataSupplyRequest()
        {
            this.AllDataSpecified = true;
        }

        /// <summary>
        /// Gets or sets the subscriber (Sender).
        /// </summary>
        [XmlAttribute(Vdv453Constants.Sender)]
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets recorded at time attribute [DateTime]
        /// </summary>
        [XmlAttribute(Vdv453Constants.TimeStamp)]
        public string TimeStampStr
        {
            get
            {
                return this.TimeStamp.ToString(Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.TimeStamp = DateTime.ParseExact(
                    value, Vdv453Constants.DatetimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets recorded at time attribute [DateTime]
        /// </summary>
        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the AllData flag is specified or not.
        /// </summary>
        [XmlIgnore]
        public bool AllDataSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the AllData flag is specified or not (DatensatzAlle).
        /// </summary>
        [XmlElement(Vdv453Constants.AllData, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool AllData { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "[DataSupplyRequest Sender: {0}, AllData: {1}, TimeStamp: {2}]",
                this.Sender,
                this.AllData,
                this.TimeStamp);
        }
    }
}