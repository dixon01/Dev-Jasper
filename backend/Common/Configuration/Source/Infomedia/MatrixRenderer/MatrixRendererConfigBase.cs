// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatrixRendererConfigBase.cs" company="Gorba AG">
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
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for the matrix renderer configurations.
    /// </summary>
    [Serializable]
    public abstract class MatrixRendererConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixRendererConfigBase"/> class.
        /// </summary>
        protected MatrixRendererConfigBase()
        {
            this.Channels = new List<ChannelConfig>();
            this.Text = new TextConfig();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public abstract XmlSchema Schema { get;  }

        /// <summary>
        /// Gets or sets the configured channels.
        /// </summary>
        [XmlElement("Channel")]
        public List<ChannelConfig> Channels { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration.
        /// </summary>
        public TextConfig Text { get; set; }
    }
}