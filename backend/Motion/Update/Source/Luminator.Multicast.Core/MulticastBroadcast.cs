namespace Luminator.Multicast.Core
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Gorba.Common.Utility.Core;

    using NLog;

    public class MulticastBroadcast : IMulticastBroadcast, IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<MulticastReceive>();

        #endregion

        #region Fields

        private IPAddress mcastAddress;

        private int mcastPort;

        private Socket mcastSocket;

        #endregion

        #region Public Methods and Operators

        public void BroadcastMessage(IPAddress mcastAddressIn, int mcastPortIn, string message)
        {
            this.mcastAddress = mcastAddressIn;
            this.mcastPort = mcastPortIn;
            this.BroadcastMessage(message);
        }

        public void BroadcastMessage(string message)
        {
            try
            {
                //Send multicast packets to the listener.
                var endPoint = new IPEndPoint(this.mcastAddress, this.mcastPort);
                this.mcastSocket?.SendTo(Encoding.ASCII.GetBytes(message), endPoint);
                Logger.Info("Multicast data sent => " + message);
            }
            catch (Exception e)
            {
                Logger.Warn("\n" + e);
            }
        }

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
            Logger.Info("Disposed Multicast Receive");
        }

        // Broadcast
        public void JoinMulticastGroup()
        {
            try
            {
                // Create a multicast socket.
                this.mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Get the local IP address used by the listener and the sender to
                // exchange multicast messages. 
                var localIpAddr = NetworkUtils.GetLocalIpAddress();
                Console.WriteLine("Local IP => " + localIpAddr);
                // Create an IPEndPoint object. 
                // ReSharper disable once InconsistentNaming
                var IPlocal = new IPEndPoint(localIpAddr, 0);

                // Bind this endpoint to the multicast socket.
                this.mcastSocket.Bind(IPlocal);

                // Define a MulticastOption object specifying the multicast group 
                // address and the local IP address.
                // The multicast group address is the same as the address used by the listener.
                var multicastOption = new MulticastOption(this.mcastAddress, localIpAddr);

                this.mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
            }
            catch (Exception e)
            {
                Logger.Error("\n" + e);
            }
        }

        #endregion
    }
}