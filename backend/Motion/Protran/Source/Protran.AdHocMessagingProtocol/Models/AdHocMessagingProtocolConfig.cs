// InfomediaAll
// Luminator.Protran.AdHocMessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;

    using Gorba.Common.Configuration.Core;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    using NLog;

    /// <summary>The messaging protocol config.</summary>
    [Serializable]
    public class AdHocMessagingProtocolConfig : IAdHocMessagingProtocolConfig
    {
        /// <summary>The default api timeout.</summary>
        public const int DefaultApiTimeout = AdHocMessageServiceConfig.DefaultApiTimeout;

        /// <summary>The default config file name.</summary>
        public const string DefaultConfigFileName = "AdHocMessagingProtocolConfig.xml";

        /// <summary>The default max ad hoc messages.</summary>
        public const int DefaultMaxAdHocMessages = AdHocMessageServiceConfig.DefaultMaxAdHocMessages;

        public const int DefaultMaxAdhocRegistrationsAttempts = 3;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="AdHocMessagingProtocolConfig"/> class.</summary>
        public AdHocMessagingProtocolConfig()
        {
            this.AdHocMessageTimerSettings = new TimerSettings();
            this.RegisterUnitTimerSettings = new TimerSettings(30);
            this.EnableUnitRegistration = true;
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessagingProtocolConfig"/> class.</summary>
        /// <param name="adHocApiUri">The ad hoc api uri.</param>
        /// <param name="destinationsApiUri">The destinations api uri.</param>
        /// <param name="messagePollInterval">The message poll interval.</param>
        /// <param name="registerUnitInterval">The register unit interval.</param>
        /// <param name="enableUnitRegistration">The enable unit registration.</param>
        public AdHocMessagingProtocolConfig(
            Uri adHocApiUri,
            Uri destinationsApiUri,
            TimeSpan messagePollInterval,
            TimeSpan registerUnitInterval,
            bool enableUnitRegistration = true)
        {
            this.AdHocApiUri = new UriSettings(adHocApiUri);
            this.DestinationsApiUri = new UriSettings(destinationsApiUri);
            this.AdHocMessageTimerSettings = new TimerSettings(messagePollInterval);
            this.RegisterUnitTimerSettings = new TimerSettings(registerUnitInterval);
            this.EnableUnitRegistration = enableUnitRegistration;
        }

        /// <summary>Gets or sets the ad hoc api uri.</summary>
        public UriSettings AdHocApiUri { get; set; }

        /// <summary>Gets or sets the ad hoc message timer settings.</summary>
        public TimerSettings AdHocMessageTimerSettings { get; set; }

        /// <summary>Gets or sets the REST api timeout.</summary>
        public int ApiTimeout { get; set; } = DefaultApiTimeout;

        /// <summary>Gets or sets the destinations api uri.</summary>
        public UriSettings DestinationsApiUri { get; set; }

        /// <summary>Gets or sets a value indicating whether units will be registered.</summary>
        public bool EnableUnitRegistration { get; set; }

        public int MaxAdHocMessages { get; set; } = DefaultMaxAdHocMessages;

        /// <summary>Gets or sets the max adhoc registration attempts.</summary>
        public int MaxAdhocRegistrationAttempts { get; set; } = DefaultMaxAdhocRegistrationsAttempts;

        /// <summary>Gets or sets the register unit timer settings.</summary>
        public TimerSettings RegisterUnitTimerSettings { get; set; }

        /// <summary>Gets or sets the request unit info timer settings.</summary>
        public TimerSettings RequestUnitInfoTimerSettings { get; set; }

        /// <summary>Read the config.</summary>
        /// <param name="configFileName">The config file name.</param>
        /// <returns>The <see cref="AdHocMessagingProtocolConfig"/>.</returns>
        public static AdHocMessagingProtocolConfig ReadConfig(string configFileName)
        {
            try
            {
                Logger.Info("Reading AudioSwitchPeripheralConfig file={0}", configFileName);

                var configMgr = new ConfigManager<AdHocMessagingProtocolConfig> { FileName = configFileName };
                return configMgr.Config;
            }
            catch (Exception ex)
            {
                Logger.Error("ReadAudioSwitchConfig() Exception reading AudioSwitchConfig file {0}, Cause {1}", configFileName, ex.Message);
                throw;
            }
        }

        /// <summary>Write the config.</summary>
        /// <param name="config">The config.</param>
        /// <param name="configFileName">The config file name.</param>
        public static void WriteConfig(AdHocMessagingProtocolConfig config, string configFileName)
        {
            Logger.Trace("Writing Config file {0}", configFileName);
            var configMgr = new ConfigManager<AdHocMessagingProtocolConfig> { FileName = configFileName };
            configMgr.SaveConfig(config);
        }
    }
}