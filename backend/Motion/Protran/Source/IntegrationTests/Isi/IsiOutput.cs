// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiOutput.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Isi
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using Gorba.Common.Protocols.Isi;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Object tasked to act as the INIT ISI board computer
    /// (to send/receive ISI XML items).
    /// </summary>
    public class IsiOutput
    {
        /// <summary>
        /// The port to use to listen for incoming TCP/IP connections.
        /// </summary>
        private readonly int portNumber;

        /// <summary>
        /// The function invoked asynchronously by the O.S. whenever a remote TCP/IP
        /// tries to connect itself whit our TCP/IP server.
        /// </summary>
        private readonly AsyncCallback acceptFunction;

        /// <summary>
        /// Asynchronous queue used to send ISI messages to Protran from this TCP/IP server.
        /// </summary>
        private readonly ProducerConsumerQueue<IsiMessageBase> sendQueue;

        /// <summary>
        /// The dictionary tasked to contain the ID used by Protran for its "get" requests.
        /// </summary>
        private readonly Dictionary<string, int> isiGetsReceivedWithId;

        /// <summary>
        /// Object tasked to parse the information exchanged
        /// with the remote ISI TCP/IP server.
        /// </summary>
        private IsiSerializer isiSerializerForProtran;

        /// <summary>
        /// The socket tasked to represent as the ISI Copilot TCP/IP server.
        /// </summary>
        private Socket serverSocket;

        /// <summary>
        /// Flags that tells if this TCP/IP server has to be stopped or not.
        /// </summary>
        private bool stop;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiOutput"/> class.
        /// with the default listening port number (51001).
        /// </summary>
        public IsiOutput()
            : this(51001)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiOutput"/> class.
        /// </summary>
        /// <param name="portNumber">The TCP/IP server listening port number.</param>
        public IsiOutput(int portNumber)
        {
            this.stop = false;
            this.ErrorCount = 0;            
            this.portNumber = portNumber;
            this.acceptFunction = this.OnAccept;
            this.isiSerializerForProtran = null;
            this.IsiExpectations = new List<IsiMessageBase>();
            this.isiGetsReceivedWithId = new Dictionary<string, int>();
            this.sendQueue = new ProducerConsumerQueue<IsiMessageBase>(this.WriteMessage, 10);
        }

        /// <summary>
        /// Gets the current amount of errors.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Gets the list of all the ISI items that this TCP/IP expects to receive from Protran.
        /// </summary>
        public List<IsiMessageBase> IsiExpectations { get; private set; }

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
        /// Converts an array of bytes to an hexadecimal string.
        /// </summary>
        /// <param name="buffer">The buffer to convert.</param>
        /// <returns>The hexadecimal representation of the incoming byte array.</returns>
        public string FromByteArrayToHexString(byte[] buffer)
        {
            return Core.Buffers.BufferUtils.FromByteArrayToHexString(buffer);
        }

        /// <summary>
        /// Converts an hexadecimal string to a byte array.
        /// </summary>
        /// <param name="hexadecimalString">The hexadecimal string to be converted.</param>
        /// <returns>The byte array representation of the incoming hexadecimal string.</returns>
        public byte[] FromHexStringToByteArray(string hexadecimalString)
        {
            return Core.Buffers.BufferUtils.FromHexStringToByteArray(hexadecimalString);
        }

        /// <summary>
        /// Gets the ID to be used for an ISI put to be sent to Protran,
        /// depending on the set of items previously received from it.
        /// </summary>
        /// <param name="isiGetItemsUsed">
        /// The ISI get items string received previously from Protran.
        /// </param>
        /// <returns>
        /// Returns the ID for the specific request.
        /// </returns>
        public int GetIdForGetRequest(string isiGetItemsUsed)
        {
            int id;
            bool ok = this.isiGetsReceivedWithId.TryGetValue(isiGetItemsUsed, out id);
            id = ok ? id : -1;
            return id;
        }

        /// <summary>
        /// Orders to our TCP/IP server to send an ISI message to the connected TCP/IP clients.
        /// </summary>
        /// <param name="message">The isi message to be sent to the connected TCP/IP clients.</param>
        public void SendIsiMessageToClient(IsiMessageBase message)
        {
            this.sendQueue.Enqueue(message);
        }

        /// <summary>
        /// Sets an expectation to this TCP/IP server.
        /// Should be invoked immediately before sending an ISI get or ISI put to Protran.
        /// </summary>
        /// <param name="isiMessage">
        /// The ISI item (get or put) that this TCP/IP server should receive from Protran.
        /// </param>
        public void ExpectIsiMessage(IsiMessageBase isiMessage)
        {
            this.IsiExpectations.Add(isiMessage);
        }

        /// <summary>
        /// Performs application-defined tasks associated with
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.stop = true;
            this.sendQueue.StopConsumer();
            this.serverSocket.Close(5);
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

                var inputStream = new NetworkStream(clientSocket);
                var outputStream = new NetworkStream(clientSocket);
                this.isiSerializerForProtran = new IsiSerializer
                {
                    Input = inputStream,
                    Output = outputStream
                };

                var reader = new Thread(this.Read) { IsBackground = true };
                reader.Start();

                this.sendQueue.StartConsumer();

                // now I get ready for next incoming connection.
                this.serverSocket.BeginAccept(this.acceptFunction, null);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(string.Format("{0}{1}{2}", "Error on accepting connection (", e.Message, ")"));
            }
        }

        /// <summary>
        /// Reads ISI messages
        /// </summary>
        private void Read()
        {
            try
            {
                while (!this.stop)
                {
                    var message = this.isiSerializerForProtran.Deserialize();
                    Console.WriteLine("{0} received", message.GetType().Name);
                    var putReceived = message as IsiPut;
                    var getReceived = message as IsiGet;

                    // now I check if the ISI message just received is inside the expectations.
                    int initialExpectationsCount = this.IsiExpectations.Count;
                    var removed = new List<IsiMessageBase>();

                    this.ManageMessageAsPut(putReceived, ref removed);
                    this.ManageMessageAsGet(getReceived, ref removed);
                    int totRemoved = initialExpectationsCount - this.IsiExpectations.Count;

                    // if we didn't removed nothing, it means that we have received
                    // an IsiMessage without expecting it.
                    this.ErrorCount = totRemoved > 0 ? this.ErrorCount : this.ErrorCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error while reading ISI messages: {0}", ex));
            }
        }

        private void ManageMessageAsPut(IsiPut putReceived, ref List<IsiMessageBase> removed)
        {
            if (putReceived == null)
            {
                // invalid put.
                return;
            }

            foreach (var isiMessage in this.IsiExpectations)
            {
                var expectedPut = isiMessage as IsiPut;
                if (expectedPut != null && putReceived.IsiId == expectedPut.IsiId)
                {
                    bool found = true;
                    foreach (var item in expectedPut.Items)
                    {
                        DataItem receivedItem =
                            putReceived.Items.Find(i => i.Name.Equals(item.Name) && i.Value.Equals(item.Value));
                        if (receivedItem == null)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        removed.Add(expectedPut);
                    }
                }
            }

            foreach (var isiMessageToBeRemoved in removed)
            {
                this.IsiExpectations.Remove(isiMessageToBeRemoved);
            }
        }

        private void ManageMessageAsGet(IsiGet getReceived, ref List<IsiMessageBase> removed)
        {
            if (getReceived == null)
            {
                // invalid get.
                return;
            }

            foreach (var isiMessage in this.IsiExpectations)
            {
                var expectedGet = isiMessage as IsiGet;
                if (expectedGet == null)
                {
                    continue;
                }

                bool found = true;
                string[] tokens = expectedGet.Items.ToString().Split(
                    new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in tokens)
                {
                    if (!getReceived.Items.Contains(item))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    removed.Add(expectedGet);
                }

                if (found && getReceived.IsiId != -1)
                {
                    this.isiGetsReceivedWithId.Add(getReceived.Items.ToString(), getReceived.IsiId);
                }
            }

            foreach (var isiMessageToBeRemoved in removed)
            {
                this.IsiExpectations.Remove(isiMessageToBeRemoved);
            }
        }

        private void WriteMessage(IsiMessageBase message)
        {
            try
            {
                this.isiSerializerForProtran.Serialize(message);
                Console.WriteLine(string.Format("Sent ISI Message to Protran: {0}", message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error while sending an ISI message: {0}", ex));
            }
        }
    }
}