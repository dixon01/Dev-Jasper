// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RepositoryConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Repository
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The repository config stored in repository.xml.
    /// </summary>
    [XmlRoot("Repository")]
    public class RepositoryConfig
    {
        /// <summary>
        /// The default file name of the repository file (repository.xml).
        /// </summary>
        public static readonly string RepositoryXmlFileName = "repository.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryConfig"/> class.
        /// </summary>
        public RepositoryConfig()
        {
            this.Versions = new List<RepositoryVersionConfig>();
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
                        typeof(RepositoryConfig).Assembly.GetManifestResourceStream(
                            typeof(RepositoryConfig), "repository.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find repository.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the different configurations valid for different versions
        /// of update. This can be used to allow newer update software to install files
        /// from different folders in the repository.
        /// Versions have to be configured new-to-old, meaning the first version config
        /// that matches the current version will be taken.
        /// </summary>
        [XmlElement("Config")]
        public List<RepositoryVersionConfig> Versions { get; set; }

        /// <summary>
        /// Gets the currently valid <see cref="RepositoryVersionConfig"/> from this configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="RepositoryVersionConfig"/> for the current version.
        /// </returns>
        /// <exception cref="UpdateException">
        /// if there is no valid configuration for the current version.
        /// </exception>
        public RepositoryVersionConfig GetCurrentConfig()
        {
            var currentVersion = this.GetType().Assembly.GetName().Version;

            foreach (var version in this.Versions)
            {
                if (version.ValidFrom == null || version.ValidFrom <= currentVersion)
                {
                    return version;
                }
            }

            throw new UpdateException(
                string.Format("Couldn't find a valid repository configuration for version {0}", currentVersion));
        }
    }
}
