// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripId.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the drive identifier.
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
    /// Define DriveID type (FahrtID type)
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    public class TripId : ICloneable
    {
        /// <summary>
        /// Gets or sets the dated vehicle journey (FahrtBezeichner).
        /// </summary>
        [XmlElement(Vdv453Constants.TripName, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TripName { get; set; }

        /// <summary>
        /// Gets or sets the dated frame (Betriebstag).
        /// </summary>
        [XmlElement(Vdv453Constants.DayType, Form = System.Xml.Schema.XmlSchemaForm.Unqualified, 
            DataType = "date")]
        public DateTime DayType { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}