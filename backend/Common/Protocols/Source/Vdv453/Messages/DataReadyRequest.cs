// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataReadyRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataReadyRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the VDV453 data ready request message (DatenBereitAnfrage).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    [XmlRoot(Vdv453Constants.DataReadyRequest, Namespace = "", IsNullable = false)]
    public class DataReadyRequest : Vdv453Message
    {
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
    }
}
