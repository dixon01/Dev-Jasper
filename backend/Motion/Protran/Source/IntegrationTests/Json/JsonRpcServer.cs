// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using Gorba.Motion.Common.Mgi.AtmelControl;
    using Gorba.Motion.Common.Mgi.AtmelControl.Args;
    using Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc;

    /// <summary>
    /// Wrapper around JSON-RPC to send telegrams
    /// </summary>
    public class JsonRpcServer
    {
        private readonly int portNumber;

        private readonly List<JSonClient> jsonClients;

        private TcpListener listener;

        private TcpClient tcpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcServer"/> class.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public JsonRpcServer(int port)
        {
            this.portNumber = port;
            this.jsonClients = new List<JSonClient>();
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.Connect();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.listener != null)
            {
                this.listener.Stop();
                this.listener = null;
            }
        }

        /// <summary>
        /// The send telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        public void SendTelegram(string telegram)
        {
            foreach (var client in this.jsonClients)
            {
                client.SendTelegram(telegram);
            }
        }

        /// <summary>
        /// The converter from hex to ASCII.
        /// </summary>
        /// <param name="hex">
        /// The hex value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string FromHexToAscii(string hex)
        {
            var data = new StringBuilder();
            for (int i = 0; i <= hex.Length - 2; i += 2)
            {
                data.Append(Convert.ToString(Convert.ToChar(int.Parse(hex.Substring(i, 2), NumberStyles.HexNumber))));
            }

            return data.ToString();
        }

        /// <summary>
        /// The start.
        /// </summary>
        private void Connect()
        {
            var ipe = new IPEndPoint(IPAddress.Any, this.portNumber);
            this.listener = new TcpListener(ipe);
            this.listener.Start();
            this.listener.BeginAcceptTcpClient(this.CreateClient, null);
        }

        private void CreateClient(IAsyncResult ar)
        {
            if (this.listener != null)
            {
                this.tcpClient = this.listener.EndAcceptTcpClient(ar);
                Console.WriteLine("Found a tcpclient");
                var client = new JSonClient(this.tcpClient);
                client.Start();
                this.jsonClients.Add(client);
                this.listener.BeginAcceptTcpClient(this.CreateClient, null);
            }
        }

        /// <summary>
        /// The j son client.
        /// </summary>
        private class JSonClient : JsonRpcConnectionBase
        {
            private readonly TcpClient tcpClient;

            private string currentTelegram;

            private int ibisAddress;

            /// <summary>
            /// Initializes a new instance of the <see cref="JSonClient"/> class.
            /// </summary>
            /// <param name="client">
            /// The client.
            /// </param>
            public JSonClient(TcpClient client)
            {
                this.tcpClient = client;
                this.ibisAddress = 8;
            }

            public void Start()
            {
                this.Configure();
                this.Start(this.tcpClient);
            }

            public void SendTelegram(string telegram)
            {
                this.currentTelegram = telegram;
                var notification = new RpcNotification();
                notification.method = "notifyObject";
                notification.@params = this.CreateIbisStream();
                this.SendObject(notification);
            }

            private void Configure()
            {
                this.AddLocalMethod("registerObject", this.RegisterObject);
                this.AddLocalMethod("ibisSetAddress", this.IbisSetAddress);
                this.AddLocalMethod("ibisSetReplyValue", this.IbisSetReplyValue);
            }

            private object IbisSetAddress(RpcRequest request)
            {
                var args = request.GetParams<IbisSetAddressArgs>();
                this.ibisAddress = args.Address;
                return 0;
            }

            private object IbisSetReplyValue(RpcRequest request)
            {
                var args = request.GetParams<IbisSetReplyValueArgs>();
                return args.Value < 0 || args.Value > 15 ? -1 : 0;
            }

            private object RegisterObject(RpcRequest request)
            {
                var receivedObjects = request.GetParams<string[]>();
                var notification = new RpcNotification();
                notification.method = "notifyObject";
                switch (receivedObjects[0])
                {
                    case "IbisStream":
                        notification.@params = this.CreateIbisStream();
                        this.SendObject(notification);
                        break;
                    case "InfovisionInputState":
                        notification.@params = this.CreateInputState();
                        this.SendObject(notification);
                        break;
                }

                return 0;
            }

            private IbisStream CreateIbisStream()
            {
                var stream = new IbisStream();
                stream.Data = new List<string>();
                if (!string.IsNullOrEmpty(this.currentTelegram))
                {
                    stream.Data.Add(this.currentTelegram);
                }

                return stream;
            }

            private InfovisionInputState CreateInputState()
            {
                var inputState = new InfovisionInputState();
                inputState.Stop0 = 0;
                inputState.Stop1 = 0;
                inputState.Ignition = 1;
                inputState.Address = this.ibisAddress;
                return inputState;
            }
        }
    }
}
