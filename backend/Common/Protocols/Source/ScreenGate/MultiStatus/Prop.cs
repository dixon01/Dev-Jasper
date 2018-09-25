// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Prop.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Prop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.MultiStatus
{
    using System.Xml.Serialization;

    /// <summary>
    /// A property of a multi-status response.
    /// </summary>
    public class Prop
    {
        /// <summary>
        /// Gets or sets the get ETag used to distinguish different versions of the same resource.
        /// </summary>
        [XmlElement("getetag")]
        public string GetETag { get; set; }
    }
}