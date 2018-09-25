// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPaperDynamicContentPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Dynamic
{
    using System.Xml.Serialization;

    /// <summary>
    /// The E-Paper dynamic content part describing the configuration of a Main unit and its display unit(s).
    /// </summary>
    public class EPaperDynamicContentPart : DynamicContentPartBase
    {
        /// <summary>
        /// Gets or sets the url of the display unit content.
        /// This is only used if <see cref="IsPersistentFile"/> is set to <c>false</c>.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the static file source hash.
        /// This is only used if <see cref="IsPersistentFile"/> is set to <c>true</c>.
        /// </summary>
        public string StaticFileSourceHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is persistent file.
        /// </summary>
        [XmlAttribute("IsPersistentFile")]
        public bool IsPersistentFile { get; set; }

        /// <summary>
        /// Gets or sets the main unit id. It is the database unit identifier.
        /// </summary>
        [XmlAttribute("MainUnitId")]
        public int MainUnitId { get; set; }

        /// <summary>
        /// Gets or sets the display unit index.
        /// </summary>
        [XmlAttribute("DisplayUnitIndex")]
        public int DisplayUnitIndex { get; set; }
    }
}