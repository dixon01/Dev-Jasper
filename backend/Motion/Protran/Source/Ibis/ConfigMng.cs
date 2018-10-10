// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigMng.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// Class manager about the configuration
    /// file regarding the IBIS options.
    /// </summary>
    public class ConfigMng
    {
        #region VARIABLES
        /// <summary>
        /// The object tasked to serialize/deserialize object in XML and vice versa.
        /// </summary>
        private readonly ConfigManager<IbisConfig> configManager;

        /// <summary>
        /// The absolute name (comprehensive of also its extension) of
        /// the configuration file to load.
        /// </summary>
        private string fileAbsName;
        #endregion VARIABLES

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigMng"/> class.
        /// </summary>
        public ConfigMng()
        {
            this.configManager = new ConfigManager<IbisConfig> { EnableCaching = true };
        }

        #region PROPERTIES

        /// <summary>
        /// Gets the instance of the unique object that contains
        /// all the IBIS configurations.
        /// </summary>
        public IbisConfig IbisConfig
        {
            get
            {
                return this.configManager.Config;
            }
        }

        /// <summary>
        /// Gets a value indicating whether
        /// the config file was loaded with success or not.
        /// </summary>
        [XmlIgnore]
        public bool InitOk { get; private set; }
        #endregion PROPERTIES

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        public void Load(string fileName)
        {
            this.fileAbsName = fileName;
            this.configManager.FileName = this.fileAbsName;

            this.configManager.XmlSchema = IbisConfig.Schema;

            if (this.configManager.Config == null)
            {
                throw new XmlException("Could not deserialize from " + this.fileAbsName);
            }

            this.InitOk = true;
        }

        /// <summary>
        /// Gets the absolute path related to the IBIS configuration file path.
        /// </summary>
        /// <param name="file">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        public string GetAbsolutePathRelatedToConfig(string file)
        {
            return this.configManager.GetAbsolutePathRelatedToConfig(file);
        }
    }
}