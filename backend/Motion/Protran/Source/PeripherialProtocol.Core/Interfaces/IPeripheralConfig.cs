namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    /// <summary>The PeripheralConfig interface.</summary>
    public interface IPeripheralConfig
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the audio switch client should be used and is enabled. Default is true</summary>
        bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether peripheral header in network byte order. The default should be True
        ///     when using the real Audio Switch.
        /// </summary>
        bool PeripheralHeaderInNetworkByteOrder { get; set; }

        int DefaultMaxPeripheralFramingBytes { get; set; }

        /// <summary>Gets or sets the serial port settings.</summary>
        SerialPortSettings SerialPortSettings { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The read config.</summary>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        T ReadConfig<T>(string configFileName) where T : class, IPeripheralConfig, new();

        /// <summary>The write config.</summary>
        /// <param name="config">The config.</param>
        /// <param name="configFileName">The config file name.</param>
        /// <typeparam name="T"></typeparam>
        void WriteConfig<T>(T config, string configFileName) where T : class, IPeripheralConfig, new();

        #endregion
    }
}