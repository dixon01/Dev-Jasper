namespace Luminator.Multicast.Core
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;

    using Gorba.Common.Utility.Core;

    using NLog;

    public class MulticastReceive : IMulticastReceive
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<MulticastReceive>();

        #endregion

        #region Fields

        private IPAddress mcastAddress;

        private MulticastOption mcastOption;

        private int mcastPort;

        private Socket mcastSocket;

     

        #endregion

        #region Constructors and Destructors

        public MulticastReceive(IPAddress mcastAddress, int mcastPort, IPAddress ipAddress)
        {
            this.mcastAddress = mcastAddress;
            this.mcastPort = mcastPort;
            this.MulticastAtLocalIpAddress = ipAddress;
            this.ReceiveBroadcastMessagesDone = false;
            this.McuResponseReceivedAtLocalIpAddress = null;
            this.MulticastReceivedInformation = new MulticastInfo();
        }

        #endregion

        #region Public Properties

        public EndPoint McuResponsedRemoteIp { get; set; }

        public IPAddress McuRespondedAtSubnetMask
        {
            get
            {
                var subnetMask = IPAddress.Any;
                Logger.Trace("Getter McuRespondedAtSubnetMask Multicast Message is =[{0}]", this.MulticastMessageReceived);
                try
                {
                    if (this.MulticastMessageReceived.Contains("Action=Update"))
                    {
                        const string Netmask = "NetMask=";
                        var idx = this.MulticastMessageReceived.IndexOf(Netmask, StringComparison.Ordinal) + Netmask.Length;
                        if (idx > 0)
                        {
                            var ipString = this.MulticastMessageReceived.Substring(idx, this.MulticastMessageReceived.Length - idx);
                            Logger.Trace($"Getting SubnetMask ....");
                            subnetMask = IPAddress.Parse(ipString);
                            Logger.Trace($"Got SubnetMask Successfully: {subnetMask}");
                        }
                    }
                    else
                    {
                        Logger.Trace(MethodBase.GetCurrentMethod().Name + "  Subnet Mask Not Processed as action was not Update." );
                    }
                }
                catch (Exception e)
                {
                    Logger.Trace(MethodBase.GetCurrentMethod().Name + e.Message + e.InnerException?.Message);
                }
                return subnetMask;
            }
          
        }

        private string McuResponseReceivedAtAdaptor { get; set; }

        private IPAddress McuResponseReceivedAtLocalIpAddress { get; set; }

        private IPAddress MulticastAtLocalIpAddress { get; set; }

        private bool ReceiveBroadcastMessagesDone { get; set; }

        private string MulticastMessageReceived { get; set; }

        public MulticastInfo MulticastReceivedInformation { get; set; }

        #endregion

        #region Public Methods and Operators

        public void CloseMultiCastSocket()
        {
            if (this.mcastSocket != null)
            {
                this.mcastSocket.Close();
                this.mcastSocket = null;
            }
        }

        public void Dispose()
        {
            this.CloseMultiCastSocket();
            Logger.Trace("Disposed MulticastReceive");
        }

        public void MulticastOptionProperties()
        {
            if (this.mcastOption != null)
            {
                Logger.Trace("Current multicast group is: " + this.mcastOption.Group);
                Logger.Trace("Current multicast local address is: " + this.mcastOption.LocalAddress);
                Logger.Trace("Current multicast remote IP is: " + this.McuResponsedRemoteIp.GetIpAddress());
            }
        }

        //Receive
        public void ReceiveBroadcastMessages()
        {
            if (this.mcastAddress == null)
            {
                Logger.Trace("mcastAddress was null - bailing from ReceiveBroadcastMessages");
                return;
            }
            var bytes = new byte[1024];
            var groupEp = new IPEndPoint(this.mcastAddress, this.mcastPort);
            EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (!this.ReceiveBroadcastMessagesDone)
                {
                    Logger.Trace("...............Waiting for multicast packets.......");
                   
                    int bytesRead = this.mcastSocket.ReceiveFrom(bytes, ref remoteEp);
                    this.McuResponseReceivedAtLocalIpAddress = ((IPEndPoint)this.mcastSocket.LocalEndPoint).Address;
                    Logger.Trace($"Listening for multicast at: {this.McuResponseReceivedAtLocalIpAddress}");
                    this.McuResponseReceivedAtAdaptor = NetworkUtils.GetAdapterDescFromIp4Address(this.McuResponseReceivedAtLocalIpAddress);
                    Logger.Trace($"Adaptor Name: {this.McuResponseReceivedAtAdaptor}");

                    this.McuResponsedRemoteIp = remoteEp;
                    Logger.Trace("RX from {0}", remoteEp);
                    Logger.Trace($"McuResponsedRemoteIp: {this.McuResponsedRemoteIp.GetIpAddress()}");
                    var messageReceived = ProcessTheReceivedMessage(bytes);
                   
                    if (!string.IsNullOrEmpty(messageReceived)) this.MulticastMessageReceived = messageReceived;
                    this.MulticastReceivedInformation = new MulticastInfo(this.McuResponseReceivedAtAdaptor,
                       this.McuResponseReceivedAtLocalIpAddress,messageReceived,this.McuResponsedRemoteIp.GetIpAddress(),
                       this.McuRespondedAtSubnetMask);
                    Logger.Trace(".................Received broadcast from {0} \n WholeMessage: [{1}] Bytes Read:{2} \n", groupEp, this.MulticastMessageReceived, bytesRead );
                }

            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Logger.Trace("ReceiveBroadcastMessages Ended, Closing Socket(s) Exception is OK \n " + objectDisposedException.Message);
            }
            catch (SocketException socketException)
            {
                Logger.Trace("ReceiveBroadcastMessages Ended, Closing Socket(s) Exception is OK \n " + socketException.Message);
            }
            catch (Exception e)
            {
                Logger.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name + 
                    " Note: Bind may throw an exception if called too early: Exception is OK \n" + 
                    e.Message  + e.InnerException?.Message);
            }
        }

        public static string ProcessTheReceivedMessage(byte [] bytes)
        {
            try
            {
                Logger.Trace($"Data In Bytes {BitConverter.ToString(bytes)} ");
                var bytesWithoutNull = bytes.TakeWhile(m => !m.Equals(0x00)).ToArray();
                var s = Encoding.UTF8.GetString(bytesWithoutNull);
                Logger.Trace($" {MethodBase.GetCurrentMethod().Name}:  {s} ");
                return s;
            }
            catch (Exception e)
            {
                Logger.Trace(e.Message + e.InnerException?.Message);
            }
            Logger.Trace($" {MethodBase.GetCurrentMethod().Name}:  Returning Empty");
            return string.Empty;

        }

        //Receive
        public void StartMulticast(IPAddress mcastAddressIn, int mcastPortIn)
        {
            this.mcastAddress = mcastAddressIn;
            this.mcastPort = mcastPortIn;
            try
            {
                this.mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                EndPoint localEp = new IPEndPoint(this.MulticastAtLocalIpAddress, this.mcastPort);

                // Define a MulticastOption object specifying the multicast group 
                // address and the local IPAddress.
                // The multicast group address is the same as the address used by the server.
                this.mcastOption = new MulticastOption(this.mcastAddress, this.MulticastAtLocalIpAddress);

                this.mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, this.mcastOption);

                this.mcastSocket.Bind(localEp);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Logger.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name + objectDisposedException.Message + objectDisposedException.InnerException?.Message);
            }
            catch (Exception exception)
            {
                Logger.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name + exception.Message + exception.InnerException?.Message );
            }
        }

        #endregion
    }
}