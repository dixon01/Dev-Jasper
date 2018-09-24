namespace AdminDataModelSpike.ViewModel
{
    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Fields;

    public class UdcpInfoViewModel : InfoViewModelBase
    {
        private string ipAddress;

        private string networkMask;

        private string gateway;

        public UdcpInfoViewModel()
        {
            this.Name = "System Info";
        }

        public string IpAddress
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

        public string NetworkMask
        {
            get
            {
                return this.networkMask;
            }

            set
            {
                this.SetProperty(ref this.networkMask, value, () => this.NetworkMask);
            }
        }

        public string Gateway
        {
            get
            {
                return this.gateway;
            }

            set
            {
                this.SetProperty(ref this.gateway, value, () => this.Gateway);
            }
        }

        public void Update(UdcpResponse response)
        {
            this.IpAddress = GetValue(response.GetField<IpAddressField>());
            this.NetworkMask = GetValue(response.GetField<NetworkMaskField>());
            this.Gateway = GetValue(response.GetField<GatewayField>());
        }

        private static string GetValue(IpAddressFieldBase field)
        {
            return field == null ? "n/a" : field.Value.ToString();
        }
    }
}