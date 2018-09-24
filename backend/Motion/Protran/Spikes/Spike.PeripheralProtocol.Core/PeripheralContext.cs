// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralContext.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core
{
    using System.IO;
    using System.Xml;

    using Gorba.Common.Configuration.Core;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>The Peripheral context.</summary>
    public class PeripheralContext<TMessageType> : IPeripheralContext
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralContext"/> class.</summary>
        /// <param name="configFileName">The config File Name.</param>
        /// <exception cref="FileNotFoundException">If the file AudioSwitchConfig.xml is not found</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        public PeripheralContext(string configFileName)
        {
            if (string.IsNullOrEmpty(configFileName))
            {
                configFileName = PeripheralConfig.DefaultPeripheralConfigName;
            }

            var configManager = new ConfigManager<PeripheralConfig>(configFileName);
            this.Config = configManager?.Config;
            this.Stream = null;
            this.PeripheralVersions = null;
            this.PeripheralSerialClient = null;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralContext"/> class.</summary>
        /// <param name="config">The config.</param>
        /// <param name="stream">The stream.</param>
        public PeripheralContext(PeripheralConfig config, Stream stream)
        {
            this.Config = config;
            this.Stream = stream;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralContext"/> class.</summary>
        /// <param name="config">The config.</param>
        /// <param name="peripheralSerialClient">The peripheral serial client.</param>
        public PeripheralContext(PeripheralConfig config, IPeripheralSerialClient<TMessageType> peripheralSerialClient)
        {
            this.Config = config;
            this.PeripheralSerialClient = peripheralSerialClient;
            this.Stream = peripheralSerialClient.SerialPort.BaseStream;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the Peripheral config.</summary>
        public PeripheralConfig Config { get; set; }

        /// <summary>Gets or sets the peripheral serial client.</summary>
        public IPeripheralSerialClient<TMessageType> PeripheralSerialClient { get; set; }

        /// <summary>Gets or sets the peripheral versions.</summary>
        public PeripheralVersionsInfo PeripheralVersions { get; set; }

        /// <summary>Gets or sets the stream.</summary>
        public Stream Stream { get; set; }

        #endregion
    }
}