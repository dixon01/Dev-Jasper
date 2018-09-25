namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.IO.Ports;
    using System.Xml.Serialization;

    using NLog;

    using Gorba.Common.Configuration.Core;

    using Luminator.PeripheralDimmer.Interfaces;

    /// <summary>The peripheral config.</summary>
    [Serializable]
    [XmlRoot(ElementName = "PeripheralConfig")]
    public class PeripheralConfig : IPeripheralConfig
    {
        #region Constants

        /// <summary>The default peripheral config name.</summary>
        public const string DefaultPeripheralConfigName = "PeripheralConfig.xml";

        /// <summary>The default baud rate.</summary>
        protected const int DefaultBaudRate = SerialPortSettings.DefaultBaudRate;

        #endregion

        #region Static Fields

        /// <summary>The logger.</summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly object configLock = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralConfig" /> class.</summary>
        public PeripheralConfig()
            : this("COM1", DefaultBaudRate, 8, Parity.None, StopBits.One, false, false)
        {
            this.Enabled = true;
            this.PeripheralHeaderInNetworkByteOrder = true;
            this.SerialPortSettings = new SerialPortSettings();
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralConfig"/> class. Initializes a new instance of the<see cref="Gorba.Common.Configuration.Obc.Common.SerialPortConfig"/> class.</summary>
        /// <param name="comPort">The serial port's name (without ':' at the end of the name).</param>
        /// <param name="baudRate">The serial port's baud rate.</param>
        /// <param name="dataBits">The serial port's data bits.</param>
        /// <param name="parity">The serial port's parity.</param>
        /// <param name="stopBits">The serial port's stop bit.</param>
        /// <param name="dtrControlEnable">The serial port's DTR control.</param>
        /// <param name="rtsControlEnable">The serial port's parity check.</param>
        /// <param name="receivedBytesThreshold">The number of bytes in the internal input buffer before a DataReceived event occurs.</param>
        public PeripheralConfig(
            string comPort,
            int baudRate = DefaultBaudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            bool dtrControlEnable = false,
            bool rtsControlEnable = false,
            int receivedBytesThreshold = DimmerConstants.RecieveBytesThreshold)
        {
            this.Enabled = true;
            this.PeripheralHeaderInNetworkByteOrder = true;
            this.SerialPortSettings = new SerialPortSettings(comPort, baudRate, dataBits, parity, stopBits, dtrControlEnable, rtsControlEnable, receivedBytesThreshold);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the audio switch client should be used and is enabled. Default is true</summary>
        [XmlElement]
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether peripheral header in network byte order. The default should be True
        ///     when using the real Audio Switch.
        /// </summary>
        [XmlElement]
        public bool PeripheralHeaderInNetworkByteOrder { get; set; }

        /// <summary>Gets or sets the serial port settings.</summary>
        [XmlElement]
        public SerialPortSettings SerialPortSettings { get; set; }

        /// <summary>Gets or sets the default max peripheral framing bytes.</summary>
        [XmlElement]
        public int DefaultMaxPeripheralFramingBytes { get; set; }

        /// <summary>
        ///     Gets or sets the version (always 1).
        /// </summary>
        [XmlAttribute("Version")]
        public int Version
        {
            get
            {
                return 1;
            }

            set
            {
                // nothing
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Read the peripheral config.</summary>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        public static T ReadPeripheralConfig<T>(string configFileName) where T : class, IPeripheralConfig, new()
        {
            try
            {
                lock (configLock)
                {
                    Logger.Trace("Reading Config file {0}", configFileName);
                    var configMgr = new ConfigManager<T> { FileName = configFileName };
                    return configMgr.Config;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ReadAudioSwitchConfig() Exception reading AudioSwitchConfig file {0}, Cause {1}", configFileName, ex.Message);
                throw;
            }
        }

        /// <summary>Write the peripheral config.</summary>
        /// <param name="config">The config.</param>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        public static void WritePeripheralConfig<T>(T config, string configFileName) where T : class, new()
        {
            lock (configLock)
            {
                Logger.Trace("Writing Config file {0}", configFileName);
                var configMgr = new ConfigManager<T> { FileName = configFileName };
                configMgr.CreateConfig();
                configMgr.SaveConfig();
            }
        }

        /// <summary>The read config.</summary>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        public T ReadConfig<T>(string configFileName) where T : class, IPeripheralConfig, new()
        {
            return ReadPeripheralConfig<T>(configFileName);
        }

        /// <summary>The write config.</summary>
        /// <param name="config">The config.</param>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        public void WriteConfig<T>(T config, string configFileName) where T : class, IPeripheralConfig, new()
        {
            WritePeripheralConfig(config, configFileName);
        }

        #endregion
    }
}