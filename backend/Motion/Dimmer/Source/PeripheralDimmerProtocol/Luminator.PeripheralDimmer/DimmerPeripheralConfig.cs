// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerPeripheralConfig.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;
    using System.IO.Ports;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;

    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Processor;

    /// <summary>The dimmer peripheral config.</summary>
    [Serializable]
    [XmlRoot(ElementName = "PeripheralConfig")]
    public class DimmerPeripheralConfig : PeripheralConfig
    {
        #region Constants

        /// <summary>
        ///     Default dimmer peripheral config filename
        /// </summary>
        public const string DefaultDimmerPeripheralConfigFileName = "DimmerPeripheralConfig.xml";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DimmerPeripheralConfig" /> class.
        ///     Initializes a new instance of the <see cref="Gorba.Common.Configuration.Obc.Common.SerialPortConfig" /> class.
        ///     Will be used the following default values:
        ///     com port name = "COM2"
        ///     baud rate = 9600;
        ///     data bits = 8;
        ///     parity = none;
        ///     stop bits = one;
        ///     DTR control = false;
        ///     RTS control = false;
        ///     parity check enabled = false;
        /// </summary>
        public DimmerPeripheralConfig()
            : base("COM2", 9600, 8, Parity.None, StopBits.One, false, false)
        {
            this.Enabled = true;
            this.DefaultMaxPeripheralFramingBytes = 1;
            this.SerialPortSettings.ReceivedBytesThreshold = 7;
            this.DimmerSensorRequestInterval = 500;
            this.DimmerProcessorTuningParams = new DimmerProcessorTuningParams();
            this.MinimumPercentage = 10;
            this.MaximumPercentage = 90;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the dimmer processor tuning params.</summary>
        [XmlElement]
        public DimmerProcessorTuningParams DimmerProcessorTuningParams { get; set; }

        /// <summary>Gets or sets the dimmer sensor request interval.</summary>
        [XmlElement]
        public int DimmerSensorRequestInterval { get; set; }

        /// <summary>Gets or sets the minimum percentage.</summary>
        [XmlElement]
        public byte MinimumPercentage { get; set; }

        /// <summary>Gets or sets the maximum percentage.</summary>
        [XmlElement]
        public byte MaximumPercentage { get; set; }
        #endregion

        #region Public Methods and Operators

        /// <summary>The dimmer config factory.</summary>
        /// <param name="configFileName">The output config file.</param>
        /// <exception cref="ArgumentException">If fileName is not valid</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="ConfiguratorException">If something went wrong during deserialization.</exception>
        /// <returns>The <see cref="DimmerPeripheralConfig"/>.</returns>
        public static DimmerPeripheralConfig ReadDimmerPeripheralConfig(string configFileName = DefaultDimmerPeripheralConfigFileName)
        {
            try
            {
                lock (typeof(DimmerPeripheralConfig))
                {
                    Logger.Info("Reading DimmerPeripheralConfig file={0}", configFileName);
                    return ReadPeripheralConfig<DimmerPeripheralConfig>(configFileName);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ReadDimmerPeripheralConfig() Exception reading DimmerPeripheralConfig file {0}, Cause {1}", configFileName, ex.Message);
                throw;
            }
        }

        /// <summary>The write dimmer peripheral config.</summary>
        /// <param name="peripheralConfig">The config.</param>
        /// <param name="configFileName">The output config file.</param>
        public static void WriteDimmerPeripheralConfig(
            DimmerPeripheralConfig peripheralConfig, 
            string configFileName = DefaultDimmerPeripheralConfigFileName)
        {
            lock (typeof(DimmerPeripheralConfig))
            {
                WritePeripheralConfig(peripheralConfig, configFileName);
            }
        }

        #endregion
    }
}