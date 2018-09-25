namespace ProtectedOperationsTester.Controllers
{
    using System;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel;

    using ProtectedOperationsTester.Contracts;
    using ProtectedOperationsTester.Core;
    using ProtectedOperationsTester.ViewModels;

    [Export]
    public class UnitServiceController
    {
        private readonly Lazy<Shell> shell;

        [ImportingConstructor]
        public UnitServiceController(Lazy<Shell> shell)
        {
            this.shell = shell;
        }

        public Shell Shell
        {
            get
            {
                return this.shell.Value;
            }
        }

        [Export(Commands.GetUnitLegacy)]
        public ICommand CreateGetUnitLegacyCommand()
        {
            return new RelayCommand(this.GetUnitLegacy);
        }

        [Export(Commands.GetUnitLogin)]
        public ICommand CreateGetUnitLoginCommand()
        {
            return new RelayCommand(this.GetUnitLogin);
        }

        [Export(Commands.GetUnitCertificate)]
        public ICommand CreateGetUnitCertificateCommand()
        {
            return new RelayCommand(this.GetUnitCertificate);
        }

        private void GetUnitLegacy()
        {
            try
            {
                var binding = new NetTcpBinding(SecurityMode.None);
                var factory = new ChannelFactory<IUnitService>(binding);
                var baseUri = this.GetUri();
                var address = new EndpointAddress(
                    new Uri(baseUri));
                var channel = factory.CreateChannel(address);
                var unit = channel.Get(1);
                MessageBox.Show(string.Format("Unit '{0}' found", unit.Name));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetUri(string path = null)
        {
            int netTcpPort;
            var netTcpPortValue = ConfigurationManager.AppSettings["NetTcpPort"];
            if (string.IsNullOrWhiteSpace(netTcpPortValue) || !int.TryParse(netTcpPortValue, out netTcpPort))
            {
                netTcpPort = 808;
            }

            return string.Format("net.tcp://localhost:{0}/BackgroundSystem/UnitService.svc{1}", netTcpPort, path);
        }

        private void GetUnitLogin()
        {
            try
            {
                var binding = new NetTcpBinding(SecurityMode.Message);
                binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                var factory = new ChannelFactory<IUnitService>(binding);
                factory.SetLogin(this.Shell.Username, this.Shell.Password.ToMd5());
                var uri = this.GetUri("/Login");
                var address = new EndpointAddress(
                    new Uri(uri),
                    EndpointIdentity.CreateDnsIdentity(this.Shell.DnsIdentity));
                var channel = factory.CreateChannel(address);
                var unit = channel.Get(2);
                MessageBox.Show(string.Format("Unit '{0}' found", unit.Name));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetUnitCertificate()
        {
            try
            {
                var binding = new NetTcpBinding(SecurityMode.Message);
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                var factory = new ChannelFactory<IUnitService>(binding);
                factory.SetCertificate(this.Shell.Certificate);
                var uri = this.GetUri("/Certificate");
                var address = new EndpointAddress(
                    new Uri(uri),
                    EndpointIdentity.CreateDnsIdentity(this.Shell.DnsIdentity));
                var channel = factory.CreateChannel(address);
                var unit = channel.Get(3);
                MessageBox.Show(string.Format("Unit '{0}' found", unit.Name));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
