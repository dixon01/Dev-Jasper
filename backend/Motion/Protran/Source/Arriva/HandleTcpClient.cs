// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandleTcpClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   COS: 23 November 2010
//   A wrapper to the ArrivaClient. Why ?
//   For example because in this way it is easier for me the re-connection management.
//   Also, the TCP server has its wrapper and why not the same treatment for the client ?
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Arriva;
    using Gorba.Common.Medi.Core.Management;

    using NLog;

    /// <summary>
    /// COS: 23 November 2010
    /// A wrapper to the ArrivaClient. Why ?
    /// For example because in this way it is easier for me the re-connection management.
    /// Also, the TCP server has its wrapper and why not the same treatment for the client ?
    /// </summary>
    public class HandleTcpClient : IManageableObject
    {
        private const int Refreshperiod = 10000; // ms

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly int port = -1;

        private readonly string ip = string.Empty;

        private ArrivaClient tcpClient;
        private Thread sockRefresherthread;
        private Mutex semaphore;

        private bool close;
        private bool refresh;
        private bool isRefreshFinished;

        private int arrivaMessageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleTcpClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration for the connection to the OBU.
        /// </param>
        public HandleTcpClient(ObuConfig config)
        {
            this.semaphore = new Mutex();
            this.sockRefresherthread = null;
            this.ip = config.RemoteIp;
            this.port = config.RemotePort;
            this.refresh = false;
            this.isRefreshFinished = true;
        }

        /// <summary>
        /// Delegate for the eventual arrival of a "Next Stop slide" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The "Next Stop slide" sent by Arriva.</param>
        public delegate void ArrivaNextStopDataHandler(object sender, NextStopSlide data);

        /// <summary>
        /// Delegate for the eventual arrival of a "SlideShowMessage" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The SlideShowMessage sent by Arriva.</param>
        public delegate void ArrivaSlideShowMessageHandler(object sender, SlideShowMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "AdHocMessage" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The AdHocMessage sent by Arriva.</param>
        public delegate void ArrivaAdHocMessageHandler(object sender, AdHocMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "ConnectionsMessage" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The ConnectionsMessage sent by Arriva.</param>
        public delegate void ArrivaConnectionsMessageHandler(object sender, ConnectionsMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "LineInfoMessage" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The LineInfoMessage sent by Arriva.</param>
        public delegate void ArrivaLineInfoHandler(object sender, LineInfoMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "WifiStatusMessage" by Arriva.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="data">The WifiStatusMessage sent by Arriva.</param>
        public delegate void ArrivaWifiStatusMessageHandler(object sender, WifiStatusMessage data);

        /// <summary>
        /// The launcher for the arrival of a ConnectionsMessage by Arriva.
        /// </summary>
        public event ArrivaConnectionsMessageHandler ArrivaConnectionsMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a AdHocMessage by Arriva.
        /// </summary>
        public event ArrivaAdHocMessageHandler ArrivaAdHocMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a LineInfoMessage by Arriva.
        /// </summary>
        public event ArrivaLineInfoHandler ArrivaLineInfoAllarmer;

        /// <summary>
        /// The launcher for the arrival of a SlideShowMessage by Arriva.
        /// </summary>
        public event ArrivaSlideShowMessageHandler ArrivaSlideShowMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a "Next Stop slide" by Arriva.
        /// </summary>
        public event ArrivaNextStopDataHandler ArrivaNextStopDataAllarmer;

        /// <summary>
        /// The launcher for the arrival of a WifiStatusMessage by Arriva.
        /// </summary>
        public event ArrivaWifiStatusMessageHandler ArrivaWifiStatusMessageAllarmer;

        /// <summary>
        /// Gets a value indicating whether IsConnected.
        /// </summary>
        public bool IsConnected
        {
            get { return (this.tcpClient != null) && this.tcpClient.IsConnected; }
        }

        /// <summary>
        /// Try to establish a TCP connection with the remote Arriva TCP server.
        /// If you want to know if this client is connected, use the property "IsConnected".
        /// </summary>
        public void Start()
        {
            this.semaphore.WaitOne();
            if (this.tcpClient != null && this.tcpClient.IsConnected)
            {
                // I don't start the Arriva client twice.
                this.semaphore.ReleaseMutex();
                return;
            }

            this.tcpClient = new ArrivaClient(this.ip, this.port);
            this.tcpClient.ClientTerminationAllarmer += this.OnClientTermination;
            this.tcpClient.NextStopSlideAllarmer += this.OnNextStopSlideArrived;
            this.tcpClient.SlideShowMessageAllarmer += this.OnSlideShowMessageAllarmer;
            this.tcpClient.AdHocMessageAllarmer += this.OnAdHocMessageAllarmer;
            this.tcpClient.ConnectionsMessageAllarmer += this.OnConnectionsMessageAllarmer;
            this.tcpClient.LineInfoAllarmer += this.OnLineInfoAllarmer;
            this.tcpClient.WifiStatusMessageAllarmer += this.OnWifiStatusMessageAllarmer;
            this.tcpClient.Connect();

            if (this.tcpClient.IsConnected)
            {
                Logger.Info("Tcp client connected");

                // wow ! our TCP client was connected. it's a good news this.
                // I think that is also good to start here a socket's refresher mechanism.
                // so, let's start it in a separate thread.
                this.refresh = true;
                this.isRefreshFinished = false;
                this.sockRefresherthread = new Thread(this.RefreshSocket);
                this.sockRefresherthread.Name = "Th_SockRefresher";
                this.sockRefresherthread.Start();
            }

            this.semaphore.ReleaseMutex();
        }

        /// <summary>
        /// Delete the TCP connection established with the remote Arriva TCP server.
        /// If you want to know if this client is disconnected, use the property "IsConnected".
        /// </summary>
        public void Stop()
        {
            this.semaphore.WaitOne();
            if (this.tcpClient == null || !this.tcpClient.IsConnected)
            {
                // the Arriva client is already close.
                // I don't close it twice.
                this.semaphore.ReleaseMutex();
                Logger.Info("Tcp client already closed");
                return;
            }

            // I stop the thread socket's refresher before closing the TCP connection.
            this.refresh = false;
            while (!this.isRefreshFinished)
            {
                // wait for the real completion of the thread.
                this.semaphore.ReleaseMutex();
                Thread.Sleep(10);
                this.semaphore.WaitOne();
            }

            Thread.Sleep(10);
            this.sockRefresherthread = null;

            // at this line of code, I'm sure that the thread is surely terminated.
            // ok, let's continue with stopping the TCP client.
            this.tcpClient.ClientTerminationAllarmer -= this.OnClientTermination;
            this.tcpClient.NextStopSlideAllarmer -= this.OnNextStopSlideArrived;
            this.tcpClient.SlideShowMessageAllarmer -= this.OnSlideShowMessageAllarmer;
            this.tcpClient.AdHocMessageAllarmer -= this.OnAdHocMessageAllarmer;
            this.tcpClient.ConnectionsMessageAllarmer -= this.OnConnectionsMessageAllarmer;
            this.tcpClient.LineInfoAllarmer -= this.OnLineInfoAllarmer;
            this.tcpClient.WifiStatusMessageAllarmer -= this.OnWifiStatusMessageAllarmer;
            this.tcpClient.Stop();
            this.tcpClient = null;
            this.semaphore.ReleaseMutex();
        }

        /// <summary>
        /// Delete all the resources allocated by this object.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            this.close = true;
            this.semaphore.Close();
            this.semaphore = null;
            if (this.semaphore == null)
            {
                return;
            }

            this.close = true;
            Thread.Sleep(100);

            this.Stop();
            this.semaphore.Close();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>(
                "Number of messages from Arriva OBU", this.arrivaMessageCount, true);
        }

        private void RefreshSocket()
        {
            // before starting with the refresh procedure,
            // I prepare here for myself all my variables.
            const int RefreshSubWaitInterval = 5;
            var randomizer = new Random();
            var data = new ArrivaHeaderData();

            while (this.refresh)
            {
                for (int i = 0; i < RefreshSubWaitInterval; i++)
                {
                    Thread.Sleep(Refreshperiod / RefreshSubWaitInterval);

                    // before making the refresh operation, I'm want
                    // to make some checks on the TCP client.
                    // for example, is it really connected ?
                    if (!this.tcpClient.IsConnected)
                    {
                        // the TCP client is not connected !?!
                        // how can I refresh it ? I can't.
                        // so, this thread has no meaning to be alive.
                        break;
                    }

                    if (!this.refresh)
                    {
                        // during our sleep time, someone has telled us to die.
                        // ok, I die.
                        break;
                    }
                }

                // ok, the TCP client is really connected.
                // good, let's try to send a foo message through the Ethernet cable...
                DateTime lastTimeSentSomething = this.tcpClient.LastIoOperationTime;
                TimeSpan diff = DateTime.Now - lastTimeSentSomething;
                if (diff.TotalMilliseconds < Refreshperiod)
                {
                    // to few time was elapsed since the last I/O sending operation.
                    // the socket is "freshed" by its own.
                    continue;
                }

                // yes the last I/O sending operation was done to much time ago.
                // I've to send an "Hello" message to keep refreshed the socket.
                data.SetMessageTransactionId(randomizer.Next());
                this.semaphore.WaitOne();
                var send = this.tcpClient.SendHello(data);
                this.semaphore.ReleaseMutex();
                if (!send)
                {
                    // Attention !!!
                    // the sending operation has failed.
                    // I do nothing. Somebody will call the "OnClientTermination" function.
                    // here, I've only to terminate this thread.
                    break;
                }

                // the "Hello" message was sent with success.
                // ok, continue the cycle.
                Logger.Debug("Hello message sent to keep alive");
            }

            this.isRefreshFinished = true;
        }

        /// <summary>
        /// Function invoked (asynchronously) whenever the ArrivaClient has lost the connection
        /// with the remote server.
        /// </summary>
        /// <param name="sender">The ArrivaClient instance that has launched the event.</param>
        private void OnClientTermination(object sender)
        {
            Thread.Sleep(3000);
            if (this.close)
            {
                // I don't do nothing.
                return;
            }

            // our TCP client is death.
            // I try to re-establish the connection.
            this.Stop(); // <== only for safety.

            Logger.Warn("Connection lost");
            while (!this.IsConnected)
            {
                Logger.Info("Retry to connect");
                Thread.Sleep(3000);
                if (this.close)
                {
                    // in the meanwhile, someone has told me to close.
                    // so, I do it.
                    return;
                }

                this.Start();
            }

            if (this.IsConnected)
            {
                Logger.Info("Reconnected");
            }
        }

        /// <summary>
        /// Function invoked (asynchronously) whenever the ArrivaClient has received a
        /// "Next Stop slide" from the remote Arriva's TCP server.
        /// </summary>
        /// <param name="data">The "Next Stop slide" received from the remote
        /// Arriva's TCP server.</param>
        private void OnNextStopSlideArrived(NextStopSlide data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaNextStopDataAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }

        private void OnWifiStatusMessageAllarmer(WifiStatusMessage data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaWifiStatusMessageAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }

        private void OnLineInfoAllarmer(LineInfoMessage data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaLineInfoAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }

        private void OnConnectionsMessageAllarmer(ConnectionsMessage data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaConnectionsMessageAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }

        private void OnAdHocMessageAllarmer(AdHocMessage data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaAdHocMessageAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }

        private void OnSlideShowMessageAllarmer(SlideShowMessage data)
        {
            this.arrivaMessageCount++;
            if (data == null)
            {
                // invalid data
                return;
            }

            var handler = this.ArrivaSlideShowMessageAllarmer;
            if (handler != null)
            {
                handler(this, data);
            }
        }
    }
}
