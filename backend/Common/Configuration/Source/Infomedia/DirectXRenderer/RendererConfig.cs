// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RendererConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// The renderer config.
    /// </summary>
    [XmlRoot("DirectXRenderer")]
    [Serializable]
    public class RendererConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererConfig"/> class.
        /// </summary>
        public RendererConfig()
        {
            this.WindowMode = WindowMode.FullScreenWindowed;
            this.FallbackTimeout = TimeSpan.FromSeconds(60);

            this.Screens = new List<ScreenConfig>(2);
            this.Device = new DeviceConfig();
            this.Text = new TextConfig();
            this.Image = new ImageConfig();
            this.Video = new VideoConfig();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(RendererConfig).Assembly.GetManifestResourceStream(
                            typeof(RendererConfig), "DirectXRenderer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find DirectXRenderer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the start-up window mode.
        /// </summary>
        public WindowMode WindowMode { get; set; }

        /// <summary>
        /// Gets or sets the fallback timeout after which the <see cref="ScreenConfig.FallbackImage"/>
        /// or the splash screen is shown if no data is received from the Composer.
        /// A value of zero or less disables the fallback timeout.
        /// </summary>
        [XmlIgnore]
        public TimeSpan FallbackTimeout { get; set; }

        /// <summary>
        /// Gets or sets the fallback timeout as an XML serializable string.
        /// </summary>
        [XmlElement("FallbackTimeout")]
        public string FallbackTimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.FallbackTimeout);
            }

            set
            {
                this.FallbackTimeout = string.IsNullOrEmpty(value) || value=="0" ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the screens to use.
        /// If this list is empty, all currently available adapters will be taken.
        /// </summary>
        [XmlArrayItem("Screen")]
        public List<ScreenConfig> Screens { get; set; }

        /// <summary>
        /// Gets or sets the DirectX device configuration.
        /// </summary>
        public DeviceConfig Device { get; set; }

        /// <summary>
        /// Gets or sets the text configuration.
        /// </summary>
        public TextConfig Text { get; set; }

        /// <summary>
        /// Gets or sets the image configuration.
        /// </summary>
        public ImageConfig Image { get; set; }

        /// <summary>
        /// Gets or sets the video configuration.
        /// </summary>
        public VideoConfig Video { get; set; }

        /// <summary>
        /// Enable/Disable the presentation logging for presentation proof of play logging
        /// </summary>
        [Obsolete("Not Used")]
        public bool EnablePresentationLogging { get; set; } = true;

        /// <summary>
        /// Loads the renderer configuration from the given XML file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="RendererConfig"/>.
        /// </returns>
        public static RendererConfig LoadFrom(string filename)
        {
            var configManager = new ConfigManager<RendererConfig>();
            configManager.FileName = filename;
            configManager.EnableCaching = true;
            configManager.XmlSchema = Schema;

            // update paths
            var config = configManager.Config;
            foreach (var screen in config.Screens)
            {
                if (!string.IsNullOrEmpty(screen.FallbackImage))
                {
                    screen.FallbackImage = configManager.GetAbsolutePathRelatedToConfig(screen.FallbackImage);
                }
            }

            for (int i = 0; i < config.Image.PreloadDirectories.Count; i++)
            {
                var preloadDirectory = config.Image.PreloadDirectories[i];
                config.Image.PreloadDirectories[i] = configManager.GetAbsolutePathRelatedToConfig(preloadDirectory);
            }

            return config;
        }
    }
}
