// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropStat.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropStat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.MultiStatus
{
    using System.Xml.Serialization;

    /// <summary>
    /// The properties of a response.
    /// </summary>
    public class PropStat
    {
        /// <summary>
        /// Gets or sets the single property.
        /// </summary>
        [XmlElement("prop")]
        public Prop Prop { get; set; }
    }
}