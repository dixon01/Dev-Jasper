namespace Luminator.Multicast.Receive.Client.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Timers;
    using System.Windows;

    using Luminator.Multicast.Core;
    using Luminator.Multicast.Receive.Client.Wpf.Properties;
    using Luminator.ResourceLibrary.Utils;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Static Fields

        private static readonly Timer programTimer = new Timer(20000);

        private static List<IPAddress> localIpsViableForUseAsMulticast = new List<IPAddress>();

        private static IPAddress mcastAddress;

        private static int mcastPort;

        private static MulticastReceive multicast;

        #endregion

        #region Fields

        private readonly BackgroundWorker multicastMcuListernerBackgroundWorker = new BackgroundWorker();

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            FtpUsername = Settings.Default.FtpUserName;
            FtpPassword = Settings.Default.FtpPassword;
            LocalFilePath = "Test.zip";
            this.Notifications.Add(FtpUsername + ":" + FtpPassword);

            // Initialize the multicast address group and multicast port.
            // Both address and port are selected from the allowed sets as
            // defined in the related RFC documents. These are the same 
            // as the values used by the sender.
            mcastAddress = IPAddress.Parse(Settings.Default.MulticastIp);
            mcastPort = Settings.Default.MulticastPort;
            this.Notifications.Add(mcastAddress + ":" + mcastPort);

            localIpsViableForUseAsMulticast = NetworkUtils.GetAllActiveLocalIpAddress();

            foreach (var ipAddress in localIpsViableForUseAsMulticast)
            {
                this.LocalIpsCollection.Add(ipAddress.ToString());
                this.Notifications.Add(ipAddress.ToString());
            }

            this.multicastMcuListernerBackgroundWorker.DoWork += this.MulticastMcuListernerBackgroundWorkerDoWork;
            this.multicastMcuListernerBackgroundWorker.ProgressChanged += this.MulticastMcuListernerBackgroundWorkerProgressChanged;
            this.multicastMcuListernerBackgroundWorker.WorkerReportsProgress = true;
            this.multicastMcuListernerBackgroundWorker.RunWorkerCompleted += this.MulticastMcuListernerBackgroundWorkerRunWorkerCompleted;
        }

        #endregion

        #region Public Properties

        public static string FtpPassword { get; set; }

        public static string FtpUsername { get; set; }

        public static string LocalFilePath { get; set; }

        public static bool ResetNetwork { get; set; }

        public static bool StaticSystemIpSetTestPassed { get; set; }

        public ObservableCollection<string> LocalIpsCollection { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> Notifications { get; set; } = new ObservableCollection<string>();

        public string SelectedIp { get; set; }

        #endregion

        #region Properties

        private static bool AllWorkComplete { get; set; }

        private static bool StaticSystemIpSet { get; set; }

        #endregion

        #region Methods

        private void LookForMcuClick(object sender, RoutedEventArgs e)
        {
            this.multicastMcuListernerBackgroundWorker.RunWorkerAsync();
        }

        private void MultiCastManager(IPAddress localIpAddressBoundtoMulticast)
        {
            this.InvokeOnUI(() => { this.Notifications.Add(multicast == null ? "Multicast is Null" : "Multicast is not Null"); });

            if (multicast == null)
            {
                this.InvokeOnUI(() => { this.Notifications.Add("-----------Multicast was null, Initilizing---------------"); });
                multicast = new MulticastReceive(mcastAddress, mcastPort, localIpAddressBoundtoMulticast);
                try
                {
                    NetworkChange.NetworkAvailabilityChanged -= this.NetworkChangeNetworkAvailabilityChanged;
                    NetworkChange.NetworkAddressChanged -= this.NetworkChangeNetworkAddressChanged;
                    NetworkChange.NetworkAvailabilityChanged += this.NetworkChangeNetworkAvailabilityChanged;
                    NetworkChange.NetworkAddressChanged += this.NetworkChangeNetworkAddressChanged;

                    // Start a multicast group.
                    multicast.StartMulticast(mcastAddress, mcastPort);

                    // Display MulticastOption properties.
                    multicast.MulticastOptionProperties();

                    // Receive broadcast messages.
                    multicast.ReceiveBroadcastMessages();
                }
                catch (Exception e)
                {
                    this.Notifications.Add(e.Message);
                }
            }
            else
            {
                multicast.Dispose();
                multicast = null;
            }
        }

        private void MulticastMcuListernerBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.MultiCastManager(IPAddress.Parse(this.SelectedIp));
            }
            catch (Exception exception)
            {
                this.Notifications.Add(exception.Message);
            }
        }

        private void MulticastMcuListernerBackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.InvokeOnUI(() => { this.Notifications.Add("MulticastMcuListernerBackgroundWorker_ProgressChanged"); });
        }

        private void MulticastMcuListernerBackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.InvokeOnUI(() => { this.Notifications.Add("MulticastMcuListernerBackgroundWorker_RunWorkerCompleted"); });
        }

        private void NetworkChangeNetworkAddressChanged(object sender, EventArgs e)
        {
            // if(NetworkManagement.IsNetworkedStatusDown()) 
            //   NetworkManagement.SetDhcp(multicast.LocalAdaptorDescription);
            NetworkUtils.GetAllLocalIpAddress().ForEach(x => this.Notifications.Add("NetworkChangeNetworkAddressChanged - Local IP =>" + x.ToString()));
            NetworkUtils.GetAllLocalIpAddress().ForEach(x => this.Notifications.Add("Adaptor Desc : " + NetworkUtils.GetAdapterDescFromIp4Address(x)));
        }

        private void NetworkChangeNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            NetworkUtils.GetAllLocalIpAddress().ForEach(x => this.Notifications.Add("NetworkChangeNetworkAddressChanged - Local IP =>" + x.ToString()));
            NetworkUtils.GetAllLocalIpAddress().ForEach(x => this.Notifications.Add("Adaptor Desc : " + NetworkUtils.GetAdapterDescFromIp4Address(x)));
        }

        private void ResetAdaptor_Click(object sender, RoutedEventArgs e)
        {
            this.Notifications.Add(DateTime.Now + "===================STEP 2: Reset The Network and wait ==================");
            NetworkManagement.SetDhcp(multicast.McuResponseReceivedAtAdaptor);
            NetworkUtils.NetworkReset();
            programTimer.Interval = 60000;
            ResetNetwork = true;
        }

        private void SendFtpFile_Click(object sender, RoutedEventArgs e)
        {
            if (StaticSystemIpSetTestPassed)
            {
                Console.WriteLine(DateTime.Now + "===================STEP 4: FTP send a file called Connected.txt ==================");
                if (multicast != null)
                {
                    NetworkManagement.SendFtpTestFile(multicast, FtpUsername, FtpPassword);
                    var ftp = new FtpUtils(multicast.McuResponsedRemoteIp.GetIpAddress().ToString(), FtpUsername, FtpPassword);
                     var ftpFile = ftp.FtpFileExists(new Uri("ftp://" + multicast.McuResponsedRemoteIp.GetIpAddress() + "Connected.txt"));
                    Console.WriteLine(ftpFile ? "Ftp Success" : "Ftp Failed");
                    AllWorkComplete = true;
                }
            }
        }

        private void setStaticIP_Click(object sender, RoutedEventArgs e)
        {
            if (multicast != null && !Equals(multicast.McuResponseReceivedAtLocalIpAddress, IPAddress.None) && ResetNetwork)
            {
                this.Notifications.Add(DateTime.Now + "===================STEP 3: Set the System Ip from MCU response ==================");

                NetworkManagement.SetSystemIp(
                    NetworkUtils.IncrementIpAddress(multicast.McuResponsedRemoteIp.GetIpAddress()).ToString(),
                    "255.255.255.0",
                    multicast.McuResponseReceivedAtAdaptor);

                StaticSystemIpSet = true;
                programTimer.Interval = 30000;
            }
        }

        private void VerifyFtpFile_Click(object sender, RoutedEventArgs e)
        {
            if (StaticSystemIpSetTestPassed)
            {
                Console.WriteLine(DateTime.Now + "===================STEP 5: FTP send a file called Connected.txt ==================");
                if (multicast != null)
                {
                    NetworkManagement.SendFtpTestFile(multicast, FtpUsername, FtpPassword);
                    var ftp = new FtpUtils(multicast.McuResponsedRemoteIp.GetIpAddress().ToString(), FtpUsername, FtpPassword);
                    var ftpFile = ftp.FtpFileExists(new Uri("ftp://" + multicast.McuResponsedRemoteIp.GetIpAddress() + "Connected.txt"));
                    Console.WriteLine(ftpFile ? "Ftp Success" : "Ftp Failed");
                    AllWorkComplete = true;
                }
            }
        }

        private void VerifyStaticIp_Click(object sender, RoutedEventArgs e)
        {
            if (StaticSystemIpSet)
            {
                this.Notifications.Add(DateTime.Now + "===================STEP 3: Verify System IP was Set ==================");
                if (multicast != null)
                {
                    this.Notifications.Add("Remote Ip" + multicast.McuResponsedRemoteIp.GetIpAddress());
                    this.Notifications.Add("Local Multicast Adaptor" + multicast.McuResponseReceivedAtAdaptor);
                    this.Notifications.Add("Local Static Ip Was Set To : " + multicast.McuResponseReceivedAtLocalIpAddress);
                    var adapterDescFromIp4Address = NetworkUtils.GetAdapterDescFromIp4Address(multicast.McuResponseReceivedAtLocalIpAddress);
                    if (multicast.McuResponseReceivedAtAdaptor.Equals(adapterDescFromIp4Address))
                    {
                        StaticSystemIpSetTestPassed = true;
                        this.Notifications.Add("Static SystemIp Set Test Passed");
                        programTimer.Interval = 60000;
                    }
                }
            }
        }

        #endregion
    }
}