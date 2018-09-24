// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Composer
{
    using System;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The root element of the composer configuration.
    /// </summary>
    [XmlRoot("Composer")]
    [Serializable]
    public class ComposerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerConfig"/> class.
        /// </summary>
        public ComposerConfig()
        {
            this.XimpleInactivity = new XimpleInactivityConfig();
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
                        typeof(ComposerConfig).Assembly.GetManifestResourceStream(
                            typeof(ComposerConfig), "Composer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find Composer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the ximple inactivity config.
        /// </summary>
        public XimpleInactivityConfig XimpleInactivity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to log presentation events.
        /// </summary>
        public bool EnablePresentationLogging { get; set; } = false;
    }
}
