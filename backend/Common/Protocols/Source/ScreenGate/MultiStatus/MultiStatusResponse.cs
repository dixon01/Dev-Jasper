// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiStatusResponse.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiStatusResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.MultiStatus
{
    using System.Xml.Serialization;

    /// <summary>
    /// One response element in a multi-status.
    /// </summary>
    [XmlType(Namespace = "DAV:")]
    public class MultiStatusResponse
    {
        /// <summary>
        /// Gets or sets the Hypertext Reference to the file to be downloaded.
        /// This is usually relative to <see cref="MultiStatus.Base"/>.
        /// </summary>
        [XmlElement("href")]
        public string HypertextReference { get; set; }

        /// <summary>
        /// Gets or sets the properties of the response.
        /// </summary>
        [XmlElement("propstat")]
        public PropStat PropStat { get; set; }
    }
}