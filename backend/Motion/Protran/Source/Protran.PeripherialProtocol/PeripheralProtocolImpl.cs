// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralProtocolImpl.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.PeripheralProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Protran.Core.Protocols;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    enum AudioConfigStates { RequestVersion, WriteConfig, WaitVersion, WriteVolumes, ConfigCompleted };

    /// <summary>The peripheral protocol implementation.</summary>
    public class PeripheralProtocolImpl : IProtocol, IManageableObject
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        ///     Event used to manage the running status of this protocol.
        /// </summary>
        private readonly ManualResetEvent protocolRunningEvent = new ManualResetEvent(false);

        private bool configurationCompleted = false;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Flag set false after we get the first GPIO received
        /// </summary>
        private bool firstGpioReceived = false;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralProtocolImpl" /> class.</summary>
        public PeripheralProtocolImpl()
        {
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);
#if DEBUG
            //     Debugger.Launch();Tas
#endif
        }

        #endregion

        #region Public Events

        /// <summary>The started.</summary>
        public event EventHandler Started;

        public EventHandler<EventArgs> AudioSwitchConfigurationCompleted;

        #endregion

        #region Public Properties

        /// <summary>Gets the audio switch serial client.</summary>
        public AudioSwitchSerialClient AudioSwitchSerialClient { get; private set; }

        public bool IgnoreFirstGpioEvent { get; private set; }

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     Gets the host.
        /// </summary>
        public IProtocolHost Host { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether
        ///     this channel is opened or not.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                var isOpen = this.AudioSwitchSerialClient?.IsComPortOpen ?? false;
                Logger.Debug("IsOpen = {0}", isOpen);
                return isOpen;
            }

            private set
            {
            }
        }

        /// <summary>Gets the name.</summary>
        public string Name
        {
            get
            {
                return "PeripheralProtocolImpl";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The configure.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
            if (this.Dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary), "Ximple Dictionary is undefined");
            }
        }

        /// <summary>Create Ximple for GPIO events from the Audio Mux</summary>
        /// <param name="peripheralGpioEventArg">The Source Event args to use</param>
        /// <returns>The <see cref="Ximple"/>.</returns>
        public Ximple CreateGipoXimple(PeripheralGpioEventArg peripheralGpioEventArg)
        {
            Ximple ximple = null;
            var table = this.Dictionary?.FindXimpleTable(XimpleConfig.InfoTainmentSystemStatusTableIndexDefault);
            if (table != null)
            {
                ximple = new Ximple("2.0.0");
                foreach (var gpioInfo in peripheralGpioEventArg.GpioInfo)
                {
                    var column = table.FirstColumn(gpioInfo.Gpio.ToString());
                    if (column != null)
                    {
                        ximple.Cells.Add(this.Dictionary.CreateXimpleCell(table, gpioInfo.Gpio.ToString(), gpioInfo.Active));
                    }
                }
            }
            else
            {
                Logger.Warn("Failed to find Table for GPIO! Check Dictionary.xml is present and contains table=105");
            }

            return ximple;
        }

        /// <summary>The get children.</summary>
        /// <param name="parent">The parent.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        /// <summary>The get properties.</summary>
        /// <returns>The <see cref="IEnumerable" />.</returns>
        public IEnumerable<ManagementProperty> GetProperties()
        {
            yield return new ManagementProperty<bool>("Channel open", this.IsOpen, true);
        }

        /// <summary>The run called on protocol start.</summary>
        /// <param name="host">The host to run.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has already been disposed. </exception>
        /// <exception cref="InvalidOperationException">The current instance is a transparent proxy for a <see cref="T:System.Threading.WaitHandle" /> in another application domain.</exception>
        /// <exception cref="AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
        public void Run(IProtocolHost host)
        {
            this.Host = host;
            try
            {
                if (this.InitializeAudioSwitchClient())
                {
                    Logger.Info("PeripheralProtocol Started Successfully");
                    this.AudioSwitchConfigurationCompleted += (sender, args) =>
                        {
                            // tell the host the protocol has fully started
                            this.RaiseStarted(EventArgs.Empty);
                        };
                }
                else
                {
                    // Signal started on failed init case here
                    this.RaiseStarted(EventArgs.Empty);
                }

                // Block this thread
                this.protocolRunningEvent.WaitOne();
            }
            catch (IOException ioException)
            {
                Logger.Warn("PeripheralProtocol will not be available {0}", ioException.Message);
            }
        }

        /// <summary>The stop.</summary>
        public void Stop()
        {
            lock (this)
            {
                Logger.Info("Stop Enter");
                this.cancellationTokenSource?.Cancel();
                this.CloseAll();
                this.protocolRunningEvent.Set();
            }
        }

        #endregion

        #region Methods

        /// <summary>Handle Gpio Changes from hardware raising a Ximple message to the host</summary>
        /// <param name="sender"></param>
        /// <param name="peripheralGpioEventArg"></param>
        private void AudioSwitchSerialClientOnGpioChanged(object sender, PeripheralGpioEventArg peripheralGpioEventArg)
        {
            try
            {
                Logger.Info("{0} Enter GpioEvent = {1}", nameof(this.AudioSwitchSerialClientOnGpioChanged), peripheralGpioEventArg);
                var ximple = this.CreateGipoXimple(peripheralGpioEventArg);
                if (ximple != null)
                {
                    if (this.IgnoreFirstGpioEvent && this.firstGpioReceived == false)
                    {
                        this.firstGpioReceived = true;
                        Logger.Warn("*** Ignored sending first GPIO Changed as Ximple message, normal operations as Protran starts ***");
                    }
                    else
                    {
                        // Raise a Ximple message to the Host ie Composer to be used in the presentation layer
                        this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                    }

                    // medi message for Protran to send out as Ximple to connected socket clients
                    var gpioState = this.CreateCreateGpioState(peripheralGpioEventArg);
                    MessageDispatcher.Instance.Broadcast(gpioState);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("!Failed to Raise GPIO Changed Ximple event Exception {0}", ex.Message);
            }
        }

        /// <summary>
        /// Create a GpioStateChanged model from PeripheralGpioEventArg
        /// </summary>
        /// <param name="peripheralGpioEventArg">PeripheralGpioEventArg arg</param>
        /// <returns>GpioStateChanged</returns>
        private GpioStateChanged CreateCreateGpioState(PeripheralGpioEventArg peripheralGpioEventArg)
        {
            GpioStateChanged gpioStateChanged = new GpioStateChanged();
            foreach (var arg in peripheralGpioEventArg.GpioInfo)
            {
                gpioStateChanged.GppioStates.Add(new GpioState(arg.Gpio.ToString(), arg.Active));
            }

            return gpioStateChanged;
        }

        private void CloseAll()
        {
            Logger.Info("{0} Enter", nameof(this.CloseAll));
            lock (this)
            {
                if (this.AudioSwitchSerialClient != null)
                {
                    Logger.Info("PeripheralProtocolImpl Closing Audio Switch Client");
                    this.AudioSwitchSerialClient.GpioChanged -= this.AudioSwitchSerialClientOnGpioChanged;
                    this.AudioSwitchSerialClient.Close();
                    this.AudioSwitchSerialClient.Dispose();
                    this.AudioSwitchSerialClient = null;
                }
            }
        }

        public bool IsConfigured
        {
            get
            {
                return configurationCompleted;
            }
        }

        /// <summary>
        /// Configure the Audio Switch for normal operations. Request the version as an initial test
        /// then send the config setting and set the initial default volume.
        /// </summary>
        /// <param name="peripheralConfigFile"></param>
        private void ConfigureAudioSwitch(string peripheralConfigFile)
        {
            if (this.configurationCompleted ||
                this.AudioSwitchSerialClient == null ||
                this.AudioSwitchSerialClient.IsComPortOpen == false)
            {
                return;
            }

            Thread.CurrentThread.Name = nameof(ConfigureAudioSwitch);
            Logger.Info("InitializeAudioSwitchClient() Enter Configure Audio Client from File:{0}", peripheralConfigFile);
            var peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig(peripheralConfigFile);
            var interiorVol = peripheralAudioConfig.InteriorDefaultVolume;
            var exteriorVol = peripheralAudioConfig.ExteriorDefaultVolume;


            // read in the initial audio configuration and send to the audio switch.
            try
            {
                bool done = false;
                ulong count = 0;
                int msTimeout = 1000;
                var state = AudioConfigStates.RequestVersion;

                // Get the Interior and Exterior default volumes from the PeripheralAudioConfig file
                // If we successfully received the version back then we have current later hardware that we can configure
                // we expect the Audio switch to respond with it's Version information and initial GPIO status.
                // after we get the version info, set the initial volumes
                bool configSet = false;
                while (!done && this.AudioSwitchSerialClient.IsComPortOpen)
                {
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    switch (state)
                    {
                        case AudioConfigStates.RequestVersion:
                            var versionReceived = this.AudioSwitchSerialClient.WriteVersionRequest(10000);
                            if (versionReceived)
                            {
                                state = AudioConfigStates.WriteConfig;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case AudioConfigStates.WriteConfig:
                            // Sending the Audio Config generates a replay of Version info and GPIO. 
                            // Wait for the reply of the Version info being received. This should occur during the initial configure exchange
                            if (configSet == false)
                            {
                                if (this.AudioSwitchSerialClient.WriteAudioConfig(peripheralAudioConfig) > 0)
                                {
                                    configSet = true;
                                    state = AudioConfigStates.WaitVersion;
                                }
                            }
                            else
                            {
                                state = AudioConfigStates.WaitVersion;
                            }
                            break;
                        case AudioConfigStates.WaitVersion:
                            Logger.Info("Waiting for Peripheral Version to be received before sending Audio status request Count={0}", count);
                            state = this.AudioSwitchSerialClient.IsVersionInfoReceived
                                        ? AudioConfigStates.WriteVolumes
                                        : AudioConfigStates.RequestVersion;
                            break;
                        case AudioConfigStates.WriteVolumes:
                            this.AudioSwitchSerialClient.WriteSetVolume(interiorVol, exteriorVol);
                            state = AudioConfigStates.ConfigCompleted;
                            break;
                        default:
                            done = true;
                            break;
                    }
                    Thread.Sleep(msTimeout);
                }

                if (!done)
                {
                    Logger.Error(
                        "Failed to receive Peripheral Version, Connection may be invalid to Audio Switch Com:{0}",
                        this.AudioSwitchSerialClient.PortName);
                }
                else
                {
                    var handler = this.AudioSwitchConfigurationCompleted;
                    handler?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (TaskCanceledException)
            {
                Logger.Info("Background Task ConfigureAudioSwitch Cancled");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to receive Peripheral Version, Connection may be invalid to Audio Switch Exception {0}", ex.Message);
            }

        }

        /// <summary>Initialize the Audio Client with it's settings file</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool InitializeAudioSwitchClient()
        {
            // read in our audio client config settings, com port, baud rate etc
            var configFile = PathManager.Instance.GetPath(FileType.Config, AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName);
            if (File.Exists(configFile) == false)
            {
                configFile = AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName;
            }

            Logger.Info("Run Enter Creating AudioSwitchSerialClient Reading Config file: {0}", configFile);
            var audioSwitchConfig = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(configFile);
            this.IgnoreFirstGpioEvent = audioSwitchConfig.IgnoreFirstGpioEvent;
            this.AudioSwitchSerialClient = new AudioSwitchSerialClient(audioSwitchConfig);
            this.AudioSwitchSerialClient.GpioChanged += this.AudioSwitchSerialClientOnGpioChanged;

            if (this.AudioSwitchSerialClient == null)
            {
                return false;
            }

            // read in the Audio Mux/Switch's config and send these values to it over the serial port
            var peripheralConfigFile = PathManager.Instance.GetPath(FileType.Config, PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile);
            if (File.Exists(peripheralConfigFile) == false)
            {
                peripheralConfigFile = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;

                if (File.Exists(configFile) == false)
                {
                    Logger.Warn("InitializeAudioSwitchClient() File Not Found {0}", configFile);
                    return false;
                }
            }

            if (this.AudioSwitchSerialClient.IsComPortOpen)
            {
                Logger.Info("InitializeAudioSwitchClient() Enter Configure Audio Client from File:{0}", peripheralConfigFile);
                var ct = cancellationTokenSource.Token;

                Task.Factory.StartNew(
                    () =>
                        {
                            ConfigureAudioSwitch(peripheralConfigFile);
                        }, ct);
            }

            return this.AudioSwitchSerialClient.IsComPortOpen;
        }

        /// <summary>The raise started.</summary>
        /// <param name="e">The e.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        private void RaiseStarted(EventArgs e)
        {
            Logger.Info("{0} Enter", nameof(this.RaiseStarted));
            var handler = this.Started;
            handler?.Invoke(this, e);
        }

        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            Logger.Info("{0} Enter", nameof(this.RaiseXimpleCreated));
            var ximple = e.Ximple;
            if (ximple == null || this.Host == null)
            {
                // no XIMPLE to send.
                return;
            }

            this.Host.OnDataFromProtocol(this, e.Ximple);
            var firstCell = e.Ximple.Cells.FirstOrDefault();
            var tableNumber = 0;
            if (firstCell != null)
            {
                tableNumber = firstCell.TableNumber;
            }

            Logger.Info("Luminator PeripheralProtocol sending XIMPLE TableNumber:{0} to Host", tableNumber);
        }

        #endregion
    }
}