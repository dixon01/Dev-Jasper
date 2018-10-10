// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MotionUnitController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MotionUnitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Unit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.Controllers.App;
    using Gorba.Center.Diag.Core.Controllers.Gioom;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Subscription;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.FileSystem;
    using Gorba.Common.Medi.Ports;
    using Gorba.Common.Medi.Ports.Config;
    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.ServiceLocation;

    using RemoteViewing.Vnc;

    using ApplicationInfo = Gorba.Common.SystemManagement.Client.ApplicationInfo;

    /// <summary>
    /// Unit controller for imotion units.
    /// </summary>
    internal class MotionUnitController : UnitControllerBase<MotionUnitViewModel>
    {
        private static readonly TimeSpan BroadcastPingTimeout = TimeSpan.FromSeconds(2);

        private static readonly TimeSpan InitialRefreshTimeout = TimeSpan.FromSeconds(1);

        private static readonly TimeSpan RefreshTimeout = TimeSpan.FromSeconds(20);

        private readonly ICommandRegistry commandRegistry;

        private readonly ITimer refreshTimer;

        private readonly List<IDisposable> broadcastSubscriptions = new List<IDisposable>();

        private IRootMessageDispatcher messageDispatcher;

        private Pinger pinger;

        private RemoteSystemManagerClient systemManagementClient;

        private RemoteFileSystem fileSystem;

        private RemoteGioomClient gioomClient;

        private GioomPortsController gioomPortsController;

        private bool receivedPong;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionUnitController"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public MotionUnitController(MotionUnitViewModel viewModel, ICommandRegistry commandRegistry)
            : base(viewModel)
        {
            this.commandRegistry = commandRegistry;

            this.refreshTimer = TimerFactory.Current.CreateTimer("MotionUnitRefresh");
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.Interval = InitialRefreshTimeout;
            this.refreshTimer.Elapsed += this.RefreshTimerOnElapsed;
        }

        private UnitTab UnitTab
        {
            get
            {
                return this.ViewModel.Shell.Tabs.OfType<UnitTab>().FirstOrDefault(t => t.Unit == this.ViewModel);
            }
        }

        /// <summary>
        /// Connects to the unit.
        /// </summary>
        public override void Connect()
        {
            if (this.ViewModel.ConnectionState != ConnectionState.Disconnected
                || this.ViewModel.ConnectionMode == ConnectionMode.NotAvailable)
            {
                // TODO: should we notify the caller that we can't connect?
                return;
            }

            if (this.ViewModel.ConnectionMode == ConnectionMode.Local && this.ViewModel.IpAddress == null)
            {
                // TODO: should we notify the caller that we can't connect?
                return;
            }

            if (this.messageDispatcher != null)
            {
                this.Disconnect();
            }

            this.Logger.Info(
                "Connecting to Unit '{0}' ({1}) in mode {2}",
                this.ViewModel.Name,
                this.ViewModel.IpAddress,
                this.ViewModel.ConnectionMode);

            this.ViewModel.ConnectionState = ConnectionState.Connecting;

            var mediConfig = this.CreateMediConfig();
            this.messageDispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(mediConfig, Environment.MachineName, "icenter.diag-" + Guid.NewGuid()));

            this.pinger = new Pinger(this.messageDispatcher);
            this.receivedPong = false;
            this.refreshTimer.Enabled = true;
        }

        /// <summary>
        /// Disconnects from the unit.
        /// </summary>
        public override void Disconnect()
        {
            this.Logger.Debug("Disconnecting from Medi on {0}", this.ViewModel.IpAddress);

            this.refreshTimer.Enabled = false;
            this.refreshTimer.Interval = InitialRefreshTimeout;

            this.broadcastSubscriptions.ForEach(s => s.Dispose());
            this.broadcastSubscriptions.Clear();

            this.ViewModel.Applications.Clear();
            this.ApplicationControllers.ForEach(c => c.Dispose());
            this.ApplicationControllers.Clear();

            var tab = this.UnitTab;
            if (tab != null)
            {
                foreach (var section in tab.InfoSections.OfType<IDisposable>())
                {
                    section.Dispose();
                }

                tab.InfoSections.OfType<ApplicationInfoSectionViewModel>()
                    .ToList()
                    .ForEach(a => tab.InfoSections.Remove(a));
            }

            if (this.systemManagementClient != null)
            {
                this.systemManagementClient.Dispose();
                this.systemManagementClient = null;
            }

            if (this.gioomPortsController != null)
            {
                this.gioomPortsController.Dispose();
                this.gioomPortsController = null;
            }

            if (this.gioomClient != null)
            {
                this.gioomClient.Dispose();
                this.gioomClient = null;
            }

            if (this.fileSystem != null)
            {
                this.fileSystem.Dispose();
                this.fileSystem = null;
            }

            if (this.messageDispatcher != null)
            {
                this.messageDispatcher.Dispose();
                this.messageDispatcher = null;
            }

            this.ViewModel.ConnectionState = ConnectionState.Disconnected;
        }

        /// <summary>
        /// Requests the unit to reboot through System Manager.
        /// </summary>
        public void RebootUnit()
        {
            if (this.systemManagementClient == null)
            {
                this.Logger.Warn("Couldn't request unit reboot since we don't have SM client");
                return;
            }

            this.systemManagementClient.Reboot("Requested by icenter.diag user");
        }

        /// <summary>
        /// Updates the <see cref="IUnitController.ViewModel"/> from the given database readable model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void UpdateFrom(UnitReadableModel model)
        {
            model.PropertyChanged -= this.ModelOnPropertyChanged;
            model.PropertyChanged += this.ModelOnPropertyChanged;
            this.UpdateConnectionMode(model.IsConnected);
        }

        /// <summary>
        /// Updates the <see cref="IUnitController.ViewModel"/> from the given UDCP response.
        /// </summary>
        /// <param name="response">
        /// The response which is expected to be a GetInformation response.
        /// </param>
        public void UpdateFrom(UdcpResponse response)
        {
            if (response.Header.Type != DatagramType.GetInformation)
            {
                return;
            }

            this.Logger.Debug("Updating unit '{0}' from UDCP response", this.ViewModel.DisplayName);
            this.ViewModel.UdcpAddress = response.Header.UnitAddress;

            var dhcp = response.GetField<DhcpEnabledField>();
            this.ViewModel.DhcpEnabled = dhcp != null && dhcp.Value;
            this.ViewModel.IpAddress = GetIpAddress<IpAddressField>(response);
            this.ViewModel.NetworkMask = GetIpAddress<NetworkMaskField>(response);
            this.ViewModel.GatewayAddress = GetIpAddress<GatewayField>(response);
            this.ViewModel.Name = GetString<UnitNameField>(response);
            this.ViewModel.SoftwareVersion = GetString<SoftwareVersionField>(response);

            this.ViewModel.ConnectionMode = ConnectionMode.Local;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.gioomPortsController != null)
            {
                this.gioomPortsController.Dispose();
            }
        }

        private static string GetString<T>(UdcpDatagram response) where T : StringFieldBase
        {
            var field = response.GetField<T>();
            return field != null ? field.Value : null;
        }

        private static IPAddress GetIpAddress<T>(UdcpDatagram response) where T : IpAddressFieldBase
        {
            var field = response.GetField<T>();
            return field != null ? field.Value : null;
        }

        private MediConfig CreateMediConfig()
        {
            var isGateway = true;
            var transportClientConfig = new TcpTransportClientConfig { ReconnectWait = 1000 };
            if (this.ViewModel.ConnectionMode == ConnectionMode.Local)
            {
                // TODO: make this more dynamic using additional UDCP fields (MediPort, ...)
                transportClientConfig.RemoteHost = this.ViewModel.IpAddress.ToString();
                isGateway = false;
                this.Logger.Debug(
                    "Connecting to Medi at {0}:{1}",
                    transportClientConfig.RemoteHost,
                    transportClientConfig.RemotePort);
            }
            else
            {
                var appController = ServiceLocator.Current.GetInstance<IDiagApplicationController>();
                var connectionString =
                    appController.ConnectionController.BackgroundSystemConfiguration.NotificationsConnectionString;

                var uriBuilder = new UriBuilder(connectionString);
                transportClientConfig.RemoteHost = uriBuilder.Host;
                if (uriBuilder.Port != -1)
                {
                    transportClientConfig.RemotePort = uriBuilder.Port;
                }

                this.Logger.Debug(
                    "Connecting to BGS Medi at {0}:{1}",
                    transportClientConfig.RemoteHost,
                    transportClientConfig.RemotePort);
            }

            var resourcesDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Gorba\\Center\\Diag\\Resources");
            return new MediConfig
                       {
                           InterceptLocalLogs = false, // nobody wants to see our logs
                           Peers =
                               {
                                   new ClientPeerConfig
                                       {
                                           Codec = new BecCodecConfig(),
                                           Transport = transportClientConfig,
                                           IsGateway = isGateway
                                       }
                               },
                           Services =
                               {
                                   new LocalResourceServiceConfig { ResourceDirectory = resourcesDir },
                                   new PortForwardingServiceConfig()
                               }
                       };
        }

        private void UpdateConnectionMode(bool isConnectedToBackgroundSystem)
        {
            if (this.ViewModel.ConnectionMode != ConnectionMode.Local)
            {
                this.ViewModel.ConnectionMode = isConnectedToBackgroundSystem
                                                    ? ConnectionMode.BackgroundSystem
                                                    : ConnectionMode.NotAvailable;
            }
        }

        private void StartInformationQueries()
        {
            this.systemManagementClient.BeginGetApplicationInfos(this.GotApplicationInfos, null);
            this.systemManagementClient.BeginGetSystemInfo(this.GotSystemInfo, null);
            if (this.ViewModel.ConnectionMode == ConnectionMode.Local)
            {
                this.BeginFindPorts(new MediAddress(this.ViewModel.Name, "*"));
            }
        }

        private void BeginFindPorts(MediAddress address)
        {
            this.gioomClient.BeginFindPorts(address, BroadcastPingTimeout, this.FoundPorts, null);
        }

        private void BroadcastPong(IAsyncResult ar)
        {
            var addresses = this.pinger.EndBroadcastPing(ar);
            this.StartNew(() => this.UpdateApplications(addresses));
        }

        private void Pong(IAsyncResult ar)
        {
            this.pinger.EndPing(ar);
            this.receivedPong = true;
            this.StartNew(this.CreateRemoteClients);
        }

        private void GotApplicationInfos(IAsyncResult ar)
        {
            var apps = this.systemManagementClient.EndGetApplicationInfos(ar);
            this.StartNew(() => this.UpdateApplications(apps));
        }

        private void GotSystemInfo(IAsyncResult ar)
        {
            var systemInfo = this.systemManagementClient.EndGetSystemInfo(ar);
            this.StartNew(() => this.UpdateSystemInfo(systemInfo));
        }

        private void FoundPorts(IAsyncResult ar)
        {
            var ports = this.gioomClient.EndFindPorts(ar);
            this.StartNew(() => this.UpdateGioomPorts(ports));
        }

        private RemoteAppController GetApplication(string appName)
        {
            var app = this.ApplicationControllers.FirstOrDefault(c => c.HasApplicationName(appName));
            if (app != null)
            {
                return app;
            }

            var tab = this.UnitTab;
            if (tab == null)
            {
                return null;
            }

            var appViewModel = new RemoteAppViewModel(this.ViewModel);
            this.ViewModel.Applications.Add(appViewModel);

            tab.InfoSections.Add(new ApplicationInfoSectionViewModel(appViewModel, this.commandRegistry));

            app = new RemoteAppController(appViewModel, this, this.messageDispatcher);
            this.ApplicationControllers.Add(app);

            return app;
        }

        private void UpdateApplications(IEnumerable<MediAddress> addresses)
        {
            if (this.messageDispatcher == null)
            {
                return;
            }

            var foreignAddresses = addresses.Where(a => !a.Equals(this.messageDispatcher.LocalAddress)).ToArray();
            if (foreignAddresses.Length == 0)
            {
                return;
            }

            var firstApplication = foreignAddresses[0];
            var found = false;
            if (string.IsNullOrEmpty(this.ViewModel.Name))
            {
                // we need to figure out our name first (we were added manually)
                this.ViewModel.Name = firstApplication.Unit;
            }

            foreach (var address in
                foreignAddresses.Where(
                    a => a.Unit.Equals(this.ViewModel.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                var app = this.GetApplication(address.Application);
                if (app == null)
                {
                    continue;
                }

                app.UpdateFrom(address);
                found = true;
            }

            if (!found)
            {
                return;
            }

            this.receivedPong = true;
            this.CreateRemoteClients();
            this.CreateFileSystem(firstApplication);
        }

        private void CreateRemoteClients()
        {
            if (this.systemManagementClient != null)
            {
                return;
            }

            // first time we get a response from an application, let's prepare everything
            // lower the refersh frequency once we get the first response
            this.refreshTimer.Interval = RefreshTimeout;

            this.systemManagementClient = new RemoteSystemManagerClient(this.ViewModel.Name, this.messageDispatcher);
            this.gioomClient = new RemoteGioomClient(this.messageDispatcher);
            this.gioomPortsController = new GioomPortsController(this.ViewModel.GioomPorts);
            this.StartInformationQueries();

            this.ViewModel.ConnectionState = ConnectionState.Connected;

            var tab = this.UnitTab;
            if (tab == null)
            {
                return;
            }

            var remoteViewer = tab.InfoSections.OfType<RemoteViewerSectionViewModel>().FirstOrDefault();
            if (remoteViewer == null)
            {
                return;
            }

            remoteViewer.ConnectCommand = new RelayCommand(this.ConnectRemoteViewer);
            remoteViewer.PropertyChanged += this.RemoteViewerOnPropertyChanged;
            remoteViewer.VncClient.Connected += this.VncClientOnConnected;
            remoteViewer.VncClient.ConnectionFailed += this.VncClientOnClosed;
            remoteViewer.VncClient.Closed += this.VncClientOnClosed;
        }

        private void RemoteViewerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var remoteViewer = sender as RemoteViewerSectionViewModel;
            if (remoteViewer == null || e.PropertyName != "IsInteractive")
            {
                return;
            }

            if (!remoteViewer.IsInteractive)
            {
                return;
            }

            if (MessageBox.Show(
                DiagStrings.RemoteViewer_InteractiveQuery,
                DiagStrings.RemoteViewer_InteractiveQueryTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No) != MessageBoxResult.Yes)
            {
                remoteViewer.IsInteractive = false;
            }
        }

        private void ConnectRemoteViewer()
        {
            var systemManager =
                this.ApplicationControllers.FirstOrDefault(
                    c => c.ViewModel.ApplicationType == ApplicationType.SystemManager);
            if (systemManager == null)
            {
                return;
            }

            var portForwarding = this.messageDispatcher.GetService<IPortForwardingService>();
            portForwarding.BeginConnect(
                systemManager.ViewModel.Address,
                new TcpClientEndPointConfig
                    {
                        RemoteAddress = "127.0.0.1",
                        RemotePort = 5900
                    },
                this.PortForwardingConnected,
                null);
        }

        private void PortForwardingConnected(IAsyncResult ar)
        {
            var portForwarding = this.messageDispatcher.GetService<IPortForwardingService>();
            var stream = portForwarding.EndConnect(ar);

            var unitTab = this.UnitTab;
            if (unitTab == null)
            {
                stream.Close();
                return;
            }

            var section = unitTab.InfoSections.OfType<RemoteViewerSectionViewModel>().FirstOrDefault();
            if (section == null)
            {
                stream.Close();
                return;
            }

            var password = section.Password.ToCharArray();
            ThreadPool.QueueUserWorkItem(
                s =>
                    {
                        try
                        {
                            section.VncClient.Connect(stream, new VncClientConnectOptions { Password = password });
                        }
                        catch (Exception ex)
                        {
                            this.Logger.ErrorException("Couldn't connect to VNC server", ex);
                            stream.Close();
                            this.VncClientOnClosed(this, EventArgs.Empty);
                            MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
        }

        private void VncClientOnConnected(object sender, EventArgs e)
        {
            var tab = this.UnitTab;
            if (tab == null)
            {
                return;
            }

            var section = tab.InfoSections.OfType<RemoteViewerSectionViewModel>().FirstOrDefault();
            if (section == null)
            {
                return;
            }

            var appController = ServiceLocator.Current.GetInstance<IDiagApplicationController>();
            section.SupportsInteractive = appController.ShellController.UserCanInteract;
        }

        private void VncClientOnClosed(object sender, EventArgs e)
        {
            var tab = this.UnitTab;
            if (tab == null)
            {
                return;
            }

            var section = tab.InfoSections.OfType<RemoteViewerSectionViewModel>().FirstOrDefault();
            if (section == null)
            {
                return;
            }

            section.IsConnected = false;
            section.SupportsInteractive = false;
            section.IsInteractive = false;
        }

        private void CreateFileSystem(MediAddress firstApplication)
        {
            if (this.fileSystem != null)
            {
                return;
            }

            this.fileSystem = new RemoteFileSystem(firstApplication, this.messageDispatcher);
            Task.Run(() => this.AddFileSystemRoots());
        }

        private void AddFileSystemRoots()
        {
            var fs = this.fileSystem;
            if (fs == null)
            {
                return;
            }

            IEnumerable<FolderViewModel> rootFolders;
            try
            {
                try
                {
                    rootFolders = this.GetRootFolders(fs);
                }
                catch (TimeoutException ex)
                {
                    this.Logger.WarnException(
                        "Load of drives of " + this.ViewModel.DisplayName + " timed out, retrying",
                        ex);
                    rootFolders = this.GetRootFolders(fs);
                }
            }
            catch (Exception ex)
            {
                this.Logger.WarnException("Couldn't load drives of " + this.ViewModel.DisplayName, ex);
                return;
            }

            this.StartNew(
                () =>
                    {
                        this.ViewModel.FileSystemRoots.Clear();
                        rootFolders.ForEach(this.ViewModel.FileSystemRoots.Add);
                    });
        }

        private IEnumerable<FolderViewModel> GetRootFolders(IFileSystem fs)
        {
            return fs.GetDrives().Select(d => new FolderViewModel(d.RootDirectory, this.ViewModel));
        }

        private void UpdateApplications(IEnumerable<ApplicationInfo> applications)
        {
            foreach (var applicationInfo in applications)
            {
                var appName = Path.GetFileNameWithoutExtension(applicationInfo.Path);
                var app = this.GetApplication(appName);
                if (app == null)
                {
                    continue;
                }

                app.UpdateFrom(applicationInfo, this.systemManagementClient);
                if (this.ViewModel.ConnectionMode != ConnectionMode.BackgroundSystem)
                {
                    continue;
                }

                // we don't get the real Medi addresses from the BGS, so let's guess it
                var address = new MediAddress(
                    this.ViewModel.Name,
                    Path.GetFileNameWithoutExtension(applicationInfo.Path));
                app.UpdateFrom(address);
                this.BeginFindPorts(address);

                if (this.broadcastSubscriptions.Count != 0)
                {
                    continue;
                }

                // this is done for the first application only, which should always be System Manager
                var broadcastSubscriptionService =
                    this.messageDispatcher.GetService<IBroadcastSubscriptionService>();
                this.broadcastSubscriptions.Add(
                    broadcastSubscriptionService.AddSubscription<StateChangeNotification>(address));
                this.CreateFileSystem(address);
            }
        }

        private void UpdateSystemInfo(SystemInfo systemInfo)
        {
            var tab = this.UnitTab;
            if (tab == null)
            {
                return;
            }

            var section = tab.InfoSections.OfType<SystemInfoSectionViewModel>().FirstOrDefault();
            if (section == null)
            {
                return;
            }

            this.UpdateGauge(section.CpuUsage, systemInfo.CpuUsage * 100, 100, "%");
            this.UpdateGauge(
                section.RamUsage,
                (systemInfo.TotalRam - systemInfo.AvailableRam) / 1024.0 / 1024,
                systemInfo.TotalRam / 1024.0 / 1024,
                "MB");

            foreach (var disk in systemInfo.Disks)
            {
                var gauge = section.DiskUsages.FirstOrDefault(g => g.Label == disk.Name);
                if (gauge == null)
                {
                    gauge = new GaugeViewModel { Label = disk.Name };
                    section.DiskUsages.Add(gauge);
                }

                this.UpdateGauge(
                    gauge,
                    (disk.TotalSize - disk.AvailableFreeSpace) / 1024.0 / 1024 / 1024,
                    disk.TotalSize / 1024.0 / 1024 / 1024,
                    "GB");
            }
        }

        private void UpdateGauge(GaugeViewModel gauge, double value, double maximum, string unit)
        {
            var step = 1;
            while (maximum / step > 10)
            {
                step *= 10;
            }

            if (maximum / step <= 2)
            {
                step /= 5;
            }
            else if (maximum / step <= 3)
            {
                step /= 2;
            }

            gauge.Maximum = maximum;
            gauge.MajorTickStep = step;
            gauge.Value = value;
            gauge.Unit = unit;
            gauge.Tooltip = string.Format("{0:0.0} {1}", value, unit);
        }

        private void UpdateGioomPorts(IPortInfo[] ports)
        {
            this.gioomPortsController.UpdateFrom(ports, this.gioomClient);
            foreach (var appController in this.ApplicationControllers)
            {
                var controller = appController;
                appController.GioomPortsController.UpdateFrom(
                    ports.Where(p => p.Address.Equals(controller.ViewModel.Address)),
                    this.gioomClient);
            }
        }

        private void RefreshTimerOnElapsed(object s, EventArgs e)
        {
            var hadReceivedPong = this.receivedPong;
            this.receivedPong = false;
            if (this.ViewModel.ConnectionMode == ConnectionMode.Local)
            {
                this.pinger.BeginBroadcastPing(BroadcastPingTimeout, this.BroadcastPong, null);
            }
            else
            {
                var address = new MediAddress(this.ViewModel.Name, "Update");
                this.pinger.BeginPing(address, this.Pong, null);
            }

            if (this.systemManagementClient == null)
            {
                return;
            }

            if (!hadReceivedPong && this.ViewModel.ConnectionMode == ConnectionMode.Local)
            {
                // we didn't receive a pong since the last ping, so let's disconnect
                this.StartNew(this.Disconnect);
                return;
            }

            this.StartInformationQueries();
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var model = sender as UnitReadableModel;
            if (model == null || e.PropertyName != "IsConnected")
            {
                return;
            }

            this.StartNew(
                () =>
                    {
                        this.UpdateConnectionMode(model.IsConnected);
                        if (!model.IsConnected && this.ViewModel.ConnectionState != ConnectionState.Disconnected)
                        {
                            this.Disconnect();
                        }
                    });
        }
    }
}