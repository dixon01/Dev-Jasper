// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageWebMediaElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageWebMediaElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The image web.media element.
    /// </summary>
    [Serializable]
    public class ImageWebmediaElement : WebmediaElementBase
    {
        /// <summary>
        /// Gets or sets the filename of the image.
        /// </summary>
        [XmlAttribute("Filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the scaling of the image.
        /// </summary>
        [XmlAttribute("Scaling")]
        public ElementScaling Scaling { get; set; }
    }
}