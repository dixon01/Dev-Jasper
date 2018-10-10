// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Motion.Protran.Arriva;

    /// <summary>
    /// Object tasked to represents the Arriva remote TCP/IP server.
    /// </summary>
    public class ArrivaServer
    {
        /// <summary>
        /// The port to use to listen for incoming TCP/IP connections.
        /// </summary>
        private readonly int portNumber;

        /// <summary>
        /// The object tasked to produce random transaction ID.
        /// </summary>
        private readonly Random randomizer;

        /// <summary>
        /// The buffer used by "Protran" to communicate with this TCP/IP server.
        /// </summary>
        private readonly byte[] protranBuffer;

        /// <summary>
        /// The function invoked asynchronously by the O.S. whenever a remote TCP/IP
        /// tries to connect itself whit our TCP/IP server.
        /// </summary>
        private readonly AsyncCallback acceptFunction;

        /// <summary>
        /// The object tasked to calculate the CRC for an Arriva's telegram.
        /// </summary>
        private readonly Crc crcCalculator;

        /// <summary>
        /// The socket tasked to represent as the ISI Copilot TCP/IP server.
        /// </summary>
        private Socket serverSocket;

        /// <summary>
        /// The socket tasked to represent Protran.
        /// </summary>
        private Socket protranSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaServer"/> class.
        /// with the default listening port number (35001).
        /// </summary>
        public ArrivaServer()
            : this(35001)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaServer"/> class.
        /// </summary>
        /// <param name="portNumber">The TCP/IP server listening port number.</param>
        public ArrivaServer(int portNumber)
        {
            this.ErrorCount = 0;
            this.portNumber = portNumber;
            this.crcCalculator = new Crc();
            this.randomizer = new Random();
            this.protranBuffer = new byte[4096];
            this.acceptFunction = this.OnAccept;
        }

        /// <summary>
        /// Gets the current amount of errors.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Starts the TCP/IP listener in order to accept TCP/IP client (like protran).
        /// </summary>
        public void Start()
        {
            // now I'll do the necessaire to start the TCP/IP server.
            var ipe = new IPEndPoint(IPAddress.Any, this.portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ipe);
            this.serverSocket.Listen(5);
            this.serverSocket.BeginAccept(this.acceptFunction, null);
            this.serverSocket.LingerState.Enabled = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.protranSocket != null)
            {
                this.protranSocket.Close();
                this.protranSocket = null;
            }

            if (this.serverSocket != null)
            {
                this.serverSocket.Close(5);
                this.serverSocket = null;
            }
        }

        /// <summary>
        /// Sends the 'Hello' message to the TCP/IP client.
        /// </summary>
        /// <param name="arrivaHeaderData">The arriva header data.</param>
        private void SendHello(ArrivaHeaderData arrivaHeaderData)
        {
            // yes, I'm really connected.
            if (arrivaHeaderData == null)
            {
                // invalid data. I don't send nothing.
                return;
            }

            // everything seems ok. let's try to send the data.
            byte[] buf =
            {
                0x64, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00
            };

            buf[8] = arrivaHeaderData.MessageTransactionId1;
            buf[9] = arrivaHeaderData.MessageTransactionId2;
            buf[10] = arrivaHeaderData.MessageTransactionId3;
            buf[11] = arrivaHeaderData.MessageTransactionId4;

            var mycrc = this.crcCalculator.CalculateCrc(buf, 30);
            buf[30] = (byte)((mycrc & 0xFF00) >> 8); // HighByte
            buf[31] = (byte)(mycrc & 0x00FF); // LowByte

            try
            {
                this.protranSocket.Send(buf, buf.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}{1}{2}", "Error on sending data (", e.Message, ")");
            }
        }

        /// <summary>
        /// The function invoked asynchronously by the O.S. whenever a remote TCP/IP
        /// tries to connect itself whit our TCP/IP server.
        /// </summary>
        /// <param name="ar">
        /// The object containing the socket associated to the remote TCP/IP client.
        /// </param>
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                var clientSocket = this.serverSocket.EndAccept(ar);
                if (!clientSocket.Connected)
                {
                    return;
                }

                // ok, I can say that I'm really connected with Protran.
                this.protranSocket = clientSocket;
                this.protranSocket.BeginReceive(
                    this.protranBuffer,
                    0,
                    this.protranBuffer.Length,
                    0,
                    this.ReadCallback,
                    null);

                // now I get ready for next incoming connection.
                this.serverSocket.BeginAccept(this.acceptFunction, null);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}{1}{2}", "Error on accepting connection (", e.Message, ")");
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = this.protranSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var arrivaHeader = new ArrivaHeaderData();
                    arrivaHeader.SetMessageTransactionId(this.randomizer.Next());
                    this.SendHello(arrivaHeader);
                    this.protranSocket.BeginReceive(
                        this.protranBuffer, 0, this.protranBuffer.Length, 0, this.ReadCallback, null);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}{1}{2}", "Error on reading data (", e.Message, ")");
            }
        }
    }
}
