// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigMng.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Motion.Protran.AbuDhabi.Config;

    /// <summary>
    /// Class manager about the configuration
    /// file regarding the ISI, ISM and CU5 options.
    /// </summary>
    public class ConfigMng
    {
        /// <summary>
        /// The absolute name (comprehensive of also its extension) of
        /// the configuration file to load.
        /// </summary>
        private string fileAbsName;

        /// <summary>
        /// Gets the instance of the unique object that contains
        /// all the configurations for the Abu Dhabi project.
        /// </summary>
        [XmlIgnore]
        public AbuDhabiConfig AbuDhabiConfig { get; private set; }

        /// <summary>
        /// Gets a value indicating whether
        /// the config file was loaded with success or not.
        /// </summary>
        [XmlIgnore]
        public bool InitOk { get; private set; }

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
            var configMgr = new ConfigManager<AbuDhabiConfig> { FileName = this.fileAbsName, EnableCaching = true };
            this.AbuDhabiConfig = configMgr.Config;
            if (this.AbuDhabiConfig == null)
            {
                throw new XmlException("Could not deserialize from " + this.fileAbsName);
            }

            this.InitOk = true;
        }

        /// <summary>
        /// Loads a string that represents the config file's content.
        /// </summary>
        /// <param name="fileContent">The file content.</param>
        public void LoadContent(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                // invalid file name.
                throw new ArgumentException("Specify a content.");
            }

            string tmpFilePath = "./AbuDhabi.xml";
            using (var fileStream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(fileContent);
                    writer.Flush();
                }
            }

            this.Load(tmpFilePath);
            File.Delete(tmpFilePath);
        }
    }
}
