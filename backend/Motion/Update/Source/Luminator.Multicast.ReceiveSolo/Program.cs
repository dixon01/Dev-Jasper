// This is the listener example that shows how to use the MulticastOption class. 
// In particular, it shows how to use the MulticastOption(IPAddress, IPAddress) 
// constructor, which you need to use if you have a host with more than one 
// network card.
// The first parameter specifies the multicast group address, and the second 
// specifies the local address of the network card you want to use for the data
// exchange.
// You must run this program in conjunction with the sender program as 
// follows:
// Open a console window and run the listener from the command line. 
// In another console window run the sender. In both cases you must specify 
// the local IPAddress to use. To obtain this address run the ipconfig comand 
// from the command line. 
//  

namespace Luminator.Multicast.ReceiveSolo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Timers;

    using Luminator.Multicast.Core;
    using Luminator.Multicast.ReceiveSolo.Properties;

    public class TestMulticastOption
    {
        #region Static Fields

        private static readonly Timer ProgramStepTimer = new Timer(Settings.Default.ProgramStepInterval);

        private static MulticastManager multicastManager;

        #endregion

        #region Public Properties

        public static bool FtpDeleteDone { get; set; }

        public static bool FtpDeleteVerified { get; set; }

        public static string FtpPassword { get; set; }

        public static bool FtpTransferVerified { get; set; }

        public static bool FtpUploadDone { get; set; }

        public static string FtpUsername { get; set; }

        public static string LocalFilePath { get; set; }

        public static bool StaticSystemIpSetTestPassed { get; set; }

        #endregion

        #region Properties

        private static bool AllWorkComplete { get; set; }

        private static Stack<IPAddress> LocalIpAddressBoundtoMulticast { get; set; }

        private static bool StaticSystemIpSet { get; set; }

        private static IConsoleMessage consoleMessage;

        #endregion

        #region Public Methods and Operators

        public static void Main(string[] args)
        {
            ProgramStepTimer.Start();
            ProgramStepTimer.Elapsed += ProgramStepTimerElapsed;
            FtpUsername = Settings.Default.FtpUserName;
            FtpPassword = Settings.Default.FtpPassword;
            LocalFilePath = ".\\Resources\\Facade.txt";
            consoleMessage = new ConsoleMessage();
            if (!File.Exists(LocalFilePath))
            {
                consoleMessage.Error(LocalFilePath + " <= File for Ftp not Found \n Aborting");
                Console.ReadKey();
                Environment.Exit(0);
            }

            LocalIpAddressBoundtoMulticast = new Stack<IPAddress>(NetworkUtils.GetAllActiveLocalIpAddress());
            Console.WriteLine("Starting application please wait....");
            NetworkUtils.GetAllLocalIpAddress().ForEach(x => Console.WriteLine(x + " : " + NetworkUtils.GetAdapterDescFromIp4Address(x)));

            Console.ReadKey();
        }

        #endregion

        #region Methods

        private static void ProgramStepTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (AllWorkComplete )
            {
                consoleMessage.Success("All Work Complete");
                AllWorkComplete = false;
                StaticSystemIpSet = false;
                StaticSystemIpSetTestPassed = false;
                FtpUploadDone = false;
                FtpTransferVerified = false;
                FtpDeleteDone = false;
                FtpDeleteVerified = false;
                return;
            }
           
            

            if (multicastManager == null || Equals(multicastManager.GetLocalIpAddressWithMcuResponse(), IPAddress.None))
            {
                consoleMessage.Highlight(DateTime.Now + " STEP 1: Looking for Multicast From MCU ==================");
                if (multicastManager == null)
                {
                    multicastManager = new MulticastManager();
                    multicastManager.Start( IPAddress.Parse(Settings.Default.MulticastIp), Settings.Default.MulticastPort, LocalIpAddressBoundtoMulticast.Pop());
                }
                else
                {
                    multicastManager.Start(IPAddress.Parse(Settings.Default.MulticastIp), Settings.Default.MulticastPort, LocalIpAddressBoundtoMulticast.Pop());
                }
            }

            if (multicastManager != null && !Equals(multicastManager.GetLocalIpAddressWithMcuResponse(), IPAddress.None) && !StaticSystemIpSet)
            {
                consoleMessage.Highlight(DateTime.Now + " STEP 2: Set the System Ip from MCU response ==================");
                if (!NetworkUtils.GetAllLocalIpAddress().Contains(NetworkUtils.IncrementIpAddress(multicastManager.GetRemoteIpAddressWithMcuResponse())))
                {
                    NetworkManagement.SetSystemIp(
                        NetworkUtils.IncrementIpAddress(multicastManager.GetRemoteIpAddressWithMcuResponse()).ToString(),
                        "255.255.255.0",
                        multicastManager.GetLocalAdaptorWithMcuResponse());
                    consoleMessage.Success("Set System to Static Ip ");
                }
                else
                {
                    consoleMessage.Warning(" Static Ip Is Already Set to " + NetworkUtils.IncrementIpAddress(multicastManager.GetRemoteIpAddressWithMcuResponse()));
                }
                StaticSystemIpSet = true;
                return;
            }
            if (StaticSystemIpSet && !StaticSystemIpSetTestPassed)
            {
                consoleMessage.Highlight(DateTime.Now + " STEP 3: Verify System IP was Set ==================");
                if (multicastManager != null)
                {
                    Console.WriteLine("Remote Ip" + multicastManager.GetRemoteIpAddressWithMcuResponse());
                    Console.WriteLine("Local Multicast Adaptor" + multicastManager.GetLocalAdaptorWithMcuResponse());
                    Console.WriteLine("Local Static Ip Was Set To : " + multicastManager.GetLocalAdaptorWithMcuResponse());
                    var adapterDescFromIp4Address = NetworkUtils.GetAdapterDescFromIp4Address(multicastManager.GetLocalIpAddressWithMcuResponse());
                    if (multicastManager.GetLocalAdaptorWithMcuResponse().Equals(adapterDescFromIp4Address))
                    {
                        StaticSystemIpSetTestPassed = true;
                        consoleMessage.Success(" Static SystemIp Set Test Passed ");
                        return;
                    }
                    else
                    {
                        consoleMessage.Error(" Static System Ip Test Failed");
                    }
                }
            }

            if (StaticSystemIpSetTestPassed && !FtpUploadDone)
            {
                consoleMessage.Highlight(DateTime.Now + " STEP 4: FTP send a file called Connected.txt ==================");
                if (multicastManager != null)
                {
                    var ftp = new FtpUtils(multicastManager.GetRemoteIpAddressWithMcuResponse().ToString(), FtpUsername, FtpPassword);
                    if (ftp.FtpFileUpload(LocalFilePath, "Connected.txt"))
                    {
                        FtpUploadDone = true;
                        consoleMessage.Success("Ftp Upload Done");
                        return;
                    }
                    else
                    {
                        consoleMessage.Error("Ftp Upload Failed...");
                    }
                }
            }

            if (multicastManager != null)
            {
                var serverFtpUri = new Uri("ftp://" + multicastManager.GetRemoteIpAddressWithMcuResponse() + "/Connected.txt");
                if (FtpUploadDone && multicastManager != null && !FtpTransferVerified)
                {
                    consoleMessage.Highlight(DateTime.Now + " STEP 5: Transfered File Exists On Ftp \n" + serverFtpUri);
                    var ftp = new FtpUtils(multicastManager.GetRemoteIpAddressWithMcuResponse().ToString(), FtpUsername, FtpPassword);

                    if (ftp.FtpFileExists(serverFtpUri))
                    {
                        consoleMessage.Success("Ftp Transfer Verified Successfully");
                        FtpTransferVerified = true;
                        return;
                    }
                    else
                    {
                        consoleMessage.Error("Ftp Transfer Verification Failed");
                    }
                }

                if (FtpTransferVerified && multicastManager != null && !FtpDeleteDone)
                {
                    consoleMessage.Highlight(DateTime.Now + " STEP 6: Delete the Transfered File From Ftp " + serverFtpUri);
                    var ftp = new FtpUtils(multicastManager.GetRemoteIpAddressWithMcuResponse().ToString(), FtpUsername, FtpPassword);

                    if (ftp.FtpFileDelete(serverFtpUri))
                    {
                        consoleMessage.Success("Ftp Delete of file Done Successfully");
                        FtpDeleteDone = true;
                        return;
                    }
                    else
                    {
                        consoleMessage.Error("Ftp Delete Failed");
                    }
                }

                if (FtpDeleteDone && multicastManager != null && !FtpDeleteVerified)
                {
                    consoleMessage.Highlight(DateTime.Now + " STEP 7: Verify FTP Deleted File No Longer Exists on the Ftp " + serverFtpUri);
                    var ftp = new FtpUtils(multicastManager.GetRemoteIpAddressWithMcuResponse().ToString(), FtpUsername, FtpPassword);

                    if (!ftp.FtpFileExists(serverFtpUri))
                    {
                        consoleMessage.Success("Ftp Deleted Verified Successfully.");
                        FtpDeleteVerified = true;
                        return;
                    }
                    else
                    {
                        consoleMessage.Error("Ftp Delete Verification Failed");
                    }
                }
            }

            if (FtpDeleteVerified)
            {
                AllWorkComplete = true;
            }
        }

      

        #endregion
    }
}