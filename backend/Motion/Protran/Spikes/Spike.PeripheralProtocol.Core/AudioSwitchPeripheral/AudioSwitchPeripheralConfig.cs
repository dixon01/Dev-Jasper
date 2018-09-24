// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchPeripheralConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.IO.Ports;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    /// <summary>The audio switch config.</summary>
    [Serializable]
    [XmlRoot(ElementName = "PeripheralConfig")]
    public class AudioSwitchPeripheralConfig : PeripheralConfig
    {
        #region Constants

        /// <summary>The default audio switch config file name.</summary>
        public const string DefaultAudioSwitchConfigFileName = "AudioSwitchConfig.xml";

        #endregion

        /// <summary>Gets or sets a value indicating whether ignore first gpio event.</summary>
        public bool IgnoreFirstGpioEvent { get; set; }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AudioSwitchPeripheralConfig" /> class.
        ///     Initializes a new instance of the <see cref="Gorba.Common.Configuration.Obc.Common.SerialPortConfig" /> class.
        ///     Will be used the following default values:
        ///     com port name = "COM1"
        ///     baud rate = 115200;
        ///     data bits = 8;
        ///     parity = none;
        ///     stop bits = one;
        ///     DTR control = false;
        ///     RTS control = false;
        ///     parity check enabled = false;
        /// </summary>
        public AudioSwitchPeripheralConfig()
            : base("COM1", DefaultBaudRate, 8, Parity.None, StopBits.One, false, false)
        {
            this.IgnoreFirstGpioEvent = true;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The audio switch config factory.</summary>
        /// <param name="configFileName">The output config file.</param>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If something went wrong during deserialization.</exception>
        /// <returns>The <see cref="AudioSwitchPeripheralConfig"/>.</returns>
        public static AudioSwitchPeripheralConfig ReadAudioSwitchConfig(string configFileName = DefaultAudioSwitchConfigFileName)
        {
            try
            {
                lock (typeof(AudioSwitchPeripheralConfig))
                {
                    Logger.Info("Reading AudioSwitchPeripheralConfig file={0}", configFileName);
                    return ReadPeripheralConfig<AudioSwitchPeripheralConfig>(configFileName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ReadAudioSwitchConfig() Exception reading AudioSwitchConfig file {0}, Cause {1}", configFileName, ex.Message);
                throw;
            }
        }

        /// <summary>The write audio switch config.</summary>
        /// <param name="peripheralConfig">The config.</param>
        /// <param name="configFileName">The output config file.</param>
        public static void WriteAudioSwitchConfig(
            AudioSwitchPeripheralConfig peripheralConfig, 
            string configFileName = DefaultAudioSwitchConfigFileName)
        {
            lock (typeof(AudioSwitchPeripheralConfig))
            {
                WritePeripheralConfig(peripheralConfig, configFileName);
            }
        }

        #endregion
    }
}