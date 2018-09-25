// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trainset.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the train part reference.
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
    /// Define train part reference (Traktion).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("xsd", "2.0.50727.42")]
    public class Trainset : ICloneable
    {
        /// <summary>
        /// Gets or sets the train part reference (TraktionsID).
        /// </summary>
        [XmlElement(Vdv453Constants.TrainsetId, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TrainsetId { get; set; }

        /// <summary>
        /// Gets or sets the number of blocks (AnzahlFahrten).
        /// </summary>
        [XmlElement(Vdv453Constants.NumOfTrips, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint NumOfTrips { get; set; }

        /// <summary>
        /// Gets or sets the position of train block (Position).
        /// </summary>
        [XmlElement(Vdv453Constants.Position, Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [DefaultValue(0u)]
        public uint Position { get; set; }

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