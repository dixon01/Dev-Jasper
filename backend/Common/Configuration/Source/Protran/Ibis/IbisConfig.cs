// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Container of all the objects that compose
    /// the ibis configuration file.
    /// </summary>
    [XmlRoot("Ibis")]
    [Serializable]
    public class IbisConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisConfig"/> class.
        /// Container of all the settings required by Protran to translate the IBIS protocol.
        /// </summary>
        public IbisConfig()
        {
            this.Behaviour = new Behaviour();
            this.Sources = new IbisSourcesConfig();
            this.Telegrams = new TelegramConfigList();
            this.Transformations = new List<Chain>();
        }

        #region PROPERTIES

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(IbisConfig).Assembly.GetManifestResourceStream(
                            typeof(IbisConfig), "ibis.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find ibis.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the XML element called <code>Behaviour</code>.
        /// </summary>
        public Behaviour Behaviour { get; set; }

        /// <summary>
        /// Gets or sets the configured sources.
        /// </summary>
        public IbisSourcesConfig Sources { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Recording.
        /// </summary>
        public RecordingConfig Recording { get; set; }

        /// <summary>
        /// Gets or sets the XML element called TimeSync.
        /// </summary>
        public TimeSyncConfig TimeSync { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Telegrams.
        /// </summary>
        public TelegramConfigList Telegrams { get; set; }

        /// <summary>
        /// Gets or sets Transformations.
        /// </summary>
        public List<Chain> Transformations { get; set; }
        #endregion PROPERTIES
    }
}
