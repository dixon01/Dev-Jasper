// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteComputer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Isi;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.AbuDhabi.Config;

    using NLog;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Object tasked to represents the remote
    /// ISI TCP/IP server.
    /// </summary>
    public class RemoteComputer
    {
        /// <summary>
        /// The logger used by this whole protocol.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ProducerConsumerQueue<IsiMessageBase> sendQueue;

        /// <summary>
        /// The IP address of the remote ISI TCP/IP server.
        /// </summary>
        private IPAddress ip;

        /// <summary>
        /// The port of the remote ISI TCP/IP server.
        /// </summary>
        private int port;

        private IsiRecorder recorder;

        /// <summary>
        /// The TCP client tasked to wrap the
        /// socket with the remote ISI TCP/IP server.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// Object tasked to parse the information exchanged
        /// with the remote ISI TCP/IP server.
        /// </summary>
        private IsiSerializer isiSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteComputer"/> class.
        /// </summary>
        public RemoteComputer()
        {
            this.sendQueue = new ProducerConsumerQueue<IsiMessageBase>(this.WriteMessage, 10);
        }

        /// <summary>
        /// Event that is fired when this remote computer is connected.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Event that is fired when this remote computer is disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Event that is fired when a new ISI message is received.
        /// </summary>
        public event EventHandler<IsiMessageEventArgs> IsiMessageReceived;

        /// <summary>
        /// Gets or sets a value indicating whether we are connected to the remote computer.
        /// </summary>
        [XmlIgnore]
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// Configures this remote computer. 
        /// </summary>
        /// <param name="config">The configuration</param>
        public virtual void Configure(IsiConfig config)
        {
            this.ip = IPAddress.Parse(config.IpAddress);
            this.port = config.Port;

            if (!string.IsNullOrEmpty(config.LogToFile))
            {
                this.recorder = new IsiRecorder(config.LogToFile);
            }
        }

        /// <summary>
        /// Connects to the remote ISI TCP/IP server.
        /// </summary>
        public virtual void Connect()
        {
            if (this.IsConnected)
            {
                return;
            }

            this.tcpClient = new TcpClient();
            Logger.Info("Connecting to {0}:{1}", this.ip, this.port);
            this.tcpClient.BeginConnect(this.ip, this.port, this.HandleConnected, null);
        }

        /// <summary>
        /// Disconnects from the remote ISI TCP/IP server.
        /// </summary>
        public virtual void Disconnect()
        {
            if (!this.IsConnected)
            {
                return;
            }

            this.sendQueue.StopConsumer();

            if (this.recorder != null)
            {
                this.recorder.Flush();
            }

            this.IsConnected = false;
            this.tcpClient.Close();
            Logger.Info("Connection closed.");

            this.RaiseDisconnected(EventArgs.Empty);
        }

        /// <summary>
        /// Sends an ISI put message to the remote ISI computer.
        /// </summary>
        /// <param name="isiMessage">The ISI message to be sent.</param>
        public virtual void SendIsiMessage(IsiMessageBase isiMessage)
        {
            if (!this.IsConnected)
            {
                Logger.Warn("Ignoring message to send since we are not connected: {0}", isiMessage);
                return;
            }

            this.sendQueue.Enqueue(isiMessage);
        }

        /// <summary>
        /// Raises the <see cref="Connected"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseConnected(EventArgs e)
        {
            var handler = this.Connected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Disconnected"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDisconnected(EventArgs e)
        {
            var handler = this.Disconnected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsiMessageReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseIsiMessageReceived(IsiMessageEventArgs e)
        {
            var handler = this.IsiMessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void WriteMessage(IsiMessageBase message)
        {
            try
            {
                this.isiSerializer.Serialize(message);
                Logger.Debug("{0} sent", message.GetType().Name);
                Logger.Trace(message.ToString);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Error while sending an ISI message");
                this.Reconnect();
            }
        }

        private void HandleConnected(IAsyncResult ar)
        {
            if (this.IsConnected)
            {
                // I avoid to handle twice.
                return;
            }

            try
            {
                this.tcpClient.EndConnect(ar);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Error while connecting");
                this.Reconnect();
                return;
            }

            Logger.Info("Connected to {0}:{1}", this.ip, this.port);
            this.IsConnected = true;
            this.isiSerializer = new IsiSerializer
            {
                Input = this.tcpClient.GetStream(),
                Output = this.tcpClient.GetStream()
            };

            if (this.recorder != null)
            {
                this.recorder.WriteComment(string.Format("Connected to {0}:{1}", this.ip, this.port));
                this.isiSerializer.Hook = this.recorder;
            }

            var reader = new Thread(this.Read) { IsBackground = true };
            reader.Start();

            this.sendQueue.StartConsumer();

            this.RaiseConnected(EventArgs.Empty);
        }

        /// <summary>
        /// Reads ISI messages
        /// </summary>
        private void Read()
        {
            try
            {
                while (this.IsConnected)
                {
                    var message = this.isiSerializer.Deserialize();
                    Logger.Debug("{0} received", message.GetType().Name);
                    Logger.Trace(message.ToString);

                    // now I notify the ISI message arrival
                    this.RaiseIsiMessageReceived(new IsiMessageEventArgs { IsiMessage = message });
                }
            }
            catch (ThreadAbortException)
            {
                // ignore TAE
            }
            catch (Exception ex)
            {
                if (this.IsConnected)
                {
                    Logger.Error(ex,"Error while reading ISI messages");
                    this.Reconnect();
                }
            }
        }

        private void Reconnect()
        {
            this.Disconnect();

            Logger.Debug("Reconnecting in 10 seconds...");
            var timer = new Timer(10 * 1000) { AutoReset = false };
            timer.Elapsed += (s, e) => this.Connect();
            timer.Start();
        }
    }
}
