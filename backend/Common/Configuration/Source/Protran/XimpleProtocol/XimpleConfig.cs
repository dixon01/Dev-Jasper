// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Protran.XimpleProtocol
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    using Gorba.Common.Configuration.Core;

    /// <summary>
    ///     The Ximple protocol configuration object.
    /// </summary>
    [Serializable]
    public class XimpleConfig : IXimpleConfig
    {
        #region Constants

        /// <summary>The default port.</summary>
        public const int DefaultPort = 1600;

        /// <summary>The infotainment audio status table index default.</summary>
        public const int InfoTainmentAudioStatusTableIndexDefault = 104;

        /// <summary>The infotainment canned message play table index default.</summary>
        public const int InfoTainmentCannedMsgPlayTableIndexDefault = 101;

        /// <summary>The infotainment system status default.</summary>
        public const int InfoTainmentSystemStatusTableIndexDefault = 105;

        /// <summary>
        ///     Default Dictionary Table for Network Connection Changes, see dictionary.xml
        /// </summary>
        public const int InfoTainmentVolumeSettingsTableIndexDefault = 100;

        /// <summary>The network changed message table index default.</summary>
        public const int NetworkChangedMessageTableIndexDefault = 102;

        /// <summary>The network shared folder table index default.</summary>
        public const int NetworkFileAccessSettingsTableIndexDefault = 103;

        public const int RouteTableIndex = 10;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="XimpleConfig" /> class.</summary>
        public XimpleConfig()
        {
            this.Enabled = true;
            this.EnableResponse = false;
            this.EnableXimpleOnlyResponse = false;
            this.Port = DefaultPort;
            this.InfoTainmentVolumeSettingsTableIndex = InfoTainmentVolumeSettingsTableIndexDefault;
            this.InfoTainmentCannedMsgPlayTableIndex = InfoTainmentCannedMsgPlayTableIndexDefault;
            this.NetworkChangedMessageTableIndex = NetworkChangedMessageTableIndexDefault;
            this.NetworkFileAccessSettingsTableIndex = NetworkFileAccessSettingsTableIndexDefault;
            this.InfoTainmentAudioStatusTableIndex = InfoTainmentAudioStatusTableIndexDefault;
            this.InfoTainmentSystemStatusTableIndex = InfoTainmentSystemStatusTableIndexDefault;
            this.NetworkFtpSettings = new NetworkFtpSettings();
            this.AudioZonePresentationValues = new AudioZonePresentationValues();
            this.XimpleServerMonitorTimerInterval = 60;
            this.MaxXimpleServerFailuresBeforeRestart = 2;
        }

        /// <summary>Initializes a new instance of the <see cref="XimpleConfig"/> class.</summary>
        /// <param name="port">The port.</param>
        public XimpleConfig(int port)
            : this()
        {
            this.Port = port;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the XSD schema for this config structure.
        /// </summary>
        /// <exception cref="FileNotFoundException" accessor="get">Couldn't find ximpleProtocol.xsd resource</exception>
        public static XmlSchema Schema
        {
            get
            {
                using (var input = typeof(XimpleConfig).Assembly.GetManifestResourceStream(typeof(XimpleConfig), "ximpleProtocol.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find ximpleProtocol.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>Gets or sets the audio zone presentation values.</summary>
        public AudioZonePresentationValues AudioZonePresentationValues { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this serial port is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets a value indicating whether enable response.</summary>
        public bool EnableResponse { get; set; }

        /// <summary>Gets or sets a value indicating whether enable ximple only response. Future Option</summary>
        public bool EnableXimpleOnlyResponse { get; set; }

        /// <summary>Gets or sets the infotainment audio status table index.</summary>
        public int InfoTainmentAudioStatusTableIndex { get; set; }

        /// <summary>Gets or sets the infotainment canned message play table index.</summary>
        public int InfoTainmentCannedMsgPlayTableIndex { get; set; }

        /// <summary>Gets or sets the infotainment system status.</summary>
        public int InfoTainmentSystemStatusTableIndex { get; set; }

        /// <summary>Gets or sets the audio settings table index.</summary>
        public int InfoTainmentVolumeSettingsTableIndex { get; set; }

        /// <summary>Gets or sets the network connection table index to use from the Dictionary.xml file.</summary>
        public int NetworkChangedMessageTableIndex { get; set; }

        /// <summary>Gets or sets the network shared access settings table index.</summary>
        public int NetworkFileAccessSettingsTableIndex { get; set; }

        /// <summary>Gets or sets the network ftp settings.</summary>
        public NetworkFtpSettings NetworkFtpSettings { get; set; }

        /// <summary>Gets or sets the port.</summary>
        public int Port { get; set; }

        public int XimpleServerMonitorTimerInterval { get; set; }

        public int MaxXimpleServerFailuresBeforeRestart { get; set; }

        #endregion

        #region Properties
        
        #endregion

        #region Public Methods and Operators

        /// <summary>Read the Ximple config.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The <see cref="XimpleConfig"/>.</returns>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        public static XimpleConfig Read(string filename = "XimpleConfig.xml")
        {
            var configMgr = new ConfigManager<XimpleConfig> { FileName = filename };
            return configMgr.Config;
        }

        /// <summary>
        /// Write the config to file
        /// </summary>
        /// <param name="config">Configuration to write</param>
        /// <param name="filename">File Name</param>
        public static void Write(XimpleConfig config, string filename = "XimpleConfig.xml")
        {
            var configMgr = new ConfigManager<XimpleConfig> { FileName = filename };
            configMgr.SaveConfig(config);
        }

        #endregion
    }
}