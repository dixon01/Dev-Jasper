// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// The image config.
    /// </summary>
    [Serializable]
    public class ImageConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageConfig"/> class.
        /// </summary>
        public ImageConfig()
        {
            this.BitmapCacheTimeout = TimeSpan.FromHours(1);
            this.MaxBitmapCacheBytes = 250 * 1024 * 1024; // 50 MB
            this.MaxCacheBytesPerBitmap = 10 * 1024 * 1024; // 10 MB (8 MB = Full HD)

            this.PreloadDirectories = new List<string>();
        }

        /// <summary>
        /// Gets or sets the bitmap cache timeout in seconds.
        /// Default value is 1 hour.
        /// </summary>
        [XmlIgnore]
        public TimeSpan BitmapCacheTimeout { get; set; }

        /// <summary>
        /// Gets or sets the bitmap cache timeout as an XML serializable string.
        /// </summary>
        [XmlElement("BitmapCacheTimeout")]
        public string BitmapCacheTimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.BitmapCacheTimeout);
            }

            set
            {
                int timeoutSeconds;
                if (ParserUtil.TryParse(value, out timeoutSeconds))
                {
                    this.BitmapCacheTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                }
                else
                {
                    this.BitmapCacheTimeout = XmlConvert.ToTimeSpan(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the approximate maximum number of bytes to use for the image cache.
        /// The cache can sometimes go above this limit depending on several factors.
        /// Default value is 50 MB.
        /// </summary>
        public long MaxBitmapCacheBytes { get; set; }

        /// <summary>
        /// Gets or sets the maximum size in bytes of an image to be cached.
        /// This can be used to limit the size of a single image to be cached
        /// and for example not cache background images.
        /// Default value is 500x500 pixels.
        /// </summary>
        public int MaxCacheBytesPerBitmap { get; set; }

        /// <summary>
        /// Gets or sets the directories from which images should be pre-loaded.
        /// All known image types from the given directories are loaded when DirectX Renderer starts.
        /// The pre-loading is limited by the <see cref="MaxBitmapCacheBytes"/> and <see cref="MaxCacheBytesPerBitmap"/>
        /// values and pre-loaded images are discarded after <see cref="BitmapCacheTimeout"/> when not used.
        /// </summary>
        [XmlElement("PreloadDirectory")]
        public List<string> PreloadDirectories { get; set; }
    }
}