// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    /// <summary>
    /// Defines the POCO for the media configuration file.
    /// </summary>
    public class MediaConfiguration
    {
        /// <summary>
        /// Gets or sets the resource settings.
        /// </summary>
        [XmlElement("ResourceSettings")]
        public ResourceSettings ResourceSettings { get; set; }

        /// <summary>
        /// Gets or sets the master layouts.
        /// </summary>
        [XmlElement("PhysicalScreenSettings")]
        public PhysicalScreenSettings PhysicalScreenSettings { get; set; }

        /// <summary>
        /// Gets or sets the direct x renderer config.
        /// </summary>
        [XmlElement("DirectXRendererSettings")]
        public RendererConfig DirectXRendererConfig { get; set; }
    }
}
