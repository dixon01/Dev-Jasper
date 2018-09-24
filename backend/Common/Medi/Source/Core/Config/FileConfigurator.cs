// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   An  implementation that reads the information from a local file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// An <see cref="IConfigurator"/> implementation that reads the information from a local file.
    /// </summary>
    public class FileConfigurator : IConfigurator
    {
        private readonly string configFileName;
        private readonly string unitName;
        private readonly string applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurator"/> class
        /// with the given file name. Unit and application name are generated automatically.
        /// </summary>
        /// <param name="configFileName">
        /// The config file name.
        /// </param>
        public FileConfigurator(string configFileName)
            : this(configFileName, ApplicationHelper.MachineName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurator"/> class
        /// with the given file name and unit name. The application name is generated automatically.
        /// </summary>
        /// <param name="configFileName">
        /// The config file name.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        public FileConfigurator(string configFileName, string unitName)
            : this(configFileName, unitName, ApplicationHelper.GetEntryAssemblyName())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurator"/> class.
        /// </summary>
        /// <param name="configFileName">
        /// The config file name.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        public FileConfigurator(string configFileName, string unitName, string applicationName)
        {
            this.configFileName = configFileName;
            this.unitName = unitName;
            this.applicationName = applicationName;
        }

        /// <summary>
        /// Creates the local Medi address using the unit and application name provided
        /// in the constructor.
        /// </summary>
        /// <returns>
        /// the local Medi address.
        /// </returns>
        public MediAddress CreateLocalAddress()
        {
            return new MediAddress { Unit = this.unitName, Application = this.applicationName };
        }

        /// <summary>
        /// Creates a config object by de-serializing it from the file provided in the constructor.
        /// </summary>
        /// <returns>
        /// the config object.
        /// </returns>
        public MediConfig CreateConfig()
        {
            Debug.Assert(!string.IsNullOrEmpty(this.configFileName), "Missing Config File Name");
            if (string.IsNullOrEmpty(this.configFileName) == false)
            {
                var serializer = new XmlSerializer(typeof(MediConfig));
                using (TextReader reader = File.OpenText(this.configFileName))
                {
                    return serializer.Deserialize(reader) as MediConfig;
                }
            }
            return null;
        }
    }
}
