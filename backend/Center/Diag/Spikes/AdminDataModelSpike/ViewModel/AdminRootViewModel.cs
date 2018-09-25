namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;

    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Utility.Core;

    public class AdminRootViewModel : SynchronizableViewModelBase
    {
        public static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(20);

        private readonly ObservableCollection<UnitViewModelBase> units = new ObservableCollection<UnitViewModelBase>();

        private readonly UdcpServer server;

        private readonly ITimer refreshTimer = TimerFactory.Current.CreateTimer("RootRefresh");

        private TabableViewModelBase selectedNode;

        public AdminRootViewModel()
        {
            this.Sections = new ObservableCollection<UnitSectionViewModel>();

            this.server = new UdcpServer();
            this.server.ResponseReceived += this.ServerOnResponseReceived;

            var localUnit = new LocalUnitViewModel();
            this.units.Add(localUnit);

            // TODO: remove debugging:
            //this.units.Add(new RemoteUnitViewModel { Name = Environment.MachineName, IpAddress = IPAddress.Loopback });

            this.Sections.Add(new UnitSectionViewModel("Local", this.units, u => u == localUnit));
            this.Sections.Add(new UnitSectionViewModel("Connected", this.units, u => u.CanConnect && u.IsConnected));
            this.Sections.Add(new UnitSectionViewModel("Favorites", this.units, u => u.IsFavorite));
            this.Sections.Add(
                new UnitSectionViewModel("Other", this.units, u => !u.IsFavorite && u.CanConnect && !u.IsConnected));

            this.server.Start();

            this.SendGetInformationRequest();

            this.refreshTimer.Interval = RefreshInterval;
            this.refreshTimer.Elapsed += this.RefreshTimerOnElapsed;
            this.refreshTimer.Enabled = true;
        }

        private void SendGetInformationRequest()
        {
            this.server.SendDatagram(new UdcpRequest(DatagramType.GetInformation, UdcpAddress.BroadcastAddress));
        }

        private void RefreshTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.SendGetInformationRequest();
        }

        private void ServerOnResponseReceived(object sender, UdcpDatagramEventArgs<UdcpResponse> e)
        {
            if (e.Datagram.Header.Type == DatagramType.GetInformation)
            {
                // restart the timer whenever we get a response
                this.refreshTimer.Enabled = false;
                this.refreshTimer.Enabled = true;
                this.TaskFactory.StartNew(() => this.AddUnit(e.Datagram));
            }
        }

        private void AddUnit(UdcpResponse response)
        {
            var unit =
                this.units.OfType<RemoteUnitViewModel>()
                    .FirstOrDefault(u => response.Header.UnitAddress.Equals(u.UdcpAddress));
            if (unit != null)
            {
                unit.Update(response);
                return;
            }

            unit = new RemoteUnitViewModel();
            unit.Update(response);
            this.units.Add(unit);
        }

        public ObservableCollection<UnitSectionViewModel> Sections { get; private set; }

        public TabableViewModelBase SelectedNode
        {
            get
            {
                return this.selectedNode;
            }

            set
            {
                this.SetProperty(ref this.selectedNode, value, () => this.SelectedNode);
            }
        }
    }
}
