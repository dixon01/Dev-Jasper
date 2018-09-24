// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that defines the StatusRequest message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the StatusRequest message (StatusAnfrage).
    /// For more information see VDVSchriften 453, 03/08, Version 2.3
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here."), Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot(Vdv453Constants.StatusRequest, Namespace = "", IsNullable = false)]
    public class StatusRequest : Vdv453Message
    {
        /// <summary>
        /// Gets or sets the sender attribute
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
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[StatusRequest Sender: {0}, TimeStamp: {1}", this.Sender, this.TimeStamp);
        }
    }
}