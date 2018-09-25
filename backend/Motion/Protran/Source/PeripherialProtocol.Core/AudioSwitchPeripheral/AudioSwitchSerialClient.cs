// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchSerialClient.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    /// <summary>The audio switch client.</summary>
    public class AudioSwitchSerialClient : PeripheralSerialClient<AudioSwitchHandler>, IAudioSwitchSerialClient
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly object UpdateImageLock = new object();

        #endregion

        #region Fields

        private ulong audioStatusRequestedCount;    // debug info

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AudioSwitchSerialClient"/> class. Construct the handler from the
        ///     given switch configuration.</summary>
        /// <returns>The <see cref="AudioSwitchHandler"/>.</returns>
        /// <summary>Initializes a new instance of the <see cref="AudioSwitchSerialClient"/> class. The audio switch client.</summary>
        /// <param name="peripheralConfig">The audio Switch Config.</param>
        public AudioSwitchSerialClient(PeripheralConfig peripheralConfig)
            : base(peripheralConfig)
        {
            this.SubscribeAudioSwitchEvents();
        }

        /// <summary>Initializes a new instance of the <see cref="AudioSwitchSerialClient"/> class.</summary>
        /// <param name="serialPortSettings">The serial port settings.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException">Reader thread already started</exception>
        /// <exception cref="ConfiguratorException">If something went wrong during de-serialization.</exception>
        public AudioSwitchSerialClient(SerialPortSettings serialPortSettings)
            : base(serialPortSettings, AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName)
        {
            this.SubscribeAudioSwitchEvents();
        }

        #endregion

        #region Public Events

        /// <summary>The audio status changed event.</summary>
        public event EventHandler<AudioStatusMessage> AudioStatusChanged;

        /// <summary>Peripheral GPIO changed event.</summary>
        public event EventHandler<PeripheralGpioEventArg> GpioChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>The close.</summary>
        public override void Close()
        {
            Logger.Info("Closing Audio Switch Client");
            base.Close();
            this.GpioChanged -= this.OnAudioHandlerGpioChangedEventHandler;
            this.AudioStatusChanged -= this.OnAudioHandlerStatusMessageEventHandler;
        }

        /// <summary>Read peripheral audio config file from xml.</summary>
        /// <param name="audioConfigFile">The audio config file.</param>
        /// <returns>The <see cref="PeripheralAudioConfig"/>.</returns>
        public PeripheralAudioConfig ReadPeripheralAudioConfigFile(string audioConfigFile)
        {
            return PeripheralAudioConfig.ReadPeripheralAudioConfig(audioConfigFile);
        }

        /// <summary>Write the audio config xml settings to the serial port stream. The reply is the Peripheral Version message.</summary>
        /// <param name="fileName">The file name or empty string use the default DefaultPeripheralAudioConfigXmlFile</param>
        /// <param name="audioStatusDelay">The optional audio Status Delay that cause the hardware to send(TX) Audio Status to the
        ///     client at this interval.</param>
        /// <returns>The <see cref="int"/>Number of bytes written.</returns>
        /// <exception cref="Exception">Reading the Config fails, logs the error and re-throws the exception.</exception>
        public int WriteAudioConfig(string fileName = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile, ushort audioStatusDelay = 0)
        {
            try
            {
                Logger.Info("{0} Enter file:{1}", nameof(WriteAudioConfig), fileName);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;
                }

                var peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig(fileName);
                Logger.Info("Modify the Audio Config, Default Volumes Interior:{0}, Exterior:{1}", peripheralAudioConfig.InteriorDefaultVolume, peripheralAudioConfig.ExteriorDefaultVolume);

                // the Peripheral will send us it's version, store this off in the state object for later retrieval
                // once we get the version we Ack the peripheral and it will reply with the current GPIO status

                // Zero is disabled or off for the status refresh interval
                // When non zero the hardware will send an Audio Status message on the port every X milliseconds
                peripheralAudioConfig.AudioStatusDelay = audioStatusDelay;
                var byteWritten = this.WriteAudioConfig(peripheralAudioConfig);
                return byteWritten;

            }
            catch (Exception ex)
            {
                Logger.Error("{0}() Exception for file {1}, {2}", nameof(WriteAudioConfig), fileName, ex.Message);
                throw;
            }
        }

        /// <summary>The write audio config. The reply should be the Audio Version & GPIO status. PeripherialAudioVersions</summary>
        /// <param name="audioConfig">The audio config.</param>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="IOException">A I/O error occurs. </exception>
        public int WriteAudioConfig(PeripheralAudioConfig audioConfig)
        {
            Logger.Info("Writing AudioSwitch Configuration Enter");
            return this.Write(audioConfig);
        }

        /// <summary>Write audio enabled message and wait for Ack.</summary>
        /// <param name="activeSpeakerZone">The active speaker zone. see enum ActiveSpeakerZone</param>
        /// <returns>The <see cref="int"/>Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public bool WriteAudioEnabled(ActiveSpeakerZone activeSpeakerZone)
        {
            Logger.Info("WriteEnableAudio Zone={0}", activeSpeakerZone);
            var model = new PeripheralAudioEnable(activeSpeakerZone);
            bool ackReceived = false;
            this.Write(model, out ackReceived, true);
            Logger.Info("ACK={0} received for Enable Audio Speaker Zone:{1}", ackReceived, activeSpeakerZone);
            return ackReceived;
        }

        /// <summary>The write audio status interval.</summary>
        /// <param name="audioStatusDelay">The audio status delay.</param>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="Exception">Reading the Config fails, logs the error and re-throws the exception.</exception>
        public int WriteAudioStatusInterval(ushort audioStatusDelay)
        {
            Logger.Info("{0} Enter audioStatusDelay={1}", nameof(WriteAudioStatusInterval), audioStatusDelay);
            return this.WriteAudioConfig(PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile, audioStatusDelay);
        }

        /// <summary>Write a request to receive audio status from the peripheral</summary>
        /// <returns>The <see cref="int" />.</returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public int WriteAudioStausRequest()
        {
            Logger.Info("{0} Enter", nameof(WriteAudioStausRequest));
            return this.Write(new PeripheralPoll());
        }

        /// <summary>Write message to set the volume to the peripheral.</summary>
        /// <param name="interiorVolume">The interior Volume in the range min to max.</param>
        /// <param name="exteriorVolume">The exterior Volume in the range min to max.</param>
        /// <returns>The <see cref="int"/>Total Bytes written</returns>
        /// <exception cref="IOException">An I/O error occurs on the serial stream.</exception>
        public bool WriteSetVolume(byte interiorVolume, byte exteriorVolume)
        {
            Logger.Info("Setting new Volume levels Interior={0}, Exterior={1}", interiorVolume, exteriorVolume);
            bool ackReceived;
            this.Write(new PeripheralSetVolume(interiorVolume, exteriorVolume), out ackReceived, true);
            Logger.Info("ACK={0} received for Setting Volume", ackReceived);
            return ackReceived;
        }

        /// <exception cref="FileLoadException">Invalid image file, no records found</exception>
        public bool UpdateImage(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("ImageUpdate Failed, file not found", fileName);
            }

            throw new NotImplementedException("UpdateImage Common Soon");

            bool result = false;
            lock (UpdateImageLock)
            {
                // calculate total records which is each CRLF - Each text line is called a record.
                var totalRecords = GetUpdateImageTotalRecords(fileName, PeripheralImageUpdateType.IntelHex);
                if (totalRecords <= 0)
                {
                    throw new FileLoadException("Invalid image file, no records found", fileName);
                }

 
                try
                {
                    using (var stream = File.OpenText(fileName))
                    {
                        string s;
                        while ((s = stream.ReadLine()) != null)
                        {
                            var bytes = s.Length;
                            // TODO create a ImageRecord and write to the peripheral
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("UpdateImage() {0}", ex.Message);
                }
                finally
                {

                }
            }
          
         
            return result;
        }

        #endregion

        #region Methods

        /// <summary>Fire windows event of Audio status change</summary>
        /// <param name="audioStatusMessage"></param>
        private void FireAudioStatusChanged(AudioStatusMessage audioStatusMessage)
        {
            var handler = this.AudioStatusChanged;
            if (handler != null && audioStatusMessage != null)
            {
                Logger.Info("Fire Event {0}... {1}", nameof(FireAudioStatusChanged), audioStatusMessage);
                handler?.Invoke(this, audioStatusMessage);
            }
        }

        /// <summary>Fire the GPIO windows event changed</summary>
        /// <param name="sender"></param>
        /// <param name="peripheralGpioEventArg">Received Gpio status from the Peripheral</param>
        private void FirePeripheralGpioEvent(object sender, PeripheralGpioEventArg peripheralGpioEventArg)
        {
            var handler = this.GpioChanged;
            if (handler != null && peripheralGpioEventArg != null)
            {
                Logger.Info("Fire Event {0}... {1}", nameof(FirePeripheralGpioEvent), peripheralGpioEventArg);
                handler?.Invoke(sender, peripheralGpioEventArg);
            }
        }

        /// <summary>
        /// Handle the Audio Playback event. Enable the correct Line output so audio is heard or turn it off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="audioPlaybackEvent"></param>
        private void OnAudioHandlerPlaybackChangedEventHandler(object sender, AudioPlaybackEvent audioPlaybackEvent)
        {
            Task.Factory.StartNew(
                () =>
                    {
                        Logger.Info("Start Task to Set Audio Playback to {0}", audioPlaybackEvent.ToString());
                        WriteAudioZonesOutput(audioPlaybackEvent);
                    });
        }

        /// <summary>
        /// Write the Audio Zone to the client to enable or disable output
        /// </summary>
        /// <param name="audioPlaybackEvent">Audio Zone to use, see enum</param>
        private bool  WriteAudioZonesOutput(AudioPlaybackEvent audioPlaybackEvent)
        {
            if (audioPlaybackEvent == null)
                return false;

            Logger.Info("{0} Enter AudioPlaybackEvent={1}", nameof(this.OnAudioHandlerPlaybackChangedEventHandler), audioPlaybackEvent);
            
            ActiveSpeakerZone audioZone;
            int value = (int)audioPlaybackEvent.AudioZone;
            bool ackReceived = false;
            if (Enum.TryParse(value.ToString(), out audioZone))
            {
                Logger.Info("{0} set ActiveSpeaker Zone = {1}", nameof(this.OnAudioHandlerPlaybackChangedEventHandler), audioZone);
                ackReceived = this.WriteAudioEnabled(audioZone);
            }
            else
            {
                Logger.Warn("Undefined Audio Zone {0} Ignored!", audioPlaybackEvent.AudioZone);
            }
            return ackReceived;
        }

        /// <summary>Handle the windows event for Audio status changed</summary>
        /// <param name="sender"></param>
        /// <param name="audioStatusMessage"></param>
        private void OnAudioHandlerStatusMessageEventHandler(object sender, AudioStatusMessage audioStatusMessage)
        {
            Logger.Info("{0} Enter AudioStatusMessage = {1}", nameof(this.OnAudioHandlerStatusMessageEventHandler), audioStatusMessage);
            FireAudioStatusChanged(audioStatusMessage);
        }

        /// <summary>Write a request to the hardware for audio status</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAudioHandlerStatusRequestedEventHandler(object sender, EventArgs args)
        {
            try
            {
                if (this.IsPeripheralConnected)
                {
                    this.audioStatusRequestedCount++;
                    Logger.Info(
                        "{0} Enter audioStatusRequestedCount = {1}",
                        nameof(this.OnAudioHandlerStatusRequestedEventHandler),
                        this.audioStatusRequestedCount);
                    this.WriteAudioStausRequest();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to request Audio Status {0}", ex.Message);
            }
        }

        /// <summary>Event handler for GIPO changes, Forward it on</summary>
        /// <param name="sender"></param>
        /// <param name="peripheralGpioEventArg"></param>
        private void OnAudioHandlerGpioChangedEventHandler(object sender, PeripheralGpioEventArg peripheralGpioEventArg)
        {
            this.FirePeripheralGpioEvent(sender, peripheralGpioEventArg);
        }

        /// <summary>Subscribe to windows events from the Audio Handler</summary>
        private void SubscribeAudioSwitchEvents()
        {
            // Subscribe to the Handler to process events and forward I/O with the peripheral client
            var audioSwitchHandler = (AudioSwitchHandler)this.PeripheralHandler;
            audioSwitchHandler.GpioChanged += this.OnAudioHandlerGpioChangedEventHandler;
            audioSwitchHandler.AudioStatusMessageEvent += this.OnAudioHandlerStatusMessageEventHandler;
            audioSwitchHandler.AudioStatusRequested += this.OnAudioHandlerStatusRequestedEventHandler;
            audioSwitchHandler.AudioPlaybackChangedEvent += this.OnAudioHandlerPlaybackChangedEventHandler;
            audioSwitchHandler.VolumeChangeRequestedEvent += this.OnAudioHandlerVolumeChangeRequestEventHandler;

            // when we configure the peripheral we will get it's version which once received we will consider it configured
            audioSwitchHandler.PeripherialVersionChangedEvent += (sender, versionInfo) =>
                {
                    Logger.Info("Peripheral Version Changed {0}", versionInfo.ToString());
                    this.IsVersionInfoReceived = true;
                    this.PeripheralVersionInfo = versionInfo;
                };
        }

        /// <summary>
        /// Handle changes to the current volume levels set on the audio switch and/or enable/disable the audio refresh interval for Audio Status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="volumeChangeRequest"></param>
        private void OnAudioHandlerVolumeChangeRequestEventHandler(object sender, VolumeChangeRequest volumeChangeRequest)
        {
            try
            {
                Logger.Info("{0} Enter VolumeChangeMessageRequest = {1}", nameof(this.OnAudioHandlerVolumeChangeRequestEventHandler), volumeChangeRequest);
                if (this.IsPeripheralConnected)
                {
                    if (volumeChangeRequest.AudioRefreshInterval != VolumeSettingsMessage.Ignored)
                    {
                        Logger.Info(
                            "{0} Enter message is VolumeChangeMessageRequest  Setting Audio Refresh Interval = {1}",
                            nameof(this.OnAudioHandlerVolumeChangeRequestEventHandler),
                            volumeChangeRequest.AudioRefreshInterval);

                        // read in the existing Peripheral config settings/use from cache and only change the audio refresh interval
                        var config = PeripheralAudioConfig.ReadPeripheralAudioConfig();
                        // this setting controls how often the audio switch hardware sends Audio Status on the RS485, time = 0 to disable, >= 1000 time in miliseconds
                        config.AudioStatusDelay = volumeChangeRequest.AudioRefreshInterval;
                        this.WriteAudioConfig(config);
                    }
                    
                    if (volumeChangeRequest.InteriorVolume!=VolumeSettingsMessage.Ignored && volumeChangeRequest.ExteriorVolume != VolumeSettingsMessage.Ignored)
                    {
                        // the the audio hardware to use new volume settings
                        this.WriteSetVolume(volumeChangeRequest.InteriorVolume, volumeChangeRequest.ExteriorVolume);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to change the Audio Volumes requested {0}", ex.Message);
            }
        }

        #endregion
    }
}