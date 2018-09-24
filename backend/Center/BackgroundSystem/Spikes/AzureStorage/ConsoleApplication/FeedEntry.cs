// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System.Xml.Serialization;

    /// <summary>
    /// The feed entry.
    /// </summary>
    [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")]
    public class FeedEntry
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [XmlElement("link")]
        public FeedLink Link { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        [XmlElement("content")]
        public FeedEntryContent Content { get; set; }
    }
}