// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feed.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Feed type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The feed.
    /// </summary>
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Feed
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
        /// Gets or sets the entries.
        /// </summary>
        [XmlElement("entry", Type = typeof(FeedEntry))]
        public List<FeedEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [XmlElement("link")]
        public FeedLink Link { get; set; }
    }
}