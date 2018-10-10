// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigLoader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Core
{
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// This object has to load a specific configuration file, parse it
    /// and store all the notable information into suitable object.
    /// If the specific file does not exist, the "Load" function will fail.
    /// You can use the "CreateDefault" function to create the default
    /// configuration file.
    /// </summary>
    public class ConfigLoader
    {
        /// <summary>
        /// The object tasked to serialize/deserialize object in XML and vice versa.
        /// </summary>
        private readonly ConfigManager<ProtranConfig> configManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigLoader"/> class.
        /// </summary>
        /// <param name="fileAbsName">
        /// The absolute file's name of the configuration
        /// file to load.
        /// </param>
        public ConfigLoader(string fileAbsName)
        {
            this.ProtranConfig = null;
            if (string.IsNullOrEmpty(fileAbsName))
            {
                // invalid data.
                // I cannot continue with this constructor.
                return;
            }

            // I store the variable for the future.
            this.configManager = new ConfigManager<ProtranConfig> { FileName = fileAbsName, EnableCaching = true };
            this.configManager.XmlSchema = ProtranConfig.Schema;
        }

        /// <summary>
        /// Gets the root TAG of the configuration file if loaded,
        /// otherwise null.
        /// </summary>
        public ProtranConfig ProtranConfig { get; private set; }

        /// <summary>
        /// Try to load the configuration file.
        /// </summary>
        /// <returns>True if the load operation has finished with success, else false.</returns>
        public bool Load()
        {
            this.ProtranConfig = this.configManager.Config;
            return this.ProtranConfig != null;
        }

        /// <summary>
        /// Saves the current settings into the equivalent XML string
        /// into the configuration file.
        /// </summary>
        /// <returns>True if the save operation was done with success, else false.</returns>
        public bool Save()
        {
            this.configManager.SaveConfig();
            return true;
        }
    }
}
