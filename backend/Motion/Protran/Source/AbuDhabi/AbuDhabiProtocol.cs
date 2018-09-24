// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;
    using Gorba.Motion.Protran.AbuDhabi.Ctu;
    using Gorba.Motion.Protran.AbuDhabi.Ibis;
    using Gorba.Motion.Protran.AbuDhabi.Isi;
    using Gorba.Motion.Protran.AbuDhabi.Ism;
    using Gorba.Motion.Protran.AbuDhabi.Multiplexing;
    using Gorba.Motion.Protran.AbuDhabi.StateMachineCycles;
    using Gorba.Motion.Protran.Core.Protocols;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Class that represents the manager of the AbuDhabi communications.
    /// </summary>
    public class AbuDhabiProtocol : IProtocol, IManageableObject
    {
        /// <summary>
        /// The name of the configuration file.
        /// Attention (this is not the absolute file's name).
        /// </summary>
        private const string AbuDhabiCfgFileName = "AbuDhabi.xml";

        private const string TopboxDataDirectory = @"D:\Infomedia";

        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Event used to manage the running status of this protocol.
        /// </summary>
        private readonly AutoResetEvent liveEvent = new AutoResetEvent(false);

        private readonly DeviceStateValueProvider deviceStateProtranProvider =
            new DeviceStateValueProvider(DeviceClass.InteriorDisplay, 0, DeviceState.Ok);

        private readonly DeviceStateValueProvider deviceStateCu5Provider =
            new DeviceStateValueProvider(DeviceClass.ExteriorDisplay, 0, DeviceState.NoConnection);

        private readonly DeviceStateValueProvider[] deviceStateDisplayProviders = new[]
            {
                new DeviceStateValueProvider(DeviceClass.ExteriorDisplay, 1, DeviceState.NoConnection),
                new DeviceStateValueProvider(DeviceClass.ExteriorDisplay, 2, DeviceState.NoConnection),
                new DeviceStateValueProvider(DeviceClass.ExteriorDisplay, 3, DeviceState.NoConnection),
                new DeviceStateValueProvider(DeviceClass.ExteriorDisplay, 4, DeviceState.NoConnection)
            };

        private readonly FtpTransferValueProvider ftpStatusValueProvider;

        /// <summary>
        /// A reference to the manager about the connection with the
        /// remote CU5 device (via UDP).
        /// </summary>
        private readonly CtuClient ctu;

        /// <summary>
        /// This object represents the "portal" to Protran core.
        /// </summary>
        private IProtocolHost host;

        /// <summary>
        /// A reference to the manager about the Abu Dhabi configurations.
        /// </summary>
        private ConfigMng configMng;

        /// <summary>
        /// A reference to the manager about the connection with the
        /// remote ISI TCP/IP server.
        /// </summary>
        private IsiClient isiClient;

        /// <summary>
        /// A reference to the manager about the connection with the
        /// remote ISM FTP server.
        /// </summary>
        private IsmClient ismClient;

        /// <summary>
        /// Host for the IBIS protocol that is used for CTS (stop requested) and
        /// fallback in case ISI or CTU is not available.
        /// </summary>
        private IbisProtocolHost ibisProtocol;

        private CycleManager cycle;

        /// <summary>
        /// Object tasked to switch between ISI and IBIS protocols as source for Ximple.
        /// </summary>
        private XimpleArbiter arbiter;

        private DynamicDataItemValueProvider serialNumberCu5Provider;

        private DynamicDataItemValueProvider softwareVersionProvider;

        private DynamicDataItemValueProvider dataVersionProvider;

        private PortListener specialInputPortListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiProtocol"/> class.
        /// Allocates and initializes all the required things for the Abu Dhabi protocol.
        /// </summary>
        public AbuDhabiProtocol()
        {
            this.ctu = ServiceLocator.Current.GetInstance<CtuClient>();
            this.isiClient = null;
            this.ismClient = null;
            this.configMng = new ConfigMng();

            var path = PathManager.Instance.GetPath(FileType.Config, AbuDhabiCfgFileName);
            Logger.Info("Loading config file: {0}", path);
            this.configMng.Load(path);

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);

            this.ftpStatusValueProvider = new FtpTransferValueProvider();
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired whenever the this protocol creates
        /// a new ximple object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get { return "ABU DHABI"; }
        }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the config assigned to this protocol.
        /// </summary>
        public AbuDhabiConfig Config
        {
            get
            {
                return this.configMng.AbuDhabiConfig;
            }
        }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        ///     The generic view dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Deletes and closes all the objects created
        /// by this object.
        /// </summary>
        public void Stop()
        {
            if (this.specialInputPortListener != null)
            {
                this.specialInputPortListener.ValueChanged -= this.SpecialInputPortOnValueChanged;
                this.specialInputPortListener.Dispose();
                this.specialInputPortListener = null;
            }

            this.liveEvent.Set();
            this.CloseAll();
        }

        /// <summary>
        /// Configures this protocol with all the information gathered from the configuration file
        /// (in order to create opportunely the ISI, ISM and CU5 connections, and so on...).
        /// </summary>
        /// <param name="configManager">The config manager.</param>
        public void Configure(ConfigMng configManager)
        {
            this.configMng = configManager;

            if (this.configMng.AbuDhabiConfig.IsiSimulation.Enabled)
            {
                // we register the simulation remote computer if we have to run in
                // simulation mode
                var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
                serviceContainer.RegisterInstance<RemoteComputer>(
                    new SimulationRemoteComputer(this.configMng.AbuDhabiConfig.IsiSimulation));
            }

            this.ismClient = new IsmClient(this.configMng.AbuDhabiConfig.Ism);
            this.ismClient.FtpDownloading += this.IsmClientFtpDownloading;

            this.isiClient = new IsiClient(this.Dictionary, this.configMng.AbuDhabiConfig);
            this.isiClient.RemoteComputer.IsiMessageReceived += this.RemoteComputerOnIsiMessageReceived;
            this.isiClient.PriorityXimpleCreated += (s, e) => this.RaiseXimpleCreated(e);

            this.isiClient.AddDataItemValueProvider(DataItemName.DeviceState, this.deviceStateProtranProvider);
            this.isiClient.AddDataItemValueProvider(DataItemName.DeviceState, this.deviceStateCu5Provider);
            this.isiClient.AddDataItemValueProvider(
                DataItemName.IsiClientRunsFtpTransfers, this.ftpStatusValueProvider);
            foreach (var provider in this.deviceStateDisplayProviders)
            {
                this.isiClient.AddDataItemValueProvider(DataItemName.DeviceState, provider);
            }

            var softwareVersion = this.GetProtranVersionNumber();
            this.softwareVersionProvider = new DynamicDataItemValueProvider(string.Format("PT: {0}", softwareVersion));
            this.isiClient.AddDataItemValueProvider(DataItemName.CurrentSoftwareVersion, this.softwareVersionProvider);

            var dataVersion = this.GetTopboxDataVersion();
            this.dataVersionProvider = new DynamicDataItemValueProvider(string.Format("TB: {0}", dataVersion));
            this.isiClient.AddDataItemValueProvider(DataItemName.CurrentParameterVersion, this.dataVersionProvider);

            this.ctu.Configure(this.configMng.AbuDhabiConfig.Cu5);
            this.ctu.CommunicationStarted += this.OnCtuCommunicationStarted;
            this.ctu.ConnectionErrorOccured += this.OnCtuConnectionError;
            this.ctu.InactivityStatusDetected += this.OnCtuInactivityDetected;
            this.ctu.RemoteStatusChanged += this.CtuOnRemoteStatusChanged;
            this.ctu.DisplayStatusChanged += this.CtuOnDisplayStatusChanged;

            if (this.configMng.AbuDhabiConfig.Ibis.Enabled)
            {
                this.ibisProtocol = new IbisProtocolHost(this.Dictionary);
                this.ibisProtocol.Configure(this.configMng.AbuDhabiConfig);
                this.ibisProtocol.PriorityXimpleCreated += (s, e) => this.RaiseXimpleCreated(e);
                this.ibisProtocol.SpecialInputStateChanged += this.IbisProtocolOnSpecialInputStateChanged;

                this.arbiter = new XimpleArbiter(this.isiClient, this.ibisProtocol);
                this.arbiter.XimpleCreated += (s, e) => this.RaiseXimpleCreated(e);
                this.arbiter.SourceChanged += this.ArbiterOnSourceChanged;
                this.arbiter.SecondarySourceTimeout += this.ArbiterOnSecondarySourceTimeout;
            }
            else
            {
                // no IBIS means no arbiter, so we have to get the Ximple directly
                // from the ISI client.
                this.isiClient.XimpleCreated += (s, e) => this.RaiseXimpleCreated(e);
            }
        }

        /// <summary>
        /// Starts all the internal activities to this Protocol.
        /// </summary>
        /// <param name="protocolHost">The owner of this protocol.</param>
        public void Run(IProtocolHost protocolHost)
        {
            this.host = protocolHost;

            if (this.configMng.InitOk)
            {
                // ok, the load phase was succedeed.
                // I can configure my protocol.
                this.Configure(this.configMng);
            }

            this.specialInputPortListener = new PortListener(MessageDispatcher.Instance.LocalAddress, "SpecialInput");
            this.specialInputPortListener.ValueChanged += this.SpecialInputPortOnValueChanged;

            this.cycle = new CycleManager();
            this.cycle.Configure(this.configMng.AbuDhabiConfig, this.Dictionary);
            this.cycle.XimpleCreated += (s, e) => this.RaiseXimpleCreated(e);

            if (this.ibisProtocol != null)
            {
                // if we have IBIS enabled, start the arbiter first and then start the IBIS protocol
                this.arbiter.Start();
                this.ibisProtocol.Start();
            }

            // now it's the time to give the "life" to the CU5 client.
            // we don't start the ISI client here since this will be done when we get CTU
            // responses from the CU5
            this.ctu.Start();
            Logger.Info("CU5 Client " + (this.ctu.IsRunning ? "running." : "not running."));

            this.RaiseStarted(EventArgs.Empty);

            this.specialInputPortListener.Start(TimeSpan.FromSeconds(10));

            // ok. Now we can wait for the end of this protocol.
            this.liveEvent.WaitOne();

            // at this line of code, to the protocol was ordered to be stopped.
            this.CloseAll();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Cu5", parent, this.ctu);
            yield return parent.Factory.CreateManagementProvider("Isi", parent, this.isiClient);
            yield return parent.Factory.CreateManagementProvider("File Downloads (FTP)", parent, this.ismClient);
            yield return parent.Factory.CreateManagementProvider("Cycle State Machine", parent, this.cycle);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("Ximple Source", this.ArbiterSource(), true);
            yield return new ManagementProperty<string>("Fallback state", this.ctu.LocalStatus.ToString(), true);
        }

        private string ArbiterSource()
        {
            return this.arbiter.CurrentSource == this.ibisProtocol ? "Ibis" : "ISI";
        }

        private void SpecialInputPortOnValueChanged(object sender, EventArgs eventArgs)
        {
            this.SendSpecialInputValue();
        }

        private void SendSpecialInputValue()
        {
            this.ctu.SendSpecialInputState(FlagValues.True.Equals(this.specialInputPortListener.Value));
        }

        /// <summary>
        /// Close all the running threads and delete all the resources
        /// allocated during the program's execution.
        /// </summary>
        private void CloseAll()
        {
            lock (this)
            {
                Logger.Info("Closing all...");

                if (this.arbiter != null)
                {
                    this.arbiter.Stop();
                }

                if (this.ibisProtocol != null)
                {
                    this.ibisProtocol.Stop();
                }

                // it's the time to remove the "life" from the CU5.
                this.ctu.Stop();

                // it's the time to remove the "life" from the ISI client.
                this.isiClient.Stop();

                // it's the time to remove the "life" from the ISM client.
                this.ismClient.Stop();

                Logger.Info("All closed. Good bye.");
            }
        }

        private DeviceState GetCu5DeviceState()
        {
            if (this.ctu.DeviceInfo == null)
            {
                return DeviceState.NoConnection;
            }

            return this.ctu.RemoteStatus == StatusCode.Ok ? DeviceState.Ok : DeviceState.ConfigurationError;
        }

        private DeviceState GetDisplayState(int id)
        {
            DisplayStatusCode status;
            if (!this.ctu.DisplayStatusCodes.TryGetValue(id, out status))
            {
                return DeviceState.NoConnection;
            }

            switch (status)
            {
                case DisplayStatusCode.BacklightError:
                    return DeviceState.Defective;
                case DisplayStatusCode.DisplayError:
                    return DeviceState.Defective;
                case DisplayStatusCode.NoConnection:
                    return DeviceState.NoConnection;
                case DisplayStatusCode.Ok:
                    return DeviceState.Ok;
                case DisplayStatusCode.Initializing:
                    return DeviceState.Ok;
                default:
                    return DeviceState.Ok;
            }
        }

        /// <summary>
        /// Function called asynchronously by the CU5 whenever
        /// it recognizes that the communication channel
        /// between it and Protran is established.
        /// </summary>
        /// <param name="sender">The CU5 object.</param>
        /// <param name="e">The event about the communication establishment.</param>
        private void OnCtuCommunicationStarted(object sender, EventArgs e)
        {
            // now it's the time to give the "life" to the ISI client.
            var deviceInfo = this.ctu.DeviceInfo;
            this.softwareVersionProvider.Value = string.Format(
                "PT: {0}; CU: {1}", this.GetProtranVersionNumber(), deviceInfo.SoftwareVersion);
            this.dataVersionProvider.Value = string.Format(
                "TB: {0}; CU: {1}", this.GetTopboxDataVersion(), deviceInfo.DataVersion);
            var serialNumber = string.Format("{0:D},{1}", DeviceClass.ExteriorDisplay, deviceInfo.SerialNumber);
            if (this.serialNumberCu5Provider == null)
            {
                this.serialNumberCu5Provider = new DynamicDataItemValueProvider(serialNumber);
                this.isiClient.AddDataItemValueProvider(DataItemName.SerialNumber, this.serialNumberCu5Provider);
            }
            else
            {
                this.serialNumberCu5Provider.Value = serialNumber;
            }

            this.deviceStateCu5Provider.State = this.GetCu5DeviceState();
            foreach (var provider in this.deviceStateDisplayProviders)
            {
                provider.State = this.GetDisplayState(provider.DeviceNumber);
            }

            this.isiClient.Start();
            this.ismClient.Start();
        }

        /// <summary>
        /// Function called asynchronously by the CU5 object whenever
        /// an error on its socket is detected.
        /// </summary>
        /// <param name="sender">The CU5 object.</param>
        /// <param name="e">The event about the socket error.</param>
        private void OnCtuConnectionError(object sender, EventArgs e)
        {
            Logger.Error("CU5 client socket error.");
            this.RestartCu5Client();
        }

        /// <summary>
        /// Function called asynchronously by the CU5 object whenever
        /// it is inactive since to much time.
        /// </summary>
        /// <param name="sender">The CU5 object.</param>
        /// <param name="e">The event about the inactivity status.</param>
        private void OnCtuInactivityDetected(object sender, EventArgs e)
        {
            Logger.Error("CU5 inactive for too long.");
            this.RestartCu5Client();
        }

        private void CtuOnRemoteStatusChanged(object sender, EventArgs e)
        {
            this.deviceStateCu5Provider.State = this.GetCu5DeviceState();
        }

        private void CtuOnDisplayStatusChanged(object sender, EventArgs e)
        {
            foreach (var provider in this.deviceStateDisplayProviders)
            {
                provider.State = this.GetDisplayState(provider.DeviceNumber);
            }
        }

        private void RestartCu5Client()
        {
            this.deviceStateCu5Provider.State = DeviceState.NoConnection;

            if (!this.isiClient.IsRunning)
            {
                Logger.Info("Starting ISI client without CU5");
                this.isiClient.Start();
                this.ismClient.Start();
            }

            // for safety, I stop the CU5.
            this.ctu.Stop();

            // wait 10 seconds before we reconnect
            var timer = new Timer(s => this.ctu.Start());
            timer.Change(10 * 1000, Timeout.Infinite);
        }

        /// <summary>
        /// Function called asynchronously by the ISI client object whenever
        /// it has received some information from the remote ISI TCP/IP server.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event ISI message.</param>
        private void RemoteComputerOnIsiMessageReceived(object sender, IsiMessageEventArgs e)
        {
            // we have received right now some ISI item from the ISI client.
            // depending on the items, we have to perform some actions.
            var isiPut = e.IsiMessage as IsiPut;

            this.ManageIsiPut(isiPut);

            // now it's the time to pass the ISI message to the CTU client.
            // it also can be interesting to its content.
            this.ctu.HandleIsiMessage(e.IsiMessage);
        }

        /// <summary>
        /// Manages the ISI put depending on the its content.
        /// </summary>
        /// <param name="isiPut">The ISI put item to be managed.</param>
        private void ManageIsiPut(IsiPut isiPut)
        {
            if (isiPut == null)
            {
                // nothing to manage.
                return;
            }

            var fallbackItem = isiPut.Items.Find(i => i.Name.Equals(DataItemName.GorbaSystemFallbackActive));
            if (fallbackItem != null)
            {
                this.ManageFallbackState(fallbackItem);
            }
        }

        /// <summary>
        /// Does all the required actions to do in case of an ISI put item
        /// with the information about the fallback state:
        /// - changes (eventually) the XIMPLE source
        /// - logs activities
        /// </summary>
        /// <param name="fallbackIsiPutItem">The ISI put item having the information for the fallback state.</param>
        private void ManageFallbackState(DataItem fallbackIsiPutItem)
        {
            if (fallbackIsiPutItem == null || !this.configMng.AbuDhabiConfig.Ibis.Enabled)
            {
                return;
            }

            int result;
            if (!int.TryParse(fallbackIsiPutItem.Value, out result))
            {
                return;
            }

            var isIbisActive = result == 1;
            Logger.Info("ISI put fallback state: {0}", isIbisActive ? "ACTIVE" : "NOT ACTIVE");
            var source = isIbisActive ? this.ibisProtocol : (IXimpleSource)this.isiClient;
            this.arbiter.CurrentSource = source;
        }

        private void IbisProtocolOnSpecialInputStateChanged(object sender, EventArgs eventArgs)
        {
            this.ctu.SendSpecialInputState(this.ibisProtocol.SpecialInputState);
        }

        private void ArbiterOnSourceChanged(object sender, EventArgs e)
        {
            this.ctu.LocalStatus = this.arbiter.CurrentSource == this.ibisProtocol
                                         ? StatusCode.Fallback
                                         : StatusCode.Ok;
        }

        private void ArbiterOnSecondarySourceTimeout(object sender, EventArgs eventArgs)
        {
            if (!this.Config.Ibis.RestartOnTimeout)
            {
                return;
            }

            Logger.Info("Stopping Protran because of timeout on IBIS source");

            var registration = ServiceLocator.Current.GetInstance<IApplicationRegistration>("Protran");
            registration.Relaunch("Timeout on IBIS source");
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Function called whenever a XIMPLE was created and ready to be sent
        /// to protran core.
        /// </summary>
        /// <param name="e">
        /// The event containing the XIMPLE.
        /// </param>
        private void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var ximple = e.Ximple;
            if (ximple == null || this.host == null)
            {
                // no XIMPLE to send.
                return;
            }

            this.host.OnDataFromProtocol(this, e.Ximple);
            if (this.cycle != null)
            {
                this.cycle.ExtractDatafromXimple(e.Ximple);
            }

            Logger.Info("Sent a XIMPLE to Protran");
            Logger.Trace(ximple.ToXmlString);

            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Function invoked asynchronously whenever the ISM client starts or ends
        /// an FTP download.
        /// </summary>
        /// <param name="sender">The sender about this kind of event (the ISM client).</param>
        /// <param name="e">The event arguments containing the FTP download status.</param>
        private void IsmClientFtpDownloading(object sender, FtpStatusEventArgs e)
        {
            // I've to advise the ISI remote computer about a change on our FTP status.
            // to do this point, it's enough to update the value of the right data provider.
            this.ftpStatusValueProvider.Value = e.DownloadStatus == DownloadStatus.Running;
            Logger.Info("Current FTP status: {0}", e.DownloadStatus);

            if (e.DownloadStatus == DownloadStatus.NotRunning)
            {
                this.ctu.BeginFileTransferToCu5();
            }
        }

        private string GetProtranVersionNumber()
        {
            var protranExe = PathManager.Instance.GetPath(FileType.Application, "Protran.exe");
            if (protranExe == null)
            {
                protranExe = PathManager.Instance.GetPath(FileType.Application, "ProtranVisualizer.exe");
            }

            return FileVersionInfo.GetVersionInfo(protranExe).ProductVersion;
        }

        private string GetTopboxDataVersion()
        {
            try
            {
                // this file is written by the ISM client when extracting the data
                var dataVersionPath = Path.Combine(TopboxDataDirectory, IsmClient.VersionFileName);
                if (File.Exists(dataVersionPath))
                {
                    return File.ReadAllText(dataVersionPath).Trim();
                }

                Logger.Warn("Data version file doesn't exist: {0}", dataVersionPath);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't get data version file");
            }

            return "unknown";
        }
    }
}
