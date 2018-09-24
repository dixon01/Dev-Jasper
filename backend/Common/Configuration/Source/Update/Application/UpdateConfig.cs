// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UpdateConfig.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Core;
    using Update.Clients;
    using Update.Providers;

    /// <summary>
    ///     Configuration for an update
    /// </summary>
    [XmlRoot("Update")]
    [Serializable]
    public class UpdateConfig
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateConfig" /> class.
        /// </summary>
        public UpdateConfig()
        {
            this.Agent = new AgentConfig();
            this.Clients = new List<UpdateClientConfigBase>();
            this.Providers = new List<UpdateProviderConfigBase>();
            this.Visualization = new VisualizationConfig();
            this.CacheLimits = new CacheLimitsConfig();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (var input = typeof(UpdateConfig).Assembly.GetManifestResourceStream(typeof(UpdateConfig), "Update.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find Update.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the update agent configuration.
        /// </summary>
        public AgentConfig Agent { get; set; }

        /// <summary>
        ///     Gets or sets all the update clients.
        /// </summary>
        [XmlArrayItem("USBUpdateClient", typeof(UsbUpdateClientConfig))]
        [XmlArrayItem("FTPUpdateClient", typeof(FtpUpdateClientConfig))]
        [XmlArrayItem("MediUpdateClient", typeof(MediUpdateClientConfig))]
        [XmlArrayItem("AzureUpdateClient", typeof(AzureUpdateClientConfig))]
        public List<UpdateClientConfigBase> Clients { get; set; }

        /// <summary>
        ///     Gets or sets all the update providers.
        /// </summary>
        [XmlArrayItem("USBUpdateProvider", typeof(UsbUpdateProviderConfig))]
        [XmlArrayItem("FTPUpdateProvider", typeof(FtpUpdateProviderConfig))]
        [XmlArrayItem("MediUpdateProvider", typeof(MediUpdateProviderConfig))]
        public List<UpdateProviderConfigBase> Providers { get; set; }

        /// <summary>
        ///     Gets or sets the update progress visualization configuration.
        /// </summary>
        public VisualizationConfig Visualization { get; set; }

        /// <summary>
        ///     Gets or sets the folder limit configuration.
        /// </summary>
        public CacheLimitsConfig CacheLimits { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Read the UpdateManager config.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The <see cref="XimpleConfig"/>.</returns>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        public static UpdateConfig Read(string filename = "Update.xml", bool enableSchema = false)
        {
            var configMgr = new ConfigManager<UpdateConfig> { FileName = filename };
            if (enableSchema)
            {
                configMgr.XmlSchema = Schema;
            }
            return configMgr.Config;
        }

        #endregion
    }
}