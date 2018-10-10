// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedEntryContent.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedEntryContent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System.Xml.Serialization;

    /// <summary>
    /// The feed entry content.
    /// </summary>
    [XmlRoot("content", Namespace = "http://www.w3.org/2005/Atom")]
    public class FeedEntryContent
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        [XmlElement("properties", Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
        public FeedEntryContentProperties Properties { get; set; }
    }
}