// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoWebMediaElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoWebMediaElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The video web.media element.
    /// </summary>
    [Serializable]
    public class VideoWebmediaElement : WebmediaElementBase
    {
        /// <summary>
        /// Gets or sets the video URI.
        /// </summary>
        [XmlAttribute("VideoUri")]
        public string VideoUri { get; set; }

        /// <summary>
        /// Gets or sets the scaling of the video.
        /// </summary>
        [XmlAttribute("Scaling")]
        public ElementScaling Scaling { get; set; }
    }
}