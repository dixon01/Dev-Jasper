// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedLink.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedLink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ConsoleApplication
{
    using System.Xml.Serialization;

    /// <summary>
    /// The feed link.
    /// </summary>
    [XmlRoot("link", Namespace = "http://www.w3.org/2005/Atom")]
    public class FeedLink
    {
        /// <summary>
        /// Gets or sets the relative value.
        /// </summary>
        [XmlAttribute("rel")]
        public string Rel { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink reference.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }
    }
}