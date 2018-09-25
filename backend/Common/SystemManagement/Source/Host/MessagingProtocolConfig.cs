// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.MessagingProtocol
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    using Luminator.Motion.Protran.MessagingProtocol.
    using NLog;

    [Serializable]
    public class MessagingProtocolConfig : IMessagingProtocolConfig
    {
        public const string DefaultConfigFileName = "MessagingProtocolConfig.xml";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [XmlIgnore]
        public Uri HostUri { get; set; }

        [XmlElement("HostUri")]
        public string UriXml
        {
            get
            {
                return this.HostUri != null ? this.HostUri.AbsoluteUri : string.Empty;
            }
            set
            {
                this.HostUri = new Uri(value);
            }
        }

        [XmlIgnore]
        public TimeSpan MessagePollInterval { get; set; } = TimeSpan.Zero;

        /// <summary>
        ///     Gets or sets the timeout in XML serializable format.
        /// </summary>
        [XmlElement("MessagePollInterval", DataType = "duration")]
        public string MessagePollIntervalXml
        {
            get => XmlConvert.ToString(this.MessagePollInterval);

            set => this.MessagePollInterval = XmlConvert.ToTimeSpan(value);
        }

        public static MessagingProtocolConfig ReadConfig(string configFileName)
        {
            try
            {
                lock (typeof(MessagingProtocolConfig))
                {
                    Logger.Info("Reading AudioSwitchPeripheralConfig file={0}", configFileName);

                    var configMgr = new ConfigManager<MessagingProtocolConfig> { FileName = configFileName };
                    return configMgr.Config;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    "ReadAudioSwitchConfig() Exception reading AudioSwitchConfig file {0}, Cause {1}",
                    configFileName,
                    ex.Message);
                throw;
            }
        }

        public static void WriteConfig(MessagingProtocolConfig config, string configFileName)
        {
            Logger.Trace("Writing Config file {0}", configFileName);
            var configMgr = new ConfigManager<MessagingProtocolConfig> { FileName = configFileName };
            configMgr.SaveConfig(config);
        }
    }
}