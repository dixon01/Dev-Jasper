namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Fields;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Core;

    public class RemoteUnitViewModel : UnitViewModelBase
    {
        private static readonly TimeSpan BroadcastPingTimeout = TimeSpan.FromSeconds(2);

        private static readonly TimeSpan InitialRefreshTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan RefreshTimeout = TimeSpan.FromSeconds(20);

        private readonly ITimer refreshTimer;

        private UdcpInfoViewModel udcpInfo;

        private IPAddress ipAddress;

        private IRootMessageDispatcher messageDispatcher;

        private Pinger pinger;

        private RemoteSystemManagerClient systemManagementClient;

        private ApplicationListInfoViewModel applicationList;

        public RemoteUnitViewModel()
        {
            this.CanConnect = true;
            this.refreshTimer = TimerFactory.Current.CreateTimer("RemoteUnitRefresh");
            this.refreshTimer.AutoReset = true;
            this.refreshTimer.Interval = InitialRefreshTimeout;
            this.refreshTimer.Elapsed += this.RefreshTimerOnElapsed;
        }

        public UdcpAddress UdcpAddress { get; private set; }

        public IPAddress IpAddress
        {
            get
            {
                return this.ipAddress;
            }

            set
            {
                this.SetProperty(ref this.ipAddress, value, () => this.IpAddress);
            }
        }

        public void Update(UdcpResponse response)
        {
            if (this.udcpInfo == null)
            {
                this.udcpInfo = new UdcpInfoViewModel();
                this.Tabs.Insert(0, this.udcpInfo);
            }

            this.UdcpAddress = response.Header.UnitAddress;
            this.Name = response.GetField<UnitNameField>().Value;
            this.IpAddress = response.GetField<IpAddressField>().Value;
            this.udcpInfo.Update(response);
        }

        protected override void Connect()
        {
            if (this.IpAddress == null)
            {
                this.IsConnected = false;
                return;
            }

            if (this.messageDispatcher != null)
            {
                this.Disconnect();
            }

            // TODO: make this more dynamic using additional UDCP fields (MediPort, ...)
            var mediConfig = new MediConfig
                {
                    Peers =
                        {
                            new ClientPeerConfig
                                {
                                    Codec = new BecCodecConfig(),
                                    Transport = new TcpTransportClientConfig
                                            {
                                                ReconnectWait = 1000,
                                                RemoteHost = this.IpAddress.ToString()
                                            }
                                }
                        }
                };
            this.messageDispatcher =
                MessageDispatcher.Create(
                    new ObjectConfigurator(mediConfig, Environment.MachineName, "icenter.admin-" + Guid.NewGuid()));

            this.pinger = new Pinger(this.messageDispatcher);

            this.refreshTimer.Enabled = true;
        }

        protected override void Disconnect()
        {
            this.Applications.Clear();
            if (this.messageDispatcher == null)
            {
                return;
            }

            this.refreshTimer.Enabled = false;

            if (this.systemManagementClient != null)
            {
                this.systemManagementClient.Dispose();
            }

            this.messageDispatcher.Dispose();
            this.messageDispatcher = null;
        }

        private void Refresh()
        {
            this.pinger.BeginBroadcastPing(BroadcastPingTimeout, this.BroadcastPong, null);
            if (this.systemManagementClient != null)
            {
                this.systemManagementClient.BeginGetApplicationInfos(this.GotApplicationInfos, null);
            }
        }

        private void BroadcastPong(IAsyncResult ar)
        {
            var addresses = this.pinger.EndBroadcastPing(ar);
            this.TaskFactory.StartNew(() => this.UpdateApplications(addresses));
        }

        private void GotApplicationInfos(IAsyncResult ar)
        {
            var apps = this.systemManagementClient.EndGetApplicationInfos(ar);
            this.TaskFactory.StartNew(() => this.UpdateApplications(apps));
        }

        private void UpdateApplications(IEnumerable<MediAddress> addresses)
        {
            var found = false;
            foreach (var address in
                addresses.Where(a => a.Unit.Equals(this.Name)))
            {
                var app = this.GetApplication(address.Application);
                app.Update(address);
                found = true;
            }

            if (!found || this.systemManagementClient != null)
            {
                return;
            }

            // lower the refersh frequency once we get the first resposne
            this.refreshTimer.Interval = RefreshTimeout;

            this.systemManagementClient = new RemoteSystemManagerClient(this.Name, this.messageDispatcher);
            this.systemManagementClient.BeginGetApplicationInfos(this.GotApplicationInfos, null);
        }

        private void UpdateApplications(IList<ApplicationInfo> applications)
        {
            if (this.applicationList == null)
            {
                this.applicationList = new ApplicationListInfoViewModel();
                this.Tabs.Add(this.applicationList);
            }

            this.applicationList.Update(applications, this.systemManagementClient);

            foreach (var applicationInfo in applications)
            {
                var appName = Path.GetFileNameWithoutExtension(applicationInfo.Path);
                var app = this.GetApplication(appName);
                app.Update(applicationInfo, this.systemManagementClient);
            }
        }

        private ApplicationViewModel GetApplication(string appName)
        {
            var app = this.Applications.FirstOrDefault(t => t.HasApplicationName(appName));
            if (app == null)
            {
                app = new ApplicationViewModel(this.messageDispatcher);
                this.Applications.Add(app);
            }

            return app;
        }

        private void RefreshTimerOnElapsed(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}