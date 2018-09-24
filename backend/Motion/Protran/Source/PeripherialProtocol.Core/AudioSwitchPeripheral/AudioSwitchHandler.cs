// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchHandler.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using NLog;

    // Class to handle the hardware Audio Switch exchange custom needs.
    // Exchange Medi messages between the other external Media enabled applications.

    /// <summary>The audio switch handler.</summary>
    public sealed class AudioSwitchHandler : PeripheralHandler, IAudioSwitchHandler
    {
        #region Constants

        private const string VolumeSettingsFile = "VolumeSettings.xml";

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly object VolumeSettingsLock = new object();

        private static PeripheralAudioConfig peripheralAudioConfig;

        #endregion

        #region Fields

        private readonly ConcurrentQueue<AudioStatusMessage> audioStatusQue = new ConcurrentQueue<AudioStatusMessage>();

        private readonly AutoResetEvent audioStatusReceived = new AutoResetEvent(false);

        private readonly ManualResetEvent endEvent = new ManualResetEvent(false);

        private readonly ConcurrentQueue<PeripheralAudioGpioStatus> gpioStatusQue = new ConcurrentQueue<PeripheralAudioGpioStatus>();

        private Task audioStatusTask;

        private AudioStatusMessage lastAudioStatusMessage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AudioSwitchHandler" /> class.
        ///     Prevents a default instance of the <see cref="AudioSwitchHandler" /> class from being created. Initializes a
        ///     new instance of the <see cref="AudioSwitchHandler" /> class.
        /// </summary>
        public AudioSwitchHandler()
            : this(null)
        {
            this.CreateAudioStatusTask();
        }

        /// <summary>Initializes a new instance of the <see cref="AudioSwitchHandler"/> class.</summary>
        /// <param name="context">The context.</param>
        public AudioSwitchHandler(PeripheralContext context)
            : base(context)
        {
            // subscribe to medi message to be handle by this class
            // respond when asked for Audio Status or VolumeSettings
            var mediFileName = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            if (File.Exists("medi.cofig"))
            {
                this.InitMedi(mediFileName);
            }
            else if (File.Exists(mediFileName))
            {
                this.InitMedi(mediFileName);
            }
            else
            {
                Logger.Warn("Ignore Medi Subscription medi.Config not found!");
            }

            this.CreateAudioStatusTask();
        }

        #endregion

        #region Public Events

        /// <summary>The event to notify of changed audio playback used to control audio line outputs.</summary>
        public event EventHandler<AudioPlaybackEvent> AudioPlaybackChangedEvent;

        /// <summary>The audio status changed event.</summary>
        public event EventHandler<AudioStatusMessage> AudioStatusMessageEvent;

        /// <summary>Handle the medi audio status request message</summary>
        public event EventHandler AudioStatusRequested;

        /// <summary>Peripheral GPIO changed event</summary>
        public event EventHandler<PeripheralGpioEventArg> GpioChanged;

        /// <summary>The volume change requested event.</summary>
        public event EventHandler<VolumeChangeRequest> VolumeChangeRequestedEvent;

        /// <summary>The volume settings message event for setting the min, max, default volume settings .</summary>
        public event EventHandler<VolumeSettingsMessage> VolumeSettingsChangedEvent;

        #endregion

        #region Properties

        private byte? LastExteriorVolume { get; set; }

        private byte? LastInteriorVolume { get; set; }

        private string PeripheralHardwareVersion
        {
            get
            {
                var peripheralVersionsInfo = this.PeripheralContext.PeripheralVersions;
                return this.PeripheralContext != null && peripheralVersionsInfo != null ? peripheralVersionsInfo.HardwareVersionText : string.Empty;
            }
        }

        private string PeripheralSerialNumber
        {
            get
            {
                var peripheralVersionsInfo = this.PeripheralContext.PeripheralVersions;
                return this.PeripheralContext != null && peripheralVersionsInfo != null ? peripheralVersionsInfo.SerialNumberText : string.Empty;
            }
        }

        private string PeripheralSoftwareVersion
        {
            get
            {
                var peripheralVersionsInfo = this.PeripheralContext.PeripheralVersions;
                return this.PeripheralContext != null && peripheralVersionsInfo != null ? peripheralVersionsInfo.SoftwareVersionText : string.Empty;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Create the PeripheralGpioEventArg using the current Peripheral Audio Gpio Status</summary>
        /// <param name="peripheralAudioGpioStatus">Current status</param>
        /// <returns>The <see cref="PeripheralGpioEventArg"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="peripheralAudioGpioStatus"/> is <see langword="null"/>.</exception>
        public static PeripheralGpioEventArg CreatePeripheralGpioEventArg(PeripheralAudioGpioStatus peripheralAudioGpioStatus)
        {
            // keep a copy of the read audio config to have access to the Pin Meanings we used. 
            if (peripheralAudioConfig == null)
            {
                peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig();
            }

            return new PeripheralGpioEventArg(peripheralAudioGpioStatus, peripheralAudioConfig);
        }

        /// <summary>The find peripheral message model.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        /// <exception cref="NotSupportedException">Stream does not support Length property(SerialPort.BaseStream) ie SerialPort
        ///     Stream</exception>
        /// <exception cref="ApplicationException">Failed to find the FromBytes method in assembly!!!</exception>
        public static IPeripheralBaseMessage FindPeripheralMessageModel(byte[] bytes)
        {
            using (var audioSwitchHandler = new AudioSwitchHandler(new PeripheralContext(new PeripheralConfig(), new MemoryStream(bytes))))
            {
                return audioSwitchHandler.Read(audioSwitchHandler.ContextStream) as IPeripheralBaseMessage;
            }
        }

        /// <summary>The find peripheral message type.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="Type"/>.</returns>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        /// <exception cref="FileNotFoundException">If the file is not found</exception>
        /// <exception cref="ConfiguratorException">If errors occurred while deserializing the file</exception>
        public static Type FindPeripheralMessageType(byte[] bytes)
        {
            // read past the framing byte if present in the array
            var audioSwitchHandler = new AudioSwitchHandler(new PeripheralContext(new PeripheralConfig(), new MemoryStream(bytes)));
            var model = audioSwitchHandler.Read();
            if (model == null)
            {
                Logger.Warn("FindPeripheralMessageType() unknown Message type from bytes");
                return null;
            }

            Type messageType = model.GetType(); // class or object type expected to be our known models.
            return messageType;
        }

        /// <summary>Broadcast via Medi an audio status change has occurred as well as the to an optional event handler.</summary>
        /// <param name="audioStatusMessage">The hardware audio status message.</param>
        public void BroadcastAudioStatusChanged(AudioStatusMessage audioStatusMessage)
        {
            this.audioStatusQue.Enqueue(audioStatusMessage);
            this.audioStatusReceived.Set();
        }

        /// <summary>Broadcast volume settings changed over medi and windows event.</summary>
        /// <param name="volumeSettingsMessage">The volume settings.</param>
        public void BroadcastVolumeSettingsResponse(VolumeSettingsMessage volumeSettingsMessage)
        {
            Info("{0} - Medi Broadcast of VolumeSettings", nameof(BroadcastVolumeSettingsResponse));
            volumeSettingsMessage.MessageAction = MessageActions.OK;
            MessageDispatcher.Instance.Broadcast(volumeSettingsMessage);
            this.FireVolumeSettingsChanged(volumeSettingsMessage);
        }

        /// <summary>The dispose.</summary>
        /// <param name="disposing">The disposing.</param>
        public override void Dispose(bool disposing)
        {
            this.endEvent.Set();
            base.Dispose(disposing);
        }

        /// <summary>Fire volume settings changed.</summary>
        /// <param name="volumeSettingsMessage">The volume settings message.</param>
        public void FireVolumeSettingsChanged(VolumeSettingsMessage volumeSettingsMessage)
        {
            var handler = this.VolumeSettingsChangedEvent;
            try
            {
                if (handler != null)
                {
                    if (volumeSettingsMessage.MessageAction == MessageActions.Set)
                    {
                        Warn("Request made to Update and set new Volume settings {0}", volumeSettingsMessage);
                    }

                    // send event to AudioSwitchClient to handle
                    Info("{0} - Fire VolumeSettingsChangedEvent", nameof(FireVolumeSettingsChanged));
                    handler.Invoke(this, volumeSettingsMessage);
                }
            }
            catch
            {
            }
        }

        /// <summary>The process received messages.</summary>
        /// <param name="state">The state.</param>
        /// <param name="message">The message.</param>
        /// <returns>The <see cref="IPeripheralBaseMessage"/>.</returns>
        public override IPeripheralBaseMessage ProcessReceivedMessages(PeripheralState state, object message)
        {
            var bytes = message.ToBytes();
            var validChecksum = CheckSumUtil.IsValidChecksum(bytes);
            var baseMessage = message as IPeripheralBaseMessage;
            if (baseMessage != null)
            {
                Info("{0} MessageType = {1}", nameof(ProcessReceivedMessages), baseMessage.Header.MessageType);

                switch (baseMessage.Header.MessageType)
                {
                    case PeripheralMessageType.AudioVersionResponse:
                        this.WriteResponse(validChecksum);
                        var peripheralAudioVersions = baseMessage as PeripheralAudioVersions;
                        if (validChecksum && peripheralAudioVersions != null)
                        {
                            // save off the version info if we get it
                            var fileName = PathManager.Instance.CreatePath(FileType.Data, "AudioPeripheralVersion.xml");
                            this.SaveVersionInfo(peripheralAudioVersions.VersionsInfo, fileName);
                        }
                        break;

                    case PeripheralMessageType.GpioStatusResponse:
                        var peripheralAudioGpioStatus = baseMessage as PeripheralAudioGpioStatus;
                        if (peripheralAudioGpioStatus != null)
                        {
                            // if GPIO status or Audio Status received relay this out as a Medi Message
                            this.FirePeripheralGpioEvent(this, peripheralAudioGpioStatus);
                        }
                        break;

                    case PeripheralMessageType.AudioStatusResponse:
                        var peripheralAudioStatus = baseMessage as PeripheralAudioStatus;
                        if (peripheralAudioStatus != null)
                        {
                            // create a audio status message to be sent over medi 
                            var audioStatusMessage = peripheralAudioStatus.ToAudioStatusMessage(
                                this.PeripheralHardwareVersion,
                                this.PeripheralSoftwareVersion,
                                this.PeripheralSerialNumber);

                            // save off the current volume levels we received
                            this.LastInteriorVolume = peripheralAudioStatus.InteriorVolume;
                            this.LastExteriorVolume = peripheralAudioStatus.ExteriorVolume;

                            // share the changes
                            this.BroadcastAudioStatusChanged(audioStatusMessage);
                        }
                        break;
                }
            }
            else
            {
                Info("{0} NO Message Found Nak Client", nameof(ProcessReceivedMessages));
                this.WriteNak();
            }

            return baseMessage;
        }
   
        #endregion

        #region Methods

        /// <summary>Initialize Medi subscriptions to our custom messages</summary>
        /// <param name="mediFileName">The medi File Name.</param>
        protected override void InitMedi(string mediFileName = "medi.config")
        {
            if (!this.IsMediInitialized)
            {
                if (File.Exists(mediFileName))
                {
                    try
                    {
                        // use the same mechanics as the host apps so we can run them standalone and debug messages to/from them
                        // var configFileName = PathManager.Instance.GetPath(FileType.Config, mediFileName);
                        this.fileConfigurator = new FileConfigurator(mediFileName, Environment.MachineName, Environment.MachineName);

                        var localAddress = MessageDispatcher.Instance.LocalAddress;
                        if (MessageDispatcher.Instance.IsValidLocalAddress == false)
                        {
                            MessageDispatcher.Instance.Configure(this.fileConfigurator);
                        }

                        // Subscribe to the Medi message we will process for Audio command and control
                        this.SubscribeAudioStatusRequest(this.AudioStatusRequestedMediHandler);

                        // Audio plaback, we want to enable/disable the speaker line outs on the audio hardware
                        this.SubscribeAudioPlaybackEvents(this.AudioPlaybackMediHandler);

                        // Respond to change in the current Volume levels or to enable the Audio Poll refresh interval
                        this.SubscribeVolumeChangeRequest(this.VolumeChangeRequestMediHandler);

                        // medi message to return the current volume settings to the Ximple TCP Clients (LAM/Prosys)
                        this.SubscribeVolumeSettingsRequestMessage(this.VolumeSettingsMessageRequestMediHandler);

                        // meid message to change and persist the Volume settings, min, max and default volumes
                        this.SubscribeVolumeSettingsMessage(this.VolumeSettingsMessageMediHandler);

                        this.IsMediInitialized = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed to Init Medi {0}", ex.Message);
                    }
                }
                else
                {
                    Logger.Warn("Ignored InitMedi File Not found {0}", mediFileName);
                }
            }
        }

        /// <summary>The un-initialize medi.</summary>
        protected override void UnInitMedi()
        {
            if (this.IsMediInitialized)
            {
                Info("UnInitMedi() UnSubscribe Medi Messages");
                this.UnSubscribeAudioStatusRequest(this.AudioStatusRequestedMediHandler);
                this.UnSubscribeAudioPlaybackEvent(this.AudioPlaybackMediHandler);
                this.UnSubscribeVolumeChnageRequest(this.VolumeChangeRequestMediHandler);
                this.UnSubscribeVolumeSettingsRequestMessage(this.VolumeSettingsMessageRequestMediHandler);
                this.UnSubscribeVolumeSettingsMessage(this.VolumeSettingsMessageMediHandler);
            }
        }

        private static void Trace(string format, params object[] args)
        {
            Logger.Info(format, args);
            Debug.WriteLine(format, args);
        }

        /// <summary>Handle the Medi message for AudioPlaybackEvent received, forward as Windows event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioPlaybackMediHandler(object sender, MessageEventArgs<AudioPlaybackEvent> e)
        {
            Info("{0} Enter", nameof(this.AudioPlaybackMediHandler));
            this.FireAudioPlaybackChanged(e.Message);
        }

        private void AudioStatusChangedTaskHandler(AudioStatusMessage audioStatusMessage)
        {
            Info("{0} Enter AudioStatusMessage={1}", nameof(this.BroadcastAudioStatusChanged), audioStatusMessage);
            if (audioStatusMessage != null)
            {
                // save the last status
                this.lastAudioStatusMessage = audioStatusMessage;

                try
                {
                    if (this.IsMediInitialized)
                    {
                        MessageDispatcher.Instance.Broadcast(audioStatusMessage);
                    }

                    var handler = this.AudioStatusMessageEvent;
                    if (handler != null)
                    {
                        Info("{0} Enter Fire AudioStatusMessageEvent", nameof(this.BroadcastAudioStatusChanged));
                        handler(this, audioStatusMessage);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>Handle the request to get and return Audio Status</summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        private void AudioStatusRequestedMediHandler(object sender, MessageEventArgs<AudioStatusRequest> messageEventArgs)
        {
            Logger.Info("{0} Enter", nameof(VolumeSettingsMessage));
            this.FireAudioStatusRequested(this, messageEventArgs.Message);
        }

        /// <summary>
        /// Create the Audio Status task to process the que of received Audio Status message
        /// </summary>
        private void CreateAudioStatusTask()
        {
            if (this.audioStatusTask == null)
            {
                this.audioStatusTask = Task.Factory.StartNew(
                    () =>
                        {
                            Thread.CurrentThread.Name = "AudioStatusQueTask";
                            var handles = new WaitHandle[] { this.audioStatusReceived, this.endEvent };
                            while (!this.disposed)
                            {
                                if (WaitHandle.WaitAny(handles) != 0)
                                {
                                    break;
                                }

                                AudioStatusMessage status;
                                if (this.audioStatusQue.TryDequeue(out status))
                                {
                                    this.AudioStatusChangedTaskHandler(status);
                                }
                            }
                        });
            }
        }

        /// <summary>Fire a windows event that Audio playback has changed</summary>
        /// <param name="audioPlaybackEvent"></param>
        private void FireAudioPlaybackChanged(AudioPlaybackEvent audioPlaybackEvent)
        {
            // Fire event to be handled by the Serial Client, send commands to peripheral to execute 
            Info("{0} Enter", nameof(FireAudioPlaybackChanged));
            if (audioPlaybackEvent != null)
            {
                // send event to AudioSwitchClient to handle
                var handler = this.AudioPlaybackChangedEvent;
                handler?.Invoke(this, audioPlaybackEvent);
            }
        }

        /// <summary>Handle request to get the current the Volume settings. Read from config file and Broadcast</summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        /// <summary>Fire windows event to request the client to ask for Audio Status</summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        private void FireAudioStatusRequested(object sender, AudioStatusRequest messageEventArgs)
        {
            // send event to AudioSwitchClient to handle
            Trace("FireAudioStatusRequested Enter");
            var handler = this.AudioStatusRequested;
            handler?.Invoke(this, new EventArgs());
        }

        /// <summary>Fire the GPIO event changed to a client a</summary>
        /// <param name="sender"></param>
        /// <param name="peripheralAudioGpioStatus">Received Gpio status from the Peripheral</param>
        private void FirePeripheralGpioEvent(object sender, PeripheralAudioGpioStatus peripheralAudioGpioStatus)
        {
            Task.Run(
                () =>
                    {
                        var handler = this.GpioChanged;
                        if (handler != null)
                        {
                            // send event to AudioSwitchClient to handle
                            var peripheralGpioEventArg = CreatePeripheralGpioEventArg(peripheralAudioGpioStatus);

                            Info("FireGpioEvent() Enter");
                            handler(sender, peripheralGpioEventArg);
                        }
                        else
                        {
                            Info("No Event! subscribe to GpioChanged");
                        }
                    });
        }

        /// <summary>Fire the current Volume level for interior and exterior changed request event.
        ///     The serial client would process this event and send the change over the serial protocol to the peripheral  device</summary>
        /// <param name="volumeSettingsRequest"></param>
        private void FireVolumeChangeRequestedEvent(VolumeChangeRequest volumeSettingsRequest)
        {
            Info("Fire event for Volume Change Request {0}", volumeSettingsRequest);

            // Save the last vol level when setting as well as on a received Audio Status from a POLL operation      
            if (volumeSettingsRequest.InteriorVolume != VolumeSettingsMessage.Ignored)
            {
                this.LastInteriorVolume = volumeSettingsRequest.InteriorVolume;
            }

            if (volumeSettingsRequest.ExteriorVolume != VolumeSettingsMessage.Ignored)
            {
                this.LastExteriorVolume = volumeSettingsRequest.ExteriorVolume;
            }

            var handler = this.VolumeChangeRequestedEvent;
            handler?.Invoke(this, volumeSettingsRequest);
        }

        /// <summary>Handle the Audio playback medi message</summary>
        /// <param name="handler"></param>
        private void SubscribeAudioPlaybackEvents(EventHandler<MessageEventArgs<AudioPlaybackEvent>> handler)
        {
            Info("{0} Enter - Subscribe to Medi message for {1}", nameof(SubscribeAudioPlaybackEvents), nameof(AudioPlaybackEvent));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeAudioPlaybackEvents received null handler object!");
            }

            MessageDispatcher.Instance.Subscribe(handler);
        }

        /// <summary>Subscribe to audio status request over medi and setup the handler.</summary>
        /// <param name="handler">The handler.</param>
        private void SubscribeAudioStatusRequest(EventHandler<MessageEventArgs<AudioStatusRequest>> handler)
        {
            Info("{0} Enter Subscribe to Medi message for {1}", nameof(SubscribeAudioStatusRequest), nameof(AudioStatusRequest));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeAudioStatusRequest received null handler object!");
            }

            // subscribe to request for Audio Status as Ximple messages from Protran to send this request via medi back filled in
            // On request we need to reply with a Medi message see Broadcast see class AudioStatusMessage     
            MessageDispatcher.Instance.Subscribe(handler);
        }

        /// <summary>Handle the Volume Change request. Change the current interior or exterior volumes on the audio switch Or
        ///     Enable/Disable the Audio refresh interval(time in milliseconds)</summary>
        /// <param name="handler"></param>
        private void SubscribeVolumeChangeRequest(EventHandler<MessageEventArgs<VolumeChangeRequest>> handler)
        {
            Info("{0} Enter - Subscribe to Medi message for {1}", nameof(SubscribeVolumeChangeRequest), nameof(VolumeChangeRequest));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeVolumeChangeRequest received null handler object!");
            }

            MessageDispatcher.Instance.Subscribe(handler);
        }

        private void SubscribeVolumeSettingsMessage(EventHandler<MessageEventArgs<VolumeSettingsMessage>> handler)
        {
            Info("{0} Enter - Subscribe to Medi message for {1}", nameof(SubscribeVolumeSettingsMessage), nameof(VolumeSettingsMessage));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeVolumeSettingsMessage received null handler object!");
            }

            MessageDispatcher.Instance.Subscribe(handler);
        }

        private void SubscribeVolumeSettingsRequestMessage(EventHandler<MessageEventArgs<VolumeSettingsRequestMessage>> handler)
        {
            Info(
                "{0} Enter Subscribe to Medi message for {1}",
                nameof(this.SubscribeVolumeSettingsRequestMessage),
                nameof(VolumeSettingsRequestMessage));
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "SubscribeVolumeSettingsMessage received null handler object!");
            }

            // subscribe to request to set volume settings
            MessageDispatcher.Instance.Subscribe(handler);
        }

        private void UnSubscribeAudioPlaybackEvent(EventHandler<MessageEventArgs<AudioPlaybackEvent>> handler)
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        /// <summary>The unsubscribe from the medi audio status request handler.</summary>
        /// <param name="handler">The handler.</param>
        private void UnSubscribeAudioStatusRequest(EventHandler<MessageEventArgs<AudioStatusRequest>> handler)
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        private void UnSubscribeVolumeChnageRequest(EventHandler<MessageEventArgs<VolumeChangeRequest>> handler)
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        private void UnSubscribeVolumeSettingsMessage(EventHandler<MessageEventArgs<VolumeSettingsMessage>> handler)
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        private void UnSubscribeVolumeSettingsRequestMessage(EventHandler<MessageEventArgs<VolumeSettingsRequestMessage>> handler)
        {
            MessageDispatcher.Instance.Unsubscribe(handler);
        }

        /// <summary>Handle medi message to change the volume levels for interior, exterior Or we are changing the audio poll refresh
        ///     interval on the hardware</summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        private void VolumeChangeRequestMediHandler(object sender, MessageEventArgs<VolumeChangeRequest> messageEventArgs)
        {
            this.FireVolumeChangeRequestedEvent(messageEventArgs.Message);
        }

        /// <summary>Save the volume settings defaults min, max and default Volumes to the peripheral config xml file.
        ///     If provided with the current volume interior and exterior volume level adjust the hardware with the new values.
        ///     Note this setting is not persisted unlike the default volume settings.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeSettingsMessageMediHandler(object sender, MessageEventArgs<VolumeSettingsMessage> e)
        {
            var volumeSettingsMessage = e.Message;
            if (volumeSettingsMessage != null && volumeSettingsMessage.MessageAction == MessageActions.Set)
            {
                // save the Audio switch new volume settings for volumes: min, max, default volume
                PeripheralAudioConfig.WritePeripheralAudioConfigVolumes(volumeSettingsMessage);

                var currentInteriorVolume = e.Message.Interior?.CurrentVolume ?? VolumeSettingsMessage.Ignored;
                var currentExteriorVolume = e.Message.Exterior?.CurrentVolume ?? VolumeSettingsMessage.Ignored;
                if (currentInteriorVolume != VolumeSettingsMessage.Ignored
                    && currentExteriorVolume != VolumeSettingsMessage.Ignored)
                {
                    // fire event to have the Audio switch serial client now apply the new current volume levels.
                    this.FireVolumeChangeRequestedEvent(
                        new VolumeChangeRequest(currentInteriorVolume, currentInteriorVolume));
                }
                else
                {
                    Info("Ignoring Current Interior or Exterior Volume Settings, Not changed");
                }
            }
        }

        /// <summary>Handle the medi message requesting the Volume Settings. We are not making changes just replying with the current
        ///     config settings.</summary>
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>
        private void VolumeSettingsMessageRequestMediHandler(object sender, MessageEventArgs<VolumeSettingsRequestMessage> messageEventArgs)
        {
            Info("VolumeSettingsMessageRequestMediHandler received request");

            /* Goal is to respond and give the Ximple client this dictionary values
             *They will send a request as Table 100 w/o any cells (empty)
             * 
             *    <Table Index="100" Name="InfoTainmentVolumeSettings" MultiLanguage="false" MultiRow="false">
                  <Description>InfoTainment Volume Settings used in the Audio Hardware switch and returned to external equipment InfoTainment</Description>
                  <Column Index="0" Name="AudioStatusRefreshInterval" Description="The auto refresh interval for the audio hardware switch to send Audio Status over the serial port. Zero to disable, default 1000 ms" />
                  <Column Index="1" Name="InteriorMaximumVolume" Description="Interior Maximum Audio level setting" />
                  <Column Index="2" Name="InteriorMinimumVolume" Description="Interior Minimum Audio level setting" />
                  <Column Index="3" Name="InteriorDefaultVolume" Description="Interior Default Audio level setting" />
                  <Column Index="4" Name="InteriorCurrentVolume" Description="Interior Current Audio level setting" />
                  <Column Index="5" Name="ExteriorMaximumVolume" Description="Exterior Maximum Audio level setting" />
                  <Column Index="6" Name="ExteriorMinimumVolume" Description="Exterior Minimum Audio level setting" />
                  <Column Index="7" Name="ExteriorDefaultVolume" Description="Exterior Default Audio level setting" />
                  <Column Index="8" Name="ExteriorCurrentVolume" Description="Exterior Current Audio level setting" />
                </Table>
             */

            // read the existing file content and send out as medi message to be further distributed
            var config = PeripheralAudioConfig.ReadPeripheralAudioConfig();
            var volumeSettingsMessage = new VolumeSettingsMessage
            {
                MessageAction = MessageActions.OK,
                Interior =
                                                    new VolumeSettings
                                                    {
                                                        DefaultVolume = config.InteriorDefaultVolume,
                                                        MinimumVolume = config.InteriorMinVolume,
                                                        MaximumVolume = config.InteriorMaxVolume,
                                                        CurrentVolume =
                                                                this.LastInteriorVolume
                                                                ?? config.InteriorDefaultVolume
                                                    },
                Exterior =
                                                    new VolumeSettings
                                                    {
                                                        CurrentVolume = config.ExteriorDefaultVolume,
                                                        DefaultVolume = config.ExteriorDefaultVolume,
                                                        MinimumVolume = config.ExteriorMinVolume,
                                                        MaximumVolume = config.ExteriorMaxVolume
                                                    }
            };

            // if we have audio status volume levels from a real time status message to us then use it here
            volumeSettingsMessage.Interior.CurrentVolume = this.LastInteriorVolume ?? config.ExteriorDefaultVolume;
            volumeSettingsMessage.Exterior.CurrentVolume = this.LastExteriorVolume ?? config.ExteriorDefaultVolume;
            volumeSettingsMessage.RefreshIntervalMiliSeconds = config.AudioStatusDelay;

            // return the request volume settings 
            MessageDispatcher.Instance.Broadcast(volumeSettingsMessage);
        }

        #endregion
    }
}