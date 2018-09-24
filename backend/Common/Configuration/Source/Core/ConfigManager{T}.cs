// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigManager{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Generic class used to handle configuration into/from file using serialization.
    /// </summary>
    /// <remarks>
    /// The FileName could be defined, but if you don't initialized it, the file name is the name of the T type + ".xml"
    /// </remarks>
    /// <typeparam name="T">
    /// Class of the top configuration container.
    /// </typeparam>
    public partial class ConfigManager<T> where T : class, new()
    {
        private string fileName;

        private T config;

        /// <summary>Initializes a new instance of the <see cref="ConfigManager{T}"/> class.</summary>
        /// <param name="fileName">The file name.</param>
        public ConfigManager(string fileName = "")
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                this.FileName = fileName;
                this.Configurator = new Configurator(this.FullConfigFileName);
            }
        }

        /// <summary>
        /// Gets or sets the object tasked to serialize/deserialize object in XML and vice versa.
        /// </summary>
        public Configurator Configurator { get; set; }

        /// <summary>
        /// Gets or sets the name of the configuration file.
        /// If you get the FileName and it has not been defined before, it returns T type + ".xml".
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.fileName))
                {
                    this.fileName = string.Format("{0}.xml", typeof(T).Name);
                }

                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// Gets The absolute name (comprehensive of also its extension) of the configuration file to be loaded.
        /// </summary>
        public string FullConfigFileName
        {
            get
            {
                var entryPath = Path.GetDirectoryName(ApplicationHelper.GetEntryAssemblyLocation());
                return Path.IsPathRooted(this.FileName) || entryPath == null
                           ? this.FileName
                           : Path.Combine(entryPath, this.FileName);
            }
        }

        /// <summary>
        /// Gets the instance of the object that contains could be serialized/deserialized into/from xml file.
        /// </summary>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If errors occurred while deserializing the file</exception>
        public T Config
        {
            get
            {
                if (!this.Initiated)
                {
                    this.LoadConfig();
                }

                return this.config;
            }
        }

        /// <summary>
        /// Gets a value indicating whether
        /// the config file was loaded with success or not.
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// Gets or sets the XmlSchema for validation.
        /// </summary>
        public XmlSchema XmlSchema { get; set; }

        /// <summary>
        /// Gets or sets the xml schema set for validation.
        /// </summary>
        /// <remarks>
        /// To be used if a schema includes another one.
        /// </remarks>
        public XmlSchemaSet XmlSchemaSet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether saving and loading a binary
        /// version of the config (automatically generated) is supported.
        /// </summary>
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Gets a value indicating whether the config was loaded from the cache file.
        /// </summary>
        /// <seealso cref="EnableCaching"/>
        public bool LoadedFromCache { get; private set; }

        /// <summary>
        /// Create a new instance of the {T} type and set theConfig property with it.
        /// </summary>
        public void CreateConfig()
        {
            this.config = new T();
            this.Initiated = true;
        }

        /// <summary>
        /// Gets the absolute path related to the configuration file path.
        /// </summary>
        /// <param name="file">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        public string GetAbsolutePathRelatedToConfig(string file)
        {
            if (Path.GetFullPath(file).Equals(file))
            {
                return file;
            }

            var baseDir = Path.GetDirectoryName(this.FullConfigFileName);
            if (baseDir != null)
            {
                var fullPath = Path.Combine(baseDir, file);
                return Path.GetFullPath(fullPath);
            }

            return file;
        }

        /// <summary>Serialize the Config object into an xml file.</summary>
        /// <param name="config">The config to write or null to use the existing Config.</param>
        /// <exception cref="NullReferenceException">Raised if the Config property is null.</exception>
        /// <exception cref="FileNotFoundException">Raised if the location of the executing assembly doesn't exist.</exception>
        public void SaveConfig(T config = null)
        {
            if (config != null)
            {
                this.config = config;
            }

            if (this.config == null)
            {
                throw new NullReferenceException("Config should be set before calling SaveConfig method");
            }

            if (this.Configurator == null)
            {
                this.Configurator = new Configurator(this.FullConfigFileName);
            }

            this.Configurator.Serialize(this.config);

            if (this.EnableCaching)
            {
                this.SaveCachedConfig();
            }
        }

        /// <summary>
        /// Converts this object into an XML string.
        /// </summary>
        /// <returns>
        /// The XML representation of this object, into a string.
        /// </returns>
        public string ToXmlString()
        {
            return this.Configurator.ToXmlString(this.config);
        }

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If something went wrong during deserialization.</exception>
        private void LoadConfig()
        {
            // If <> null, could be in testing mode
            if (this.Configurator == null)
            {
                if (this.XmlSchemaSet != null)
                {
                    this.Configurator = new Configurator(this.FullConfigFileName, this.XmlSchemaSet);
                }
                else
                {
                    this.Configurator = new Configurator(this.FullConfigFileName, this.XmlSchema);
                }
            }

            this.LoadedFromCache = this.EnableCaching && this.LoadCachedConfig();
            if (this.LoadedFromCache)
            {
                this.Initiated = true;
                return;
            }

            this.config = this.Configurator.Deserialize<T>();

            if (this.config == null)
            {
                throw new XmlException("Could not deserialize from " + this.FullConfigFileName);
            }

            if (this.config is IVerifiable)
            {
                (this.config as IVerifiable).Verify();
            }

            if (this.EnableCaching)
            {
                this.SaveCachedConfig();
            }

            this.Initiated = true;
        }
    }
}
