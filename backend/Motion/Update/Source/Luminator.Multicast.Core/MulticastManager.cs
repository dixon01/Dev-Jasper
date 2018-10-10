// MulticastManager.cs

namespace Luminator.Multicast.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Timers;

    using WindowsInput;
    using WindowsInput.Native;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.CommonEmb;

    using NLog;

    using Timer = System.Timers.Timer;

    public class MulticastManager : IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<MulticastManager>();

        #endregion

        #region Fields


        private readonly Timer programStepTimer = new Timer(10000);

        public MulticastConfig MulticastConfig;

        private MultiCastUpdateStatus multiCastUpdateStatus;

        private MulticastCommands multicastActionCommand;


        private readonly Stopwatch stopwatch = new Stopwatch();

        public bool ShowDisplaySent { get; set; }

        private int numberOfTimesStaticIpWasSet;

        #endregion

        #region Constructors and Destructors

        public MulticastManager()
        {
            this.OriginalAdaptorNetworkInfo = null;
            this.ShowDisplaySent = false;
            this.numberOfTimesStaticIpWasSet = 0;
        }

        #endregion

        #region Public Events

        public event EventHandler MultiCastCommand = delegate { };

        public event EventHandler MultiCastUpdateStatusChanged = delegate { };

        public event EventHandler<RemoteNetworkInfo> RemoteNetworkInfoChanged = delegate { };

        #endregion

        #region Public Properties

        public MulticastInfo MulticastReceivedInformation = new MulticastInfo();

        public MulticastCommands MulticastActionCommand
        {
            get
            {
                return this.multicastActionCommand;
            }
            set
            {
                //if (value != this.multicastActionCommand) // Invoke the commad even if it is the same
                {
                    this.multicastActionCommand = value;
                    this.MultiCastCommand(this, new MulticastCommandEventArgs(this.multicastActionCommand));
                }
            }
        }
        public MultiCastUpdateStatus MultiCastUpdateStatus
        {
            get
            {
                return this.multiCastUpdateStatus;

            }
            set
            {
                if (value != this.multiCastUpdateStatus)
                {
                    this.multiCastUpdateStatus = value;
                    this.MultiCastUpdateStatusChanged(this, new EventArgs());
                    if (this.multiCastUpdateStatus == MultiCastUpdateStatus.StaticIpVerified)
                    {
                        this.RemoteNetworkInfoChanged(
                            this,
                            new RemoteNetworkInfo
                            {
                                Username = this.MulticastConfig.FtpUsername,
                                Password = this.MulticastConfig.FtpPassword,
                                RemoteIpAddress = this.MulticastReceivedInformation.MulticastReceivedFromIpAddress,
                                SubnetMask = this.MulticastReceivedInformation.MuticastReceivedFromSubnetMask,
                                UpdateComplete = false
                            });
                    }
                }
            }
        }

        #endregion

        #region Properties

        private Stack<IPAddress> LocalIpAddressBoundtoMulticast { get; set; }

        private IPAddress LocalIpAddressBoundtoMulticastInProgress { get; set; }

        private MulticastReceive multicastReceive { get; set; }

        private AdaptorNetworkInfo OriginalAdaptorNetworkInfo { get; set; }

        private IPAddress localIpAddressToMatchMcuDomain { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void RestorePrevNetworkInfo(AdaptorNetworkInfo prevAdaptorNetworkInfo)
        {
            if (prevAdaptorNetworkInfo != null)
            {
                try
                {
                    Logger.Trace(
                        prevAdaptorNetworkInfo.IsDhcpEnabled
                            ? "-----------Restoring Previous Network To DHCP ------------"
                            : "-----------Restoring Original Network To Static IP ------------");
                    if (prevAdaptorNetworkInfo.IsDhcpEnabled)
                    {
                        NetworkManagement.SetDhcp(prevAdaptorNetworkInfo.Description);
                    }
                    else
                    {
                        NetworkManagement.SetSystemIp(
                            prevAdaptorNetworkInfo.SetTheAdaptorToIpAddress.ToString(),
                            prevAdaptorNetworkInfo.SubnetMask.ToString(),
                            prevAdaptorNetworkInfo.Description);
                        NetworkManagement.SetSystemGateway(prevAdaptorNetworkInfo.Gateway.ToString(), prevAdaptorNetworkInfo.Description);
                    }
                }
                catch (Exception e)
                {
                    Logger.Trace(e.Message + (e.InnerException?.Message ?? string.Empty));
                }
            }
        }

        public static AdaptorNetworkInfo SaveAdaptorNetworkInfo(string nameOrDesc, IPAddress staticIpBeingSetForThisNic, IPAddress subnetMask)
        {
            Logger.Trace("Saving SaveAdaptorNetworkInfo for " + nameOrDesc + ":" + staticIpBeingSetForThisNic);
            try
            {
                var physicalNics = NetworkUtils.GetAllActiveLocalPhysicalNics();
                foreach (var networkInterface in physicalNics)
                {
                    Logger.Trace("Try to match nameOrDesc Name {0} , Desc  {1} ", networkInterface.Name, networkInterface.Description);
                    if (networkInterface.Description.Equals(nameOrDesc) || networkInterface.Name.Equals(nameOrDesc))
                    {
                        var adaptorDescriptionOrName = networkInterface.Description;
                        var defaultGatewayOfNic = NetworkManagement.GetGatewayForAdaptor(adaptorDescriptionOrName);
                        var adaptorNetworkInfo = new AdaptorNetworkInfo
                        {
                            Description = adaptorDescriptionOrName,
                            SetTheAdaptorToIpAddress = staticIpBeingSetForThisNic,
                            Gateway = defaultGatewayOfNic,
                            IpAddress = staticIpBeingSetForThisNic,
                            SubnetMask = subnetMask,
                            IsDhcpEnabled = !NetworkManagement.IsNetworkedWithStaticIp(adaptorDescriptionOrName),
                            Name = networkInterface.Name
                        };
                        Logger.Trace(adaptorNetworkInfo.IsDhcpEnabled ? " --- Saving DHCP Information ------" : " ----------Saving Static IP Information --------------");
                        Logger.Trace(adaptorNetworkInfo);
                        return adaptorNetworkInfo;
                    }
                    Logger.Trace(" NOT Saving AdaptorInfo - name nor description matched. ");
                }
            }
            catch (Exception e)
            {
                Logger.Trace(e.Message + "\n Inner Exception =" + (e.InnerException?.Message ?? ""));
            }

            return new AdaptorNetworkInfo();
        }

        public void Dispose()
        {
            Logger.Trace(this.OriginalAdaptorNetworkInfo);
            RestorePrevNetworkInfo(this.OriginalAdaptorNetworkInfo);
            if (this.multicastReceive != null)
            {
                this.multicastReceive.Dispose();
            }
        }



        public void StartMulticast()
        {
            try
            {
                Logger.Trace("============= Starting Multicast =======================");
                this.MulticastConfig = new MulticastConfig();
                this.LocalIpAddressBoundtoMulticast = new Stack<IPAddress>(NetworkUtils.GetAllActiveLocalIpAddress());
                this.programStepTimer.Interval = this.MulticastConfig.ProgramStepInterval;
                this.programStepTimer.Enabled = true;
                this.programStepTimer.Elapsed += this.ProgramStepTimerElapsed;
                this.programStepTimer.Start();

                if (this.MultiCastUpdateStatus != MultiCastUpdateStatus.Idle)
                {
                    this.MultiCastUpdateStatus = MultiCastUpdateStatus.Idle;
                }

            }
            catch (Exception e)
            {
                Logger.Trace(MethodBase.GetCurrentMethod().Name + e.Message + e.InnerException?.Message);
            }
        }

        public void Stop()
        {
            this.Dispose();
        }

        public void StopMulticast()
        {
            Logger.Warn("============= Stopping Multicast =======================");
            this.stopwatch.Stop();
            Logger.Trace($"  Starting Downloads took {this.stopwatch.Elapsed.TotalSeconds:##.000} seconds");
            this.stopwatch.Restart();
            this.programStepTimer.Enabled = false;
            this.programStepTimer.Elapsed -= this.ProgramStepTimerElapsed;
            this.programStepTimer.Stop();
            if (this.multicastReceive != null)
            {
                this.multicastReceive.Dispose();
            }
        }

        #endregion

        #region Methods

        private void ProgramStepTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Trace(" Executing Step => " + this.MultiCastUpdateStatus.GetDescription());
            this.ProcessMulticastCommand();
            switch (this.MultiCastUpdateStatus)
            {
                case MultiCastUpdateStatus.Idle:
                    if (this.OriginalAdaptorNetworkInfo != null)
                    {
                        this.stopwatch.Stop();
                        Logger.Trace($" ########## Total time for Update {this.stopwatch.Elapsed.TotalSeconds:##.000} seconds");
                        Logger.Trace("OriginalAdaptorNetworkInfo was not NUll - Restore Begin" + this.OriginalAdaptorNetworkInfo);
                        RestorePrevNetworkInfo(this.OriginalAdaptorNetworkInfo);
                        this.MultiCastUpdateStatus = MultiCastUpdateStatus.RestorePreviosNetworkState;
                    }
                    else
                    {
                        Logger.Trace("OriginalAdaptorNetworkInfo was NUll - Not Restoring");
                        this.MultiCastUpdateStatus = MultiCastUpdateStatus.Started;
                    }
                    break;
                case MultiCastUpdateStatus.RestorePreviosNetworkState:
                    this.MultiCastUpdateStatus = MultiCastUpdateStatus.Started;
                    Logger.Trace(" ==================Reset Gateway============================");
                    if (this.OriginalAdaptorNetworkInfo.IsDhcpEnabled)
                    {
                        NetworkManagement.SetSystemGateway(this.OriginalAdaptorNetworkInfo.SetTheAdaptorToIpAddress.ToString(), this.OriginalAdaptorNetworkInfo.Description);
                    }
                    break;
                case MultiCastUpdateStatus.Started:
                    this.LocalIpAddressBoundtoMulticastInProgress = this.LocalIpAddressBoundtoMulticast.Pop();
                    this.Start(this.LocalIpAddressBoundtoMulticastInProgress);
                    this.MultiCastUpdateStatus = MultiCastUpdateStatus.WaitingForMcuResponse;
                    break;
                case MultiCastUpdateStatus.WaitingForMcuResponse:
                    this.MultiCastUpdateStatus = this.WaitForMcuResponse();
                    break;
                case MultiCastUpdateStatus.ReceivedMcuResponse:
                    this.stopwatch.Restart();
                    if (this.multicastReceive != null)
                    {
                        this.MulticastReceivedInformation = this.multicastReceive.MulticastReceivedInformation;
                        this.multicastReceive.Dispose();
                        this.multicastReceive = null;
                        if (this.MulticastReceivedInformation == null)
                            Logger.Warn($" {nameof(this.MulticastReceivedInformation)} cannot be null.");
                    }
                    if (this.MulticastReceivedInformation?.MulticastMessageReceived?.Contains("Action=Update") == true)
                    {
                        this.MultiCastUpdateStatus = this.SetStaticIpForNetworkAdaptor();
                    }
                    break;
                case MultiCastUpdateStatus.StaticIpSet:
                    this.MultiCastUpdateStatus = this.VerifyStaticIp();
                    break;
                case MultiCastUpdateStatus.StaticIpVerified:
                    Logger.Warn(
                        MultiCastUpdateStatus.StaticIpVerified.GetDescription()
                        + " Note: We are all done for now. Passing control to caller \n expecting a call to stop Muticast and then start after the client is done with his work.");
                    this.MultiCastUpdateStatus = MultiCastUpdateStatus.AllDone;
                    break;

                case MultiCastUpdateStatus.AllDone:
                case MultiCastUpdateStatus.ErrorDuringMulticast:
                    this.MultiCastUpdateStatus = MultiCastUpdateStatus.Idle;
                    break;
                default:
                    Logger.Warn("Should not be here - Unhandled case");
                    break;
            }
        }

        private void ProcessMulticastCommand()
        {
            var keyboardSimulator = new KeyboardSimulator(new InputSimulator());
            if (string.IsNullOrEmpty(this.MulticastReceivedInformation?.MulticastMessageReceived))
            {
                Logger.Trace(MethodBase.GetCurrentMethod().Name + $" Command was Empty ");
                if (this.ShowDisplaySent && this.MulticastConfig.UseWindowsInputSimulator)
                {
                    keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_S);
                    this.ShowDisplaySent = false;
                }
                return;
            }
            Logger.Trace(MethodBase.GetCurrentMethod().Name + $" : {this.MulticastReceivedInformation?.MulticastMessageReceived} ");
            if (this.MulticastReceivedInformation?.MulticastMessageReceived?.Contains("Action=Update") == true)
            {
                this.MulticastActionCommand = MulticastCommands.Update;
            }
            else if (this.MulticastReceivedInformation?.MulticastMessageReceived?.Contains("Action=ShowDisplay") == true)
            {
                this.MulticastActionCommand = MulticastCommands.ShowDisplay;
                this.MultiCastUpdateStatus = MultiCastUpdateStatus.Idle; // No need to process the multicast further as we are just showing the display
                this.MulticastReceivedInformation.MulticastMessageReceived = string.Empty;

                if (!this.ShowDisplaySent && this.MulticastConfig.UseWindowsInputSimulator)
                {
                    this.ShowDisplaySent = true;
                    keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_S);
                }
            }
        }



        private MultiCastUpdateStatus SetStaticIpForNetworkAdaptor()
        {
            Logger.Trace("-----------SetStaticIpForNetworkAdaptor--------------------");
            try
            {
                var multicastIpLanAddress = this.MulticastReceivedInformation.MulticastListenerSocketAddress;
                Logger.Trace(" ----->Multicast Response Found at TFT IP Address() {0} ", multicastIpLanAddress);
                var remoteIpAddressWithMcuResponse = this.MulticastReceivedInformation.MulticastReceivedFromIpAddress;
                Logger.Trace(" ----->RemoteIp Address Of Mcu {0} ", remoteIpAddressWithMcuResponse);
                var newLocalIpAddress = NetworkUtils.GetNextFreeIpAddress(NetworkUtils.IncrementIpAddress(remoteIpAddressWithMcuResponse));
                this.localIpAddressToMatchMcuDomain = newLocalIpAddress;
                Logger.Trace(" GetNextFreeIpAddress returned {0} ", newLocalIpAddress);
                var subnetMask = this.MulticastReceivedInformation.MuticastReceivedFromSubnetMask.ToString();
                Logger.Trace(" ----->New subnetMask to use as got multicast {0} ", subnetMask);
                var localLanSubnetMask = NetworkUtils.GetSubnetMaskForIpAddress(multicastIpLanAddress);
                Logger.Trace(" ----->localLanSubnetMask {0} ", localLanSubnetMask);
                Logger.Trace(
                    " ------>Changing local lan IP from {0}:{1} to {2}:{3} for Adaptor name {4} ",
                    multicastIpLanAddress,
                    localLanSubnetMask,
                    newLocalIpAddress,
                    subnetMask,
                    this.MulticastReceivedInformation.MulticastListenerSocketAdaptorName);

                this.OriginalAdaptorNetworkInfo = SaveAdaptorNetworkInfo(this.MulticastReceivedInformation.MulticastListenerSocketAdaptorName,
                    multicastIpLanAddress, localLanSubnetMask);
                if (!newLocalIpAddress.Equals(multicastIpLanAddress) || !localLanSubnetMask.Equals(this.MulticastReceivedInformation.MuticastReceivedFromSubnetMask))
                {
                    NetworkManagement.SetSystemIp(newLocalIpAddress.ToString(), subnetMask, this.MulticastReceivedInformation.MulticastListenerSocketAdaptorName);
                    Logger.Trace("Set System to Static Ip {0}", newLocalIpAddress);
                }
                else
                {
                    Logger.Warn(" Static Ip Is Already Set to {0} should match {1} If not we did not call SetStatic IP ", multicastIpLanAddress, newLocalIpAddress);
                }
                return MultiCastUpdateStatus.StaticIpSet;
            }
            catch (Exception exception)
            {
                Logger.Error(MethodBase.GetCurrentMethod().Name + exception.Message + exception.InnerException?.Message);
                return MultiCastUpdateStatus.ReceivedMcuResponse;
            }
        }

        public bool IsMasterUnit()
        {
            Process[] composerProcesses = Process.GetProcessesByName("Composer");
            var isMasterUnit = composerProcesses.Length > 0;
            Logger.Trace(MethodBase.GetCurrentMethod().Name + $" => Found {composerProcesses.Length} Composer(s) ");
            return isMasterUnit;

        }

        private void Start(IPAddress localIpAddress)
        {
            if (this.LocalIpAddressBoundtoMulticast.Count == 0)
            {
                this.LocalIpAddressBoundtoMulticast = new Stack<IPAddress>(NetworkUtils.GetAllActiveLocalIpAddress());
                Logger.Trace("Local Ip Stack was Empty --- Reconstructing");
            }
            Logger.Trace(this.multicastReceive == null ? "Multicast is Null - Initializing" : "Multicast is not Null - Disposing");
            if (this.multicastReceive != null)
            {
                this.multicastReceive.Dispose();
                this.multicastReceive = null;
            }
            if (this.multicastReceive == null)
            {
                Logger.Trace("Multicast is Null - Initializing");
                // Initialize the multicast address group and multicast port.
                // Both address and port are selected from the allowed sets as
                // defined in the related RFC documents. These are the same 
                // as the values used by the sender.
                this.multicastReceive = new MulticastReceive(this.MulticastConfig.MulticastAddress, this.MulticastConfig.MulticastPort, localIpAddress);
                try
                {
                    // Start a multicast group.
                    this.multicastReceive.StartMulticast(this.MulticastConfig.MulticastAddress, this.MulticastConfig.MulticastPort);

                    // Display MulticastOption properties.
                    this.multicastReceive.MulticastOptionProperties();

                    // Receive broadcast messages.
                    this.multicastReceive.ReceiveBroadcastMessages();
                }
                catch (Exception e)
                {
                    Logger.Trace(
                        MethodBase.GetCurrentMethod().Name + " Note: Multicast can throw exception when networking settings are changing. " + e.Message + e.InnerException?.Message);
                }
            }
        }

        private MultiCastUpdateStatus VerifyStaticIp()
        {
            Logger.Trace("-----------Verify Static Ip was Set --------------------");
            var localAdaptorWithMcuResponse = this.OriginalAdaptorNetworkInfo.Description;
            Logger.Trace("Local Multicast Adaptor Name =" + localAdaptorWithMcuResponse);
            Logger.Trace("local IpAddress was set to {0}, to Match Mcu Domain ", this.localIpAddressToMatchMcuDomain);
            Logger.Trace("local Ip address when queried  = {0}", NetworkUtils.GetIpFromAdaptorDesc(localAdaptorWithMcuResponse));
            if (NetworkUtils.GetIpFromAdaptorDesc(localAdaptorWithMcuResponse).Equals(this.localIpAddressToMatchMcuDomain))
            {
                this.numberOfTimesStaticIpWasSet = 0;
                Logger.Trace($" Static SystemIp Set Test Passed the {this.numberOfTimesStaticIpWasSet} time. ");
                // if(this.CheckForMulticastFtpServer())
                return MultiCastUpdateStatus.StaticIpVerified;

            }
            if (this.numberOfTimesStaticIpWasSet++ > 3)
                return MultiCastUpdateStatus.ErrorDuringMulticast;

            Logger.Warn($" Static System Ip Test Failed {this.numberOfTimesStaticIpWasSet} times: Try again. ");
            return MultiCastUpdateStatus.ReceivedMcuResponse;
        }

 

        private MultiCastUpdateStatus WaitForMcuResponse()
        {
            var localAdaptorWithMcuResponse = this.multicastReceive.MulticastReceivedInformation.MulticastListenerSocketAdaptorName;
            var localIpAddressWithMcuResponse = this.multicastReceive.MulticastReceivedInformation.MulticastListenerSocketAddress;
            if (this.multicastReceive != null && !string.IsNullOrEmpty(localAdaptorWithMcuResponse) && !Equals(localIpAddressWithMcuResponse, IPAddress.None))
            {
                Logger.Trace("Got MCU Response at local Adaptor {0} and Local Ip {1}", localAdaptorWithMcuResponse, localIpAddressWithMcuResponse);
                return MultiCastUpdateStatus.ReceivedMcuResponse;
            }
            Logger.Trace("No MCU Response ......");
            return MultiCastUpdateStatus.Started;
        }


        #endregion

        #region Not Used
        //private bool FtpCreateSlaveFileAtFtp()
        //{
        //    Logger.Trace($"Creating SLAVE ftp file at {this.GetRemoteIpAddressWithMcuResponse()} using {this.MulticastConfig.FtpUsername}: {this.MulticastConfig.FtpPassword}");
        //    try
        //    {
        //        using (var ftp = new FtpUtils(this.GetRemoteIpAddressWithMcuResponse().ToString(), this.MulticastConfig.FtpUsername, this.MulticastConfig.FtpPassword))
        //        {
        //            Logger.Trace($" Ftp connection was successful ");
        //            if (ftp.DeleteAllSlaveFiles())
        //            {
        //                Logger.Trace($" DeleteAllSlaveFiles was sucess ");
        //            }
        //            if (ftp.CreateSlaveFile())
        //            {
        //                Logger.Trace($" FtpCreateSlaveFileAtFtp was sucess ");
        //                return true;
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Trace($" FtpCreateSlaveFileAtFtp threw an exception {e.Message} and {e.InnerException?.Message}");
        //    }
        //    return false;
        //}
        //private MultiCastUpdateStatus FtpDeleteFile()
        //{
        //    this.serverFtpUri = new Uri("ftp://" + this.GetRemoteIpAddressWithMcuResponse() + "/Connected.txt");
        //    using (var ftp = new FtpUtils(this.GetRemoteIpAddressWithMcuResponse().ToString(), this.MulticastConfig.FtpUsername, this.MulticastConfig.FtpPassword))
        //    {
        //        if (ftp.FtpFileDelete(this.serverFtpUri))
        //        {
        //            Logger.Trace("Ftp Deleted Done.");
        //            return MultiCastUpdateStatus.FtpDeleteVerified;
        //        }
        //        Logger.Error(MethodBase.GetCurrentMethod().Name + "Ftp Delete  Failed");
        //        return MultiCastUpdateStatus.ErrorDuringMulticast;
        //    }
        //}

        //private MultiCastUpdateStatus FtpDeleteVerified()
        //{
        //    this.serverFtpUri = new Uri("ftp://" + this.GetRemoteIpAddressWithMcuResponse());
        //    var ftp = new FtpUtils(this.serverFtpUri.ToString(), this.MulticastConfig.FtpUsername, this.MulticastConfig.FtpPassword);
        //    if (ftp.FtpFileUpload(this.MulticastConfig.LocalFilePath, "Complete.txt"))
        //    {
        //        Logger.Trace("Ftp Upload Done");
        //        return MultiCastUpdateStatus.AllDone;
        //    }
        //    Logger.Error("Ftp Upload Failed...");
        //    return MultiCastUpdateStatus.ErrorDuringMulticast;
        //}

        //private MultiCastUpdateStatus FtpUploadMulticastFile()
        //{
        //    this.serverFtpUri = new Uri("ftp://" + this.GetRemoteIpAddressWithMcuResponse());
        //    var ftp = new FtpUtils(this.serverFtpUri.ToString(), this.MulticastConfig.FtpUsername, this.MulticastConfig.FtpPassword);
        //    if (ftp.FtpFileUpload(this.MulticastConfig.LocalFilePath, "Connected.txt"))
        //    {
        //        Logger.Warn("Ftp Upload Done");
        //        return MultiCastUpdateStatus.FtpUploadVerified;
        //    }
        //    Logger.Error("Ftp Upload Failed...");
        //    return MultiCastUpdateStatus.ErrorDuringMulticast;
        //}

        //private MultiCastUpdateStatus FtpUploadVerification()
        //{
        //    this.serverFtpUri = new Uri("ftp://" + this.GetRemoteIpAddressWithMcuResponse() + "/Connected.txt");
        //    using (var ftptempUtils = new FtpUtils(this.GetRemoteIpAddressWithMcuResponse().ToString(), this.MulticastConfig.FtpUsername, this.MulticastConfig.FtpPassword))
        //    {
        //        if (ftptempUtils.FtpFileExists(this.serverFtpUri))
        //        {
        //            Logger.Trace("Ftp Transfer Verified Successfully");
        //            return MultiCastUpdateStatus.FtpDeleteStart;
        //        }
        //        Logger.Error("Ftp Transfer Verification Failed");
        //        return MultiCastUpdateStatus.ErrorDuringMulticast;
        //    }
        //}

        #endregion
    }
}