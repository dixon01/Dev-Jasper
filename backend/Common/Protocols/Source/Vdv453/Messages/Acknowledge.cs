// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Acknowledge.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the VDV453 response status type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Response status type (BestaetigungType)
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.Acknowledge, Namespace = "", IsNullable = false)]
    public class Acknowledge : Vdv453Message
    {
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
        /// Gets or sets response result type
        /// </summary>
        [XmlAttribute(Vdv453Constants.Result)]
        public Result Result { get; set; }

        /// <summary>
        /// Gets or sets response error number
        /// </summary>
        [XmlAttribute(Vdv453Constants.ErrorNumber)]
        public int ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets response error text
        /// </summary>
        [XmlElement(Vdv453Constants.ErrorText, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets or sets data valid until time string
        /// </summary>
        [XmlElement(Vdv453Constants.DataValidUntil, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DataValidUntilStr { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether data valid until ist specified
        /// </summary>
        [XmlIgnore]
        public bool DataValidUntilSpecified { get; set; }

        /// <summary>
        /// Gets or sets shortest possible cycle
        /// </summary>
        [XmlElement(Vdv453Constants.ShortestPossibleCycleTime, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint ShortestPossibleCycleTime { get; set; }
    }
}