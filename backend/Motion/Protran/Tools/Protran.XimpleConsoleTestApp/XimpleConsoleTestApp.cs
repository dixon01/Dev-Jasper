// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleConsoleTestApp.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Protran.XimpleConsoleTestApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Xml;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.Motion.Protran.XimpleProtocol;

    using Protran.XimpleConsoleTestApp.Properties;

    /// <summary>The ximple console test app.</summary>
    internal class XimpleConsoleTestApp : ApplicationBase
    {
        #region Static Fields

        private static ConsoleApplicationHost<XimpleConsoleTestApp> host;

        #endregion

        #region Fields

        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        #endregion

        #region Public Methods and Operators

        /// <summary>The send ximple.</summary>
        /// <param name="xmlDocuments">The xml documents.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="msDelay">The ms Delay.</param>
        public void SendXimple(List<XmlDocument> xmlDocuments, string ip = "127.0.0.1", int port = XimpleSocketService.DefaultPort, int msDelay = 0)
        {
            if (xmlDocuments.Count == 0)
            {
                Logger.Error("No Xml Documents found to process");
                return;
            }

            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(ip, port);
                    Logger.Info("Success Connected to server {0}:{1}", ip, port);

                    if (tcpClient.Connected)
                    {
                        Logger.Info("Connecting to server {0}:{1}", ip, port);

                        // tcpClient.Client.DontFragment = true; // if the Socket allows datagram fragmentation; otherwise, false. The default is true.

                        // Disable the Nagle Algorithm for this tcp socket.
                        tcpClient.Client.NoDelay = true;

                        var socket = tcpClient.Client;
                        socket.SendTimeout = 5000;
                        var readBuffer = new byte[4096];
                        foreach (var doc in xmlDocuments)
                        {
                            var xml = doc.InnerXml;
                            var buffer = Encoding.UTF8.GetBytes(xml);
                            Logger.Info("Sending XML to server {0}:{1}, Length={2}", ip, port, xml.Length);
                            var bytesWritten = socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                            if (bytesWritten != buffer.Length)
                            {
                                Debug.Fail("(bytesWritten != buffer.Length");
                                Logger.Error("!Expected number of bytes written to socket failed");
                            }

                            try
                            {
                                socket.ReceiveTimeout = 5000;
                                var bytesRead = socket.Receive(readBuffer, 0, readBuffer.Length, SocketFlags.None);
                                if (bytesRead > 0)
                                {
                                    DebugOutputServerResponce(readBuffer, bytesRead);
                                }

                                // optional delay
                                if (msDelay > 0 && bytesRead <= 0)
                                {
                                    Thread.Sleep(msDelay);
                                }
                            }
                            catch (SocketException ex)
                            {
                                if (ex.SocketErrorCode != SocketError.TimedOut)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (SocketException socketException)
                {
                    Logger.Error(socketException.Message);
                }
                finally
                {
                    if (tcpClient.Client.Connected)
                    {
                        tcpClient.Client.Shutdown(SocketShutdown.Both);
                        tcpClient.Close();
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>The do run.</summary>
        /// <param name="args">The args.</param>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        protected override void DoRun(string[] args)
        {
            base.DoRun(args);
            this.SetRunning();

            var defaultPath = Settings.Default.DefaultXimpleFolder;
            var path = args.Length > 0 ? args.First() : defaultPath;

            // UnitTestMediaMessages();
            Console.WriteLine("Running...");

            ExecuteTest(path);
        }

        /// <summary>The do stop.</summary>
        protected override void DoStop()
        {
            this.runWait.Set();
        }

        private static string DebugOutputServerResponce(byte[] responseBuffer, int bytesReceived)
        {
            var s = Encoding.UTF8.GetString(responseBuffer, 0, bytesReceived);
            Console.WriteLine("Bytes received = " + responseBuffer.Length + "\r\nString=" + s);
            return s;
        }

        private static void Main(string[] args)
        {
            // medi.config is a required file by ApplicationBase ! 
            host = new ConsoleApplicationHost<XimpleConsoleTestApp>();            
            host.Run("XimpleConsoleTestApp");
        }

        private void ExecuteTest(string path)
        {
            Console.CancelKeyPress += (sender, args) => { runWait.Set(); };

            while (!this.runWait.WaitOne(1000))
            {
                var xmlDocs = this.LoadXimpleXmlFiles(path);
                if (xmlDocs.Any())
                {
                    this.SendXimple(xmlDocs, Settings.Default.ProtranServerIp, Settings.Default.Port, Settings.Default.DelayBetweenXimpleMessages);
                }
                else
                {
                    Logger.Warn("No Test XML Source files Found in path {0}", path);
                }

                Console.WriteLine("Done. Q = Quit and Exit, Enter Key = Run Again");
                var key = Console.ReadKey();
                if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                {
                    break;
                }
            }
        }

        private List<XmlDocument> LoadXimpleXmlFiles(string path)
        {
            this.Logger.Info("Loading Ximple documents from {0}", path);

            var xmlDocuments = new List<XmlDocument>();
            var files = Directory.GetFiles(Path.GetFullPath(path), "*.xml");
            foreach (var file in files)
            {
                this.Logger.Info("Reading XML File:  {0}", Path.GetFileName(file));
                try
                {
                    var xmlDoc = new XmlDocument();
                    using (var f = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        xmlDoc.Load(f);
                        var xml = xmlDoc.InnerXml;

                        if (Settings.Default.EnableXmlToXimpleCheck)
                        {
                            // quick check to see if the xml will serialize to Ximple class
                            var ximple = XmlHelpers.ToXimple(xml);
                            if (ximple != null)
                            {
                                xmlDocuments.Add(xmlDoc);
                            }
                        }
                        else
                        {
                            xmlDocuments.Add(xmlDoc);
                        }

                        f.Close();
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Failed reading xml File = {0}", file);
                }
            }

            this.Logger.Info("Found{0} XML Files", xmlDocuments.Count);
            return xmlDocuments;
        }

        private void UnitTestMediaMessages()
        {
            MessageDispatcher.Instance.Subscribe<NetworkChangedMessage>(
                (sender, args) => { Console.WriteLine("!!!  Received Medi Message NetworkChangedMessage"); });
        }

        #endregion
    }
}