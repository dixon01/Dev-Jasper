// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralAudioConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    /*    
    typedef struct _g3audioconfig_t
    {
        PCP_HEADER      hdr;
        uint_8          pinMeaning[16];     // ordered by Pin Number LSB -> MSB (PCP_07_GPIO_MEANING)
        uint_8          pinSense[16];       // 1 = Active High 0 = Active Low
        uint_8          noiseLevel;         // 0 - 16 noise levels 0 = no compensation
        uint_8          interiorGain[16];   // Interior Gain Levels (0-10) for each of 16 noise levels
        uint_8          exteriorGain[16];   // Exterior Gain Levels (0-10) for each of 16 noise levels
        uint_8          priorityTable[3];   // Source Priority in array order
        uint_8          defaultVolume;      // 0 - 100
        uint_8          intMinVolume;       // 0 - 100
        uint_8          intMaxVolume;       // 0 - 100 - must be greater than min
        uint_8          extMinVolume;       // 0 - 100
        uint_8          extMaxVolume;       // 0 - 100 - must be greater than min
        uint_16         pttTimeout;         // Maximum time PTT active before lockout
        uint_16         pttLockoutTime;     // PTT Lockout reset delay in seconds
        uint_8          lineInEnable;       // 0 = not used 1 = used
        uint_16         audioStatusDelay;   // Time in ms. between audio status messages 0 = no message unless POLLED 
        uint_8          chksum;
    } PACKED(PCP_07_AUDIO_CFG_PKT);
    */

    // New Structure 10/2016

    /*    
     *    This is the new structure 10/10/2016 to replace the old one
    typedef struct _g3audioconfig_t
    {
        PCP_HEADER      hdr;
        uint_8          pinMeaning[16];     // ordered by Pin Number LSB -> MSB (PCP_07_GPIO_MEANING)
        uint_8          pinSense[16];       // 1 = Active High 0 = Active Low
        uint_8          noiseLevel;         // 0 - 16 noise levels 0 = no compensation
        uint_8          interiorGain[16];   // Interior Gain Levels (0-10) for each of 16 noise levels
        uint_8          exteriorGain[16];   // Exterior Gain Levels (0-10) for each of 16 noise levels
        uint_8          priorityTable[3];   // Source Priority in array order
        uint_8          intDefaultVolume;   New// 0 - 100
        uint_8          extDefaultVolume;   New // 0 - 100
        uint_8          intMinVolume;       // 0 - 100
        uint_8          intMaxVolume;       // 0 - 100 - must be greater than min
        uint_8          intMaxAllowVolume; New // 0 - 100 – Max Allowed Noise Adjust Volume - must be greater than min
        uint_8          extMinVolume;       // 0 - 100
        uint_8          extMaxVolume;       // 0 - 100 - must be greater than min
        uint_8          extMaxAllowVolume; New // 0 - 100 - Max Allowed Noise Adjust Volume - must be greater than min
        uint_16         pttTimeout;         // Maximum time PTT active before lockout
        uint_16         pttLockoutTime;     // PTT Lockout reset delay in seconds
        uint_16         audioStatusDelay;   // Time in ms. between audio status messages 0 = no message unless POLLED 
        uint_8          lineInEnable;       // 0 = not used 1 = used
        uint_8          chksum;
    } PACKED(PCP_07_AUDIO_CFG_PKT);
    */

    /// <summary>The peripheral config.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioConfig : IPeripheralAudioConfig
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Write Lock for WritePeripheralAudioConfigVolumes
        /// </summary>
        private static readonly object ReadWriteLock = new object();

        /// <summary>The expected size.</summary>
        public const int Size = 90;

        // Expected array sizes

        /// <summary>The pin meaning size.</summary>
        public const int PinMeaningSize = 16;

        /// <summary>The pin sense size.</summary>
        public const int PinSenseSize = 16;

        /// <summary>The interior gain size.</summary>
        public const int InteriorGainSize = 16;

        /// <summary>The exterior gain size.</summary>
        public const int ExteriorGainSize = 16;

        /// <summary>The priority table size.</summary>
        public const int PriorityTableSize = 3;

        /// <summary>
        ///     Default Config file name
        /// </summary>
        public const string DefaultPeripheralAudioConfigXmlFile = "PeripheralAudioConfig.xml";

        /// <summary>The default noise level.</summary>
        public const byte DefaultNoiseLevel = 10;

        /// <summary>The default interior volume level.</summary>
        public const byte InteriorDefaultVolumeLevel = 30;

        /// <summary>The default exterior volume level.</summary>
        public const byte ExteriorDefaultVolumeLevel = 30;

        private byte interiorDefaultVolume;

        private byte interiorMinVolume;

        private byte interiorMaxVolume;

        private byte exteriorMinVolume;

        private byte exteriorMaxVolume;

        private byte exteriorDefaultVolume;

        private ushort audioStatusUpdateInterval;

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioConfig" /> class.</summary>
        public PeripheralAudioConfig()
        {
            this.Header = new PeripheralHeader(PeripheralMessageType.AudioConfig, (ushort)Marshal.SizeOf(this));
            this.PinMeaning = new byte[PinMeaningSize]
                                  {
                                      (byte)PeripheralGpioType.Door1, (byte)PeripheralGpioType.Door2, (byte)PeripheralGpioType.StopRequest, 
                                      (byte)PeripheralGpioType.AdaStopRequest, (byte)PeripheralGpioType.PushToTalk, 
                                      (byte)PeripheralGpioType.InteriorActive, (byte)PeripheralGpioType.ExteriorActive, 
                                      (byte)PeripheralGpioType.Odometer, (byte)PeripheralGpioType.Reverse, (byte)PeripheralGpioType.InteriorSpeakerMuted, 
                                      (byte)PeripheralGpioType.ExteriorSpeakerMuted, (byte)PeripheralGpioType.RadioSpeakerMuted, 0, 0, 0, 0
                                  };



            this.PinSense = new byte[PinSenseSize] { 1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            this.InteriorGain = new byte[InteriorGainSize] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 };
            this.ExteriorGain = new byte[ExteriorGainSize] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 };
            this.PriorityTable = new[]
                                     {
                                         (byte)AudioSourcePriority.DriverMic, (byte)AudioSourcePriority.DefaultPlatform, 
                                         (byte)AudioSourcePriority.LineIn
                                     };
            this.InteriorMaxVolume = Constants.MaxVolume;
            this.ExteriorMaxVolume = Constants.MaxVolume;
            this.NoiseLevel = DefaultNoiseLevel;
            this.InteriorDefaultVolume = InteriorDefaultVolumeLevel;
            this.ExteriorDefaultVolume = ExteriorDefaultVolumeLevel;
            this.InteriorMaxAllowedVolume = this.InteriorMaxVolume;
            this.ExteriorMaxAllowedVolume = this.ExteriorMaxVolume;
            this.AudioStatusDelay = 0;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Gets or sets the header.</summary>
        [Order]
        [XmlIgnore]
        public PeripheralHeader Header { get; set; }

        /// <summary>The pin meaning. ordered by Pin Number LSB -> MSB (see enum PeripheralGpioType)</summary>
        [Order]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PinMeaningSize)]
        [XmlElement(DataType = "hexBinary")]
        public byte[] PinMeaning;

        /// <summary>The pin sense. 1 = Active High 0 = Active Low</summary>
        [Order]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PinSenseSize)]
        [XmlElement(DataType = "hexBinary")]
        public byte[] PinSense;

        /// <summary>Gets or sets the noise level. 0 - 16 noise levels 0 = no compensation</summary>
        [Order]
        public byte NoiseLevel { get; set; }

        /// <summary>The interior gain. Interior Gain Levels (0-10) for each of 16 noise levels</summary>
        [Order]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = InteriorGainSize)]
        [XmlElement(DataType = "hexBinary")]
        public byte[] InteriorGain;

        /// <summary>The exterior gain. Exterior Gain Levels (0-10) for each of 16 noise levels</summary>
        [Order]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ExteriorGainSize)]
        [XmlElement(DataType = "hexBinary")]
        public byte[] ExteriorGain;

        /// <summary>The priority.  Source Priority in array order</summary>
        [Order]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PriorityTableSize)]
        [XmlElement(DataType = "hexBinary")]
        public byte[] PriorityTable;

        /// <summary>Gets or sets the default volume.</summary>
        [Order]
        public byte InteriorDefaultVolume
        {
            get
            {
                return this.interiorDefaultVolume;
            }

            set
            {
                this.interiorDefaultVolume = this.ValidateVolume(value);
            }
        }

        [Order]
        public byte ExteriorDefaultVolume
        {
            get
            {
                return this.exteriorDefaultVolume;
            }

            set
            {
                this.exteriorDefaultVolume = this.ValidateVolume(value);
            }
        }

        /// <summary>Gets or sets the interior min volume 0 - 100.</summary>
        [Order]
        public byte InteriorMinVolume
        {
            get
            {
                return this.interiorMinVolume;
            }

            set
            {
                this.interiorMinVolume = this.ValidateVolume(value);
            }
        }

        /// <summary>Gets or sets the interior max volume 0 - 100 - must be greater than min.</summary>
        [Order]
        public byte InteriorMaxVolume
        {
            get
            {
                return this.interiorMaxVolume;
            }

            set
            {
                this.interiorMaxVolume = this.ValidateVolume(value);
            }
        }

        [Order]
        public byte InteriorMaxAllowedVolume { get; set; }

        /// <summary>Gets or sets the exterior min volume  0 - 100.</summary>
        [Order]
        public byte ExteriorMinVolume
        {
            get
            {
                return this.exteriorMinVolume;
            }

            set
            {
                this.exteriorMinVolume = this.ValidateVolume(value);
            }
        }

        /// <summary>Gets or sets the exterior max volume. 0 - 100 - must be greater than min</summary>
        [Order]
        public byte ExteriorMaxVolume
        {
            get
            {
                return this.exteriorMaxVolume;
            }

            set
            {
                this.exteriorMaxVolume = this.ValidateVolume(value);
            }
        }

        /// <summary>Gets or sets the exterior max allowed volume.</summary>
        [Order]
        public byte ExteriorMaxAllowedVolume { get; set; }

        /// <summary>Gets or sets the put to talk timeout. Maximum time PTT active before lockout</summary>
        [Order]
        public ushort PushToTalkTimeout { get; set; }

        /// <summary>Gets or sets the put to talk lockout time. PTT Lockout reset delay in seconds</summary>
        [Order]
        public ushort PushToTalkLockoutTime { get; set; }
     
        /// <summary>
        ///     Gets or sets the audio status delay to receive real time status updates. Time in milliseconds. between audio
        ///     status messages 0 = disabled
        /// </summary>
        [Order]
        public ushort AudioStatusDelay
        {
            get
            {
                return this.audioStatusUpdateInterval;
            }

            set
            {
                // TODO move MinAudioStatusUpdateInterval const to config and make as setting
                if (value != this.audioStatusUpdateInterval && (value == 0 || (value >= Constants.MinAudioStatusUpdateInterval && value <= 10000)))
                {
                    this.audioStatusUpdateInterval = value;
                }
            }
        }

        /// <summary>Gets or sets the line in enable. 0 = not used 1 = used</summary>
        [Order]
        public byte LineInEnable { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        [XmlIgnore]
        public byte Checksum { get; set; }

        /// <summary>Validate volume level is in range, throws ArgumentOutOfRangeException</summary>
        /// <param name="value">Volume level</param>
        /// <param name="minVolume">Default min volume</param>
        /// <param name="maxVolume"Default max volume<></param>
        /// <returns>The value if valid, else exception is thrown</returns>
        private byte ValidateVolume(byte value, byte minVolume = Constants.MinVolume, byte maxVolume = Constants.MaxVolume)
        {
            if (minVolume >= 0 && maxVolume > 0)
            {
                if (value < minVolume || value > maxVolume)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("Invalid Volume {0} outside of range {1} -{2}", value, minVolume, maxVolume));
                }
            }
          
            return value;
        }

        /// <summary>Read the audio switch Peripheral Audio config from file.</summary>
        /// <param name="outputConfigFile">The output config file.</param>
        /// <returns>The <see cref="PeripheralAudioConfig"/>.</returns>
        public static PeripheralAudioConfig ReadPeripheralAudioConfig(string outputConfigFile = DefaultPeripheralAudioConfigXmlFile)
        {
            try
            {
                Logger.Info("{0} Reading Audio Config file {1}", nameof(ReadPeripheralAudioConfig), outputConfigFile);
                lock (ReadWriteLock)
                {
                    var configMgr = new ConfigManager<PeripheralAudioConfig>(outputConfigFile) { EnableCaching = false };
                    var config = configMgr.Config;
                    CheckSumUtil.CalculateCheckSum(config);
                    return config;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error {0} Reading config File: {1}, Exception {2}", nameof(ReadPeripheralAudioConfig), outputConfigFile, ex.Message);
                throw;
            }
        }

        /// <summary>Write peripheral audio config to file.</summary>
        /// <param name="config">The config.</param>
        /// <param name="outputConfigFile">The output config file.</param>
        /// <returns>The <see cref="bool"/>True if file exists</returns>
        /// <exception cref="NullReferenceException">
        /// Raised if the Config property is null.
        /// </exception>
        public static bool WritePeripheralAudioConfig(PeripheralAudioConfig config, string outputConfigFile = DefaultPeripheralAudioConfigXmlFile)
        {
            lock (ReadWriteLock)
            {
                var configMgr = new ConfigManager<PeripheralAudioConfig> { FileName = outputConfigFile };
                config.AudioStatusDelay = 0;    // Keep from being saved active
                configMgr.SaveConfig(config);
                return File.Exists(configMgr.FullConfigFileName);
            }
        }    

        /// <summary>Write changes to the peripheral audio config for volumes.</summary>
        /// <param name="volumeSettings">The volume settings.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="bool"/>True if file exists</returns>
        public static bool WritePeripheralAudioConfigVolumes(
            VolumeSettingsMessage volumeSettings, 
            string fileName = DefaultPeripheralAudioConfigXmlFile)
        {
            lock (ReadWriteLock)
            {
                // get the current settings from file & change just the min, max and default volumes
                var config = ReadPeripheralAudioConfig(fileName);
                if (config != null)
                {
                    config.InteriorDefaultVolume = volumeSettings.Interior.DefaultVolume;
                    config.InteriorMinVolume = volumeSettings.Interior.MinimumVolume;
                    config.InteriorMaxVolume = volumeSettings.Interior.MaximumVolume;

                    config.ExteriorMinVolume = volumeSettings.Exterior.MinimumVolume;
                    config.ExteriorMaxVolume = volumeSettings.Exterior.MaximumVolume;
                    config.ExteriorDefaultVolume = volumeSettings.Exterior.DefaultVolume;

                    config.AudioStatusDelay = 0;    // we do not want to save this value non-zero

                    // persist the settings to file
                    return WritePeripheralAudioConfig(config, fileName);
                }

                return false;
            }
        }

        public override string ToString()
        {
            return typeof(AudioStatusMessage).GetProperties().Aggregate(string.Empty, (current, p) => current + string.Format("  {0}={1}\r\n", p.Name, p.GetValue(this)));
        }   
    }
}