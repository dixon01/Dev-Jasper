// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatrixRendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MatrixRendererConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.MatrixRenderer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The config for the Matrix renderer.
    /// </summary>
    [Serializable]
    [XmlRoot("MatrixRenderer")]
    public class MatrixRendererConfig : MatrixRendererConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixRendererConfig"/> class.
        /// </summary>
        public MatrixRendererConfig()
        {
            this.Channels = new List<ChannelConfig>();
            this.Text = new TextConfig();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public override XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(MatrixRendererConfig).Assembly.GetManifestResourceStream(
                            typeof(MatrixRendererConfig), "MatrixRenderer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find matrixRenderer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }
    }
}
