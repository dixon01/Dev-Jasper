// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The Arriva client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Gorba.Motion.Protran.Core.Buffers;

    using NLog;

    /// <summary>
    /// The Arriva client.
    /// </summary>
    internal class ArrivaClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string versionString = string.Empty;
        private readonly bool socketSimulation;
        private readonly Random randomizer;
        private readonly string ipaddress;
        private readonly Crc crcData;

        /// <summary>
        /// The remote TCP server's port.
        /// </summary>
        private readonly int port;

        private Socket clientSocket;
        private Thread threadListen;

        /// <summary>
        /// COS: 23 November 2010
        /// Flag that pilot the activity of the thread tasked to listen
        /// from the TCP connection.
        /// </summary>
        private bool listen = true;

        /// <summary>
        /// COS: 23 November 2010
        /// Flag that stores the listening status of this TCP listener.
        /// </summary>
        private bool isListening;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaClient"/> class.
        /// Allocates all the resources needed by this object.
        /// </summary>
        /// <param name="ip">
        /// The IP address of the remote Arriva's TCP server.
        /// </param>
        /// <param name="port">
        /// The port of the remote Arriva's TCP server.
        /// </param>
        public ArrivaClient(string ip, int port)
        {
            this.socketSimulation = false;
            this.clientSocket = null;
            this.ipaddress = ip;
            this.threadListen = null;
            this.port = port;
            this.IsConnected = false;
            this.crcData = new Crc();
            this.randomizer = new Random();
            this.LastIoOperationTime = new DateTime(1970, 1, 1);
            var path = Assembly.GetExecutingAssembly().Location;
            this.versionString = FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        /// <summary>
        /// Delegate for the eventual closure of the TCP socket occurred to this ArrivalClient.
        /// </summary>
        /// <param name="sender">The object which has sent the event
        /// (it is your ArrivaClient instance). The object that has sent the event.</param>
        public delegate void ClientTerminationHandler(object sender);

        /// <summary>
        /// Delegate for the eventual arrival of a "Next Stop slide" by Arriva.
        /// </summary>
        /// <param name="data">The "Next Stop slide" sent by Arriva.</param>
        public delegate void NextStopSlideHandler(NextStopSlide data);

        /// <summary>
        /// Delegate for the eventual arrival of a "SlideShowMessage" by Arriva.
        /// </summary>
        /// <param name="data">The SlideShowMessage sent by Arriva.</param>
        public delegate void SlideShowMessageHandler(SlideShowMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "AdHocMessage" by Arriva.
        /// </summary>
        /// <param name="data">The AdHocMessage sent by Arriva.</param>
        public delegate void AdHocMessageHandler(AdHocMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "ConnectionsMessage" by Arriva.
        /// </summary>
        /// <param name="data">The ConnectionsMessage sent by Arriva.</param>
        public delegate void ConnectionsMessageHandler(ConnectionsMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "LineInfoMessage" by Arriva.
        /// </summary>
        /// <param name="data">The LineInfoMessage sent by Arriva.</param>
        public delegate void LineInfoHandler(LineInfoMessage data);

        /// <summary>
        /// Delegate for the eventual arrival of a "WifiStatusMessage" by Arriva.
        /// </summary>
        /// <param name="data">The WifiStatusMessage sent by Arriva.</param>
        public delegate void WifiStatusMessageHandler(WifiStatusMessage data);

        /// <summary>
        /// The launcher for the closure of the TCP socket events.
        /// </summary>
        public event ClientTerminationHandler ClientTerminationAllarmer;

        /// <summary>
        /// The launcher for the arrival of a "Next Stop slide" by Arriva.
        /// </summary>
        public event NextStopSlideHandler NextStopSlideAllarmer;

        /// <summary>
        /// The launcher for the arrival of a AdHocMessage by Arriva.
        /// </summary>
        public event AdHocMessageHandler AdHocMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a SlideShowMessage by Arriva.
        /// </summary>
        public event SlideShowMessageHandler SlideShowMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a ConnectionsMessage by Arriva.
        /// </summary>
        public event ConnectionsMessageHandler ConnectionsMessageAllarmer;

        /// <summary>
        /// The launcher for the arrival of a LineInfoMessage by Arriva.
        /// </summary>
        public event LineInfoHandler LineInfoAllarmer;

        /// <summary>
        /// The launcher for the arrival of a WifiStatusMessage by Arriva.
        /// </summary>
        public event WifiStatusMessageHandler WifiStatusMessageAllarmer;

        /// <summary>
        /// Gets a value indicating whether the socket is connected or not.
        /// COS: 23 November 2010
        /// Gets the connective status of this client.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the timestamp in which was done the last I/O operation
        /// by this TCP client.
        /// COS: 24 November 2010
        /// </summary>
        public DateTime LastIoOperationTime { get; private set; }

        /// <summary>
        /// Create the TCP client and try to connect it to the remote TCP server.
        /// If you want to know the connection status of this TCP client,
        /// please call the property "IsConnected".
        /// </summary>
        public void Connect()
        {
            // COS: 23 November 2010
            // flag protection and code refactoring.
            if (this.IsConnected)
            {
                // the client is already running.
                // I don't start it twice.
                Logger.Info("Client already running");
                return;
            }

            var ok = this.CreateAndConnectSocket();
            if (!ok)
            {
                // it was impossible to create/connect the TCP client.
                // I don't start the referring thread listener.
                Logger.Info("Not connected to Arriva server");
                return;
            }

            this.listen = true;
            this.isListening = false;
            this.threadListen = new Thread(this.Listen) { IsBackground = true, Name = "Th_TCPClientListener" };
            this.threadListen.Start();
        }

        /// <summary>
        /// Close the TCP connection with the remote TCP server.
        /// If you want to know the connection status of this TCP client,
        /// please call the property "IsConnected".
        /// </summary>
        public void Stop()
        {
            // COS: 23 November 2010
            // flag protection and code refactoring.
            if (!this.IsConnected)
            {
                // the client is not running.
                // I don't stop it twice.
                Logger.Info("Client already stopped");
                return;
            }

            this.listen = false;
            this.clientSocket.Close(500);
            while (this.isListening)
            {
                // wait for the real completion of the thread.
                Thread.Sleep(10);
            }

            // if I reach this line of code, it means that the thread listener
            // was really terminated.
            // ok, so let's start to clean the room...
            Thread.Sleep(10);
            this.clientSocket = null;
            this.threadListen = null;
            this.IsConnected = false;
        }

        /// <summary>
        /// COS: 24 November 2010
        /// I've changed the signature of this function (now it is no more "void",
        /// but it returns a boolean).
        /// Send an "Hello" message to the remote TCP server.
        /// </summary>
        /// <param name="arrivaHeaderData">The data representing the "Hello" message to send</param>
        /// <returns>True if the operation was done with success, else false.</returns>
        public bool SendHello(ArrivaHeaderData arrivaHeaderData)
        {
            if (!this.IsConnected)
            {
                // my TCP client is not connected.
                // I don't send nothing.
                return false;
            }

            // yes, I'm really connected.
            if (arrivaHeaderData == null)
            {
                // invalid data. I don't send nothing.
                Logger.Info("Invalid header received");
                return false;
            }

            // everything seems ok. let's try to send the data.
            try
            {
                // Connect();
                byte[] buf =
                    {
                        0x32, 0x64, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00
                    };

                buf[8] = arrivaHeaderData.MessageTransactionId1;
                buf[9] = arrivaHeaderData.MessageTransactionId2;
                buf[10] = arrivaHeaderData.MessageTransactionId3;
                buf[11] = arrivaHeaderData.MessageTransactionId4;

                var mycrc = this.crcData.CalculateCrc(buf, 30);
                buf[30] = (byte)((mycrc & 0xFF00) >> 8); // HighByte
                buf[31] = (byte)(mycrc & 0x00FF); // LowByte

                var lenSent = this.clientSocket.Send(buf, buf.Length, SocketFlags.None);
                var boolToReturn = lenSent == buf.Length;

                // update the time in which was done the last I/O operation
                this.LastIoOperationTime = boolToReturn ? DateTime.Now : this.LastIoOperationTime;
                Logger.Info("Reconstruction of buffer successful");
                return boolToReturn;
            }
            catch (Exception ex)
            {
                Logger.Info("Reconstruction of buffer failed due to {0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Send a message containing the information about our TCP client's status
        /// or about our application's version.
        /// </summary>
        /// <param name="arrivaHeaderData">The data to send.</param>
        /// <param name="infoType">The media type of the information that will be sent.</param>
        /// <param name="info">The information to send, contained in a string.</param>
        /// <returns>True if sent with success, else false.</returns>
        public bool SendStatusOrVersion(ArrivaHeaderData arrivaHeaderData, InfoRequested infoType, string info)
        {
            if (!this.IsConnected)
            {
                // my TCP client is not connected.
                // I don't send nothing.
                return false;
            }

            // yes, I'm really connected.
            if (arrivaHeaderData == null || infoType == InfoRequested.None)
            {
                Logger.Info("Invalid header received");
                return false;
            }

            int payloadLength = (info.Length * 2) + 4;

            var headerBuffer = this.ComposeStatusVersionBuffer(arrivaHeaderData, infoType, info, payloadLength);

            int lenSent;
            byte[] totBuffer;
            try
            {
                var payloadBuffer = Encoding.BigEndianUnicode.GetBytes(info);
                totBuffer = new byte[headerBuffer.Length + payloadBuffer.Length];
                Array.Copy(headerBuffer, 0, totBuffer, 0, headerBuffer.Length);
                Array.Copy(payloadBuffer, 0, totBuffer, headerBuffer.Length, payloadBuffer.Length);
                lenSent = this.clientSocket.Send(totBuffer, totBuffer.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                // some error with the buffer. I cannot send nothing.
                Logger.Info(ex, "Error in sending status buffer");
                return false;
            }

            var ok = lenSent == totBuffer.Length;

            // update the time in which was done the last I/O operation
            this.LastIoOperationTime = ok ? DateTime.Now : this.LastIoOperationTime;
            return ok;
        }

        private byte[] ComposeStatusVersionBuffer(
            ArrivaHeaderData arrivaHeaderData, InfoRequested infoType, string info, int payloadLength)
        {
            var headerBuffer = new byte[32 + 4];
            headerBuffer[0] = 0x32;
            headerBuffer[1] = 0x64;
            headerBuffer[6] = 0x20;
            headerBuffer[7] = 0x01;
            headerBuffer[8] = arrivaHeaderData.MessageTransactionId1;
            headerBuffer[9] = arrivaHeaderData.MessageTransactionId2;
            headerBuffer[10] = arrivaHeaderData.MessageTransactionId3;
            headerBuffer[11] = arrivaHeaderData.MessageTransactionId4;

            headerBuffer[12] = 0x00;
            headerBuffer[13] = 0x01;

            headerBuffer[14] = 0x10;
            headerBuffer[15] = 0x00;

            headerBuffer[18] = (byte)((payloadLength & 0xFF00) >> 8);
            headerBuffer[19] = (byte)(payloadLength & 0x00FF);

            headerBuffer[20] = (byte)((payloadLength & 0xFF000000) >> 24);
            headerBuffer[21] = (byte)((payloadLength & 0x00FF0000) >> 16);
            headerBuffer[22] = (byte)((payloadLength & 0x0000FF00) >> 8);
            headerBuffer[23] = (byte)(payloadLength & 0x000000FF);

            ushort crc = this.crcData.CalculateCrc(headerBuffer, 30);
            headerBuffer[30] = (byte)((crc & 0xFF00) >> 8); // HighByte
            headerBuffer[31] = (byte)(crc & 0x00FF); // LowByte

            headerBuffer[32] = 0x01;
            headerBuffer[33] = 50;
            headerBuffer[34] = (byte)((infoType == InfoRequested.Status) ? 0x01 : 0x02);
            headerBuffer[35] = (byte)info.Length;
            return headerBuffer;
        }

        /// <summary>
        /// Create the TCP client and try to connect it to the remote TCP server.
        /// </summary>
        /// <returns>
        /// The create and connect socket.
        /// </returns>
        private bool CreateAndConnectSocket()
        {
            // COS: 23 November 2010
            // code refactoring.
            // Attention !!!
            // I've deleted the old function "Disconnect" function.
            // Why ? because I strongly reject the "Shutdown" function to close a socket, as it did.
            try
            {
                var ipa = IPAddress.Parse(this.ipaddress);
                var endPoint = new IPEndPoint(ipa, this.port);

                // I don't want the socket to stay connected for any length of time after closing,
                // so I create a LingerOption with the enable parameter set to true
                // and the seconds parameter set to zero.
                // In this case, the socket will close immediately and any unsent data will be lost.
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    LingerState = new LingerOption(true, 0)
                };

                this.clientSocket.Connect(endPoint);
            }
            catch
            {
                this.clientSocket = null;
                Logger.Debug("Connection to Arriva server failed");
                return false;
            }

            // ok, our client is really connected to the remote server.
            this.IsConnected = this.clientSocket.Connected;
            Logger.Info("Connected to Arriva server");
            return this.IsConnected;
        }

        /// <summary>
        /// Listen in an infinite loop data from the TCP connection established with
        /// the remote TCP server, until an error occurs on the socket or to this TCP
        /// client was told to be closed.
        /// </summary>
        private void Listen()
        {
            this.isListening = true;
            var receiveBuffer = new byte[4096];

            var parserArriveHeader = new ParseArrivaHeader();
            var parserNextStop = new ParseNextStop();
            var arrivaHeader = new ArrivaHeaderData();

            var ok = this.SendHello(arrivaHeader);
            if (ok)
            {
                Logger.Info("Initial 'Hello' message sent with success");
            }

            Logger.Debug("Waiting for data");
            while (this.listen)
            {
                try
                {
                    this.LastIoOperationTime = DateTime.Now;
                    byte[] msg;
                    int bytesRead = 0;

                    if (this.socketSimulation)
                    {
                        const string DebugString = "ADD HERE THE HEX STRING YOU WANT TO DEBUG";
                        msg = BufferUtils.FromHexStringToByteArray(DebugString);
                        bytesRead = (msg != null) ? msg.Length : 0;
                    }
                    else
                    {
                        do
                        {
                            bytesRead += this.clientSocket.Receive(
                                receiveBuffer, bytesRead, this.clientSocket.Available, SocketFlags.None);
                        }
                        while (this.clientSocket.Available > 0);
                        if (bytesRead == 0)
                        {
                            // something was occured on the remote TCP server.
                            break;
                        }

                        // I've read a valid amount of bytes from the socket, so I can copy them to the rigth buffer.
                        msg = new byte[bytesRead];
                        Array.Copy(receiveBuffer, 0, msg, 0, bytesRead);
                    }

                    Logger.Debug("Msg arrived (bytes: " + bytesRead + ")");
                    string bufferString = BufferUtils.DumpBufferAsHexString(msg);
                    Logger.Debug(bufferString);
                    parserArriveHeader.Msg = msg;
                    arrivaHeader.Clear();
                    arrivaHeader = parserArriveHeader.CheckHeader();

                    if (this.AnalyseMessageCode(ok, parserArriveHeader, arrivaHeader, msg, parserNextStop))
                    {
                        Logger.Info("Parsed correctly");
                    }

                    // if !(this.ClientSocket.Available > 0) it means that the remote server was closed.
                    // ok, I've only to terminate this thread.
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Socket error");
                    break;
                }
            }

            // eventually, notify about the TCP client's death.
            if (this.ClientTerminationAllarmer != null)
            {
                var threadNotifyClosure = new Thread(this.NotifyClosure) { Name = "Th_NotifyArrivaClosure" };
                threadNotifyClosure.Start();
            }

            // this thread is terminated.
            this.isListening = false;
        }

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Legacy code.")]
        private bool AnalyseMessageCode(
            bool ok,
            ParseArrivaHeader parserArriveHeader,
            ArrivaHeaderData arrivaHeader,
            byte[] msg,
            ParseNextStop parserNextStop)
        {
            switch (arrivaHeader.MessageCode)
            {
                case 0x0001:
                    {
                        // old version
                        // answerHello = !answerHello ? !answerHello : answerHello;
                        // if (!answerHello)
                        // {
                        //    this.SendHello(arrivaHeader);
                        // }
                    }

                    break;

                case 0x2000:
                    {
                        // wow ! someone is interested to know our status or our version.
                        InfoRequested info = parserArriveHeader.GetInfoRequested();
                        if (info == InfoRequested.None)
                        {
                            // bad request. I skip it.
                            return true;
                        }

                        // ok, let's start to prepare the answer.
                        string infoToSend = (info == InfoRequested.Status) ? "Running" : this.versionString;
                        var answerData = new ArrivaHeaderData();
                        answerData.SetMessageTransactionId(this.randomizer.Next());
                        this.SendStatusOrVersion(answerData, info, infoToSend);
                    }

                    break;

                case 0x1800:
                    {
                        // I've received a message regarding a slide show.
                        // now I "give" the buffer to the parser
                        parserNextStop.Msg = msg;

                        // and now I invoke its parsing abilities...
                        SlideShowMessage slideShowMsg = parserNextStop.ParseSlideShowMsg();
                        if (slideShowMsg == null)
                        {
                            // an error was occured parsing the message.
                            // I continue anymore with this case.
                            return true;
                        }

                        this.NotifyArrivaSlideShowMsg(slideShowMsg);
                    }

                    break;

                case 0x1801:
                    {
                        // I've received a message regarding an ad hoc message.
                        // now I "give" the buffer to the parser
                        parserNextStop.Msg = msg;

                        // and now I invoke its parsing abilities...
                        var parsegMessage = parserNextStop.ParseAdHocMsg();
                        if (parsegMessage == null)
                        {
                            // an error was occured parsing the message.
                            // I continue anymore with this case.
                            return true;
                        }

                        this.NotifyArrivaAdHocMsg(parsegMessage);
                    }

                    break;

                case 0x1802:
                    {
                        parserNextStop.Msg = msg;
                        NextStopSlide nextStopSlide = parserNextStop.ParseNextStopData();

                        // update the time in which was done the last I/O operation
                        this.LastIoOperationTime = ok ? DateTime.Now : this.LastIoOperationTime;

                        if (nextStopSlide != null)
                        {
                            // I notify the arrival of a "next stop" slide from Arriva.
                            this.NotifyArrivaNextStop(nextStopSlide);
                        }
                    }

                    break;

                case 0x1803:
                    {
                        // I've received a message regarding a connection message.
                        // now I "give" the buffer to the parser
                        parserNextStop.Msg = msg;

                        // and now I invoke its parsing abilities...
                        ConnectionsMessage connectionsMsg = parserNextStop.ParseConnectionsMsg();
                        if (connectionsMsg == null)
                        {
                            // an error was occured parsing the message.
                            // I continue anymore with this case.
                            return true;
                        }

                        this.NotifyArrivaConnectisonsMsg(connectionsMsg);
                    }

                    break;

                case 0x1804:
                    {
                        // I've received a message regarding line info.
                        // now I "give" the buffer to the parser
                        parserNextStop.Msg = msg;

                        // and now I invoke its parsing abilities...
                        LineInfoMessage lineInfoMsg = parserNextStop.ParseLineInfoMsg();
                        if (lineInfoMsg == null)
                        {
                            // an error was occured parsing the message.
                            // I continue anymore with this case.
                            return true;
                        }

                        this.NotifyArrivaLineInfoMsg(lineInfoMsg);
                    }

                    break;

                case 0x3000:
                    {
                        // I've received a message regarding the wifi status.
                        // now I "give" the buffer to the parser
                        parserNextStop.Msg = msg;

                        // and now I invoke its parsing abilities...
                        WifiStatusMessage wifiStatusMsg = parserNextStop.ParseWifiStatusMsg();
                        if (wifiStatusMsg == null)
                        {
                            // an error was occured parsing the message.
                            // I continue anymore with this case.
                            return true;
                        }

                        this.NotifyArrivaWifiStatusMsg(wifiStatusMsg);
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Notify each registered handlers about the closure of this TCP client.
        /// </summary>
        private void NotifyClosure()
        {
            if (this.ClientTerminationAllarmer != null)
            {
                // <== a check for safety
                this.ClientTerminationAllarmer(this);
            }
        }

        /// <summary>
        /// Notify each registered handlers about the arrival of a "Next Stop slide"
        /// from Arriva.
        /// </summary>
        /// <param name="nextStopSlide">The "Next Stop slide" from Arriva.</param>
        private void NotifyArrivaNextStop(NextStopSlide nextStopSlide)
        {
            if (nextStopSlide == null)
            {
                // invalid container.
                return;
            }

            var handler = this.NextStopSlideAllarmer;
            if (handler != null)
            {
                Logger.Info("Next stop slide message received");
                handler(nextStopSlide);
            }
        }

        private void NotifyArrivaSlideShowMsg(SlideShowMessage slideShowMsg)
        {
            if (slideShowMsg == null)
            {
                // invalid message
                return;
            }

            // ok, I notify this message to the all registered handlers.
            var handler = this.SlideShowMessageAllarmer;
            if (handler != null)
            {
                Logger.Info("Slide show message received");
                handler(slideShowMsg);
            }
        }

        private void NotifyArrivaAdHocMsg(AdHocMessage message)
        {
            if (message == null)
            {
                // invalid message
                return;
            }

            // ok, I notify this message to the all registered handlers.
            var handler = this.AdHocMessageAllarmer;
            if (handler != null)
            {
                Logger.Info("Ad Hoc message received");
                handler(message);
            }
        }

        private void NotifyArrivaConnectisonsMsg(ConnectionsMessage connectionsMsg)
        {
            if (connectionsMsg == null)
            {
                // invalid message
                return;
            }

            // ok, I notify this message to the all registered handlers.
            var handler = this.ConnectionsMessageAllarmer;
            if (handler != null)
            {
                Logger.Info("Connection message received");
                handler(connectionsMsg);
            }
        }

        private void NotifyArrivaLineInfoMsg(LineInfoMessage lineInfoMsg)
        {
            if (lineInfoMsg == null)
            {
                // invalid message
                return;
            }

            // ok, I notify this message to the all registered handlers.
            var handler = this.LineInfoAllarmer;
            if (handler != null)
            {
                Logger.Info("Line info message received");
                handler(lineInfoMsg);
            }
        }

        private void NotifyArrivaWifiStatusMsg(WifiStatusMessage wifiStatusMsg)
        {
            if (wifiStatusMsg == null)
            {
                // invalid message
                return;
            }

            // ok, I notify this message to the all registered handlers.
            var handler = this.WifiStatusMessageAllarmer;
            if (handler != null)
            {
                Logger.Info("Wifi message received");
                handler(wifiStatusMsg);
            }
        }
    }
}
