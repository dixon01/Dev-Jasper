// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="RecieveNetworkMulticast.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace MulticastUpdateTest
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>The network multicast option.</summary>
    public class RecieveNetworkMulticast
    {
        #region Fields

        private readonly IPAddress mcastAddress;

        private MulticastOption mcastOption;

        private readonly int mcastPort;

        private Socket mcastSocket;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="RecieveNetworkMulticast"/> class.</summary>
        /// <param name="mcastAddress">The mcast address.</param>
        /// <param name="port">The port.</param>
        public RecieveNetworkMulticast(IPAddress mcastAddress, int port = 31000)
        {
            this.mcastAddress = mcastAddress;
            this.mcastPort = port;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            // Initialize the multicast address group and multicast port.
            // Both address and port are selected from the allowed sets as
            // defined in the related RFC documents. These are the same 
            // as the values used by the sender.
            var mcastAddress = IPAddress.Parse("239.255.100.2");
            var mcastPort = 31000;

            Console.Write("Enter the local IP address: ");
            var localIPAddr = IPAddress.Parse(Console.ReadLine());

            var recieveNetworkMulticast = new RecieveNetworkMulticast(mcastAddress, mcastPort);
            recieveNetworkMulticast.Start(localIPAddr);
        }

        /// <summary>The receive broadcast messages.</summary>
        public void ReceiveBroadcastMessages()
        {
            bool done = false;
            byte[] bytes = new byte[100];
            IPEndPoint groupEP = new IPEndPoint(mcastAddress, mcastPort);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for multicast packets.......");
                    Console.WriteLine("Enter ^C to terminate.");

                    mcastSocket.ReceiveFrom(bytes, ref remoteEP);
                    Console.WriteLine("RX from {0}", remoteEP);
                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", groupEP, Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }

                mcastSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        #region Methods

        private void MulticastOptionProperties()
        {
            Console.WriteLine("Current multicast group is: " + mcastOption.Group);
            Console.WriteLine("Current multicast local address is: " + mcastOption.LocalAddress);
        }

        private void Start(IPAddress localIPAddress)
        {
            // Start a multicast group.
            StartMulticast(localIPAddress);

            // Display MulticastOption properties.
            MulticastOptionProperties();

            // Receive broadcast messages.
            ReceiveBroadcastMessages();
        }

        private void StartMulticast(IPAddress localIPAddr)
        {
            try
            {
                this.mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

      
               

                // IPAddress localIP = IPAddress.Any;
                EndPoint localEp = new IPEndPoint(localIPAddr, this.mcastPort);

                this.mcastSocket.Bind(localEp);

                // Define a MulticastOption object specifying the multicast group 
                // address and the local IPAddress.
                // The multicast group address is the same as the address used by the server.
                this.mcastOption = new MulticastOption(this.mcastAddress, localIPAddr);

                this.mcastSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, this.mcastOption);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion
    }
}