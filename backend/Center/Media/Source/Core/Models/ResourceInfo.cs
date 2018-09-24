// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// Defines resource information.
    /// </summary>
    [XmlRoot("ResourceInfo")]
    public class ResourceInfo : DataModelBase
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ResourceType Type { get; set; }

        /// <summary>
        /// Gets or sets the hash of the thumbnail.
        /// </summary>
        /// <value>
        /// The hash of the thumbnail.
        /// </value>
        public string ThumbnailHash { get; set; }

        /// <summary>
        /// Gets or sets the references count.
        /// </summary>
        /// <value>
        /// The references count.
        /// </value>
        public int ReferencesCount { get; set; }

        /// <summary>
        /// Gets or sets the dimension of the resource. (Width x Height).
        /// </summary>
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the duration of a video resource.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the face name.
        /// </summary>
        public string Facename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is led font.
        /// </summary>
        public bool IsLedFont { get; set; }

        /// <summary>
        /// Gets or sets the led font type.
        /// </summary>
        public LedFontType LedFontType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is led image.
        /// </summary>
        public bool IsLedImage { get; set; }

        /// <summary>
        /// Gets or sets the length of a resource.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force export.
        /// </summary>
        public bool ForceExport { get; set; }
    }
}