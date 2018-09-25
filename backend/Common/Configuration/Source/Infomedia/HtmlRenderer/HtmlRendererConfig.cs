// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlRendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.HtmlRenderer
{
    using System;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the Html Renderer.
    /// </summary>
    [XmlRoot("HtmlRenderer")]
    [Serializable]
    public class HtmlRendererConfig
    {
        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                   var input =
                       typeof(HtmlRendererConfig).Assembly.GetManifestResourceStream(
                           typeof(HtmlRendererConfig), "HtmlRenderer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find HtmlRenderer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the port used to send the updates.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the screen width.
        /// </summary>
        public int ScreenWidth { get; set; }

        /// <summary>
        /// Gets or sets the screen height.
        /// </summary>
        public int ScreenHeight { get; set; }
    }
}
