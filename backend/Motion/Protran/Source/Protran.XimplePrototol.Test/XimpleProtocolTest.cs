// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleProtocolTest.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Protran.XimplePrototol.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.XimpleProtocol;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.Motion.Protran.XimpleProtocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]


    /// <summary>The ximple protocol test.</summary>
    [TestClass]
    [DeploymentItem(@"Config\medi.config")]
    [DeploymentItem(@"Config\dictionary.xml")]
    [DeploymentItem(@"Config\Update.xml")]
    [DeploymentItem(@"Config\XimpleConfigTest2.xml")]
    [DeploymentItem(@"Config\XimpleConfig.xml")]
    public class XimpleProtocolTest
    {
        private const string UnitName = "TEST";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly object socketTestLock = new object();

        private static int autoServerPort = XimpleSocketService.DefaultPort;

        private static FileConfigurator fileConfigurator;

        private static TestContext testContext;

        private readonly ManualResetEvent cannedMediMessageEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Event that is fired whenever a medi message of InfoTainment AudioStatusMessage is received and
        ///     a new Ximple response is generated to goto the 3rdparty equipment - LTG Infotrainment.
        /// </summary>
        private event EventHandler<AudioStatusMessage> AudioStatusMessageChanged;

        private event EventHandler<XimpleEventArgs> XimpleCreated;

        private event EventHandler<string> XimpleSocketDataReceived;

        /// <summary>The class init.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            testContext = context;
            var nlogTraceListner = new NLogTraceListener();
            nlogTraceListner.TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.ThreadId;
            Debug.Listeners.Add(nlogTraceListner);
            InitMedi();
            Debug.WriteLine("XimpleProtocolTest.ClassInit()");
        }

        /// <summary>The broadcast medi network connection changed.</summary>
        [TestMethod]
        public void BoardcastMediNetworkConnectionChanged()
        {
            const bool ExpectedNetworkConnectionState = true;

            var expectedEvents = new ManualResetEvent(false);

            // here we test getting the MessageDispatcher to send us our class entity for a very specific Ximple Table definition
            MessageDispatcher.Instance.Subscribe<NetworkChangedMessage>(
                (sender, args) =>
                    {
                        var networkConnectionMessage = args.Message;
                        Assert.IsNotNull(networkConnectionMessage);
                        Assert.AreEqual(ExpectedNetworkConnectionState, networkConnectionMessage.WiFiConnected);
                        Debug.WriteLine(
                            "Unit Test Received Message Dispatched for NetworkConnection value = "
                            + networkConnectionMessage.WiFiConnected);
                        expectedEvents.Set();
                    });

            MessageDispatcher.Instance.Broadcast(
                new NetworkChangedMessage { WiFiConnected = ExpectedNetworkConnectionState });

            expectedEvents.WaitOne(5000);
        }

        /// <summary>The broadcast network connection changed_ connected.</summary>
        [TestMethod]
        public void BoardcastNetworkConnectionChanged_Connected()
        {
            // Table 102 is our Network Changed Table in the Dictionary.xml the value is 1 | 0 to indicate a network / wifi connection exists
            // Subscribers to the message can be done with a SimplePort and the property name see "SimplePortNetworkConnectionName"
            const string XmlString =
                "<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\"><Cells><Cell Language=\"0\" Table=\"102\" Column=\"0\" Row=\"0\">true</Cell></Cells></Ximple>";
            const bool ExpectedNetworkConnectionState = true;
            int count;
            var result = XmlHelpers.IsValidXmlDocument(XmlString, out count);
            Assert.IsTrue(result);

            Assert.IsNotNull(MessageDispatcher.Instance.LocalAddress);
            var expectedEvents = new CountdownEvent(2);

            var simplePort = XimpleProtocolFactory.CreateNetworkConnectionSimplePort();
            Assert.IsNotNull(simplePort);
            Assert.AreEqual(XimpleProtocolFactory.SimplePortNetworkConnectionName, simplePort.Info.Name);

            // setup delegate for the simplePort to also be fired with a bool value
            simplePort.ValueChanged += (sender, args) =>
                {
                    expectedEvents.Signal();
                    var connectedValue = simplePort.Value.Value;
                    Debug.WriteLine(
                        "Success Unit Test Received SimplePort message Dispatched for NetworkConnection Value="
                        + connectedValue);
                    Assert.AreEqual(ExpectedNetworkConnectionState, connectedValue != 0 ? true : false);
                };

            // here we test getting the MessageDispatcher to send us our class entity for a very specific Ximple Table definition
            MessageDispatcher.Instance.Subscribe<NetworkChangedMessage>(
                (sender, args) =>
                    {
                        var networkConnectionMessage = args.Message;
                        Assert.IsNotNull(networkConnectionMessage);
                        Assert.AreEqual(ExpectedNetworkConnectionState, networkConnectionMessage.WiFiConnected);
                        Debug.WriteLine(
                            "Success Unit Test Received Message Dispatched for NetworkConnection value = "
                            + networkConnectionMessage.WiFiConnected);
                        expectedEvents.Signal();
                    });

            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(1, 0));

            // wait for our expected events to validate
            Debug.WriteLine("Waiting for all events");
            expectedEvents.Wait(Debugger.IsAttached ? 60000 : 7000);
        }

        /// <summary>The construct default socket state.</summary>
        [TestMethod]
        public void ConstructDefaultSocketState()
        {
            var s = new SocketState(Guid.NewGuid(), SocketState.DefaultBufferSize);
            Assert.AreEqual(SocketState.DefaultBufferSize, s.BufferSize);
            Assert.IsNotNull(s.Buffer);
            Assert.AreEqual(SocketState.DefaultBufferSize, s.Buffer.Length);
            Assert.IsFalse(s.Running);
            Assert.IsNull(s.Socket);
            Assert.IsNull(s.ThreadReader);
            Assert.IsNotNull(s.SignaledExited);
            Assert.IsNotNull(s.StringBuffer);
        }

        /// <summary>The construct empty ximple xml.</summary>
        [TestMethod]
        public void ConstructEmptyXimpleXml()
        {
            var x = new Ximple(Constants.Version2);
            x.Cells.Add(new XimpleCell(string.Empty, 103, 0, 0, 0));
            var xml = x.ToXmlString();
            Debug.WriteLine(xml);

            // Construct an instance of the XmlSerializer with the type
            // of object that is being de-serialized.
            var mySerializer = new XmlSerializer(typeof(Ximple));
            var s = new StringReader(xml);

            // Call the De-serialize method and cast to the object type.
            var ximple = (Ximple)mySerializer.Deserialize(s);
            Assert.IsNotNull(ximple);
            Assert.IsTrue(ximple.Cells.Count == 1);
            Assert.AreEqual(103, ximple.Cells.First().TableNumber);
            Assert.AreEqual(0, ximple.Cells.First().ColumnNumber);
            Assert.AreEqual(0, ximple.Cells.First().RowNumber);
            Assert.IsTrue(ximple.Cells.First().Value == string.Empty);
        }

        /// <summary>The construct network ftp settings.</summary>
        [TestMethod]
        public void ConstructNetworkFtpSettings()
        {
            var n = new NetworkFtpSettings("test", "1234", new Uri("ftp://localhost/root"));
            Assert.IsNotNull(n.Uri);
            Assert.IsTrue(n.Uri.IsWellFormedOriginalString());
            var myIpv4Address = NetworkFtpSettings.GetIP4Address();
            Assert.AreEqual(myIpv4Address, n.Uri.Host);

            var n2 = new NetworkFtpSettings("test", "1234", new Uri("ftp://127.0.0.1/root"));
            Assert.IsNotNull(n2.Uri);
            Assert.AreEqual("127.0.0.1", n2.Uri.Host);

            var n3 = new NetworkFtpSettings("test", "1234", new Uri("ftp://192.1.2.3/root"));
            Assert.IsNotNull(n3.Uri);
            Assert.AreEqual("192.1.2.3", n3.Uri.Host);

            var n4 = new NetworkFtpSettings();
            Assert.IsNull(n4.Uri);
            n4.SharedUriString = "ftp://localhost/Foobar";
            Assert.AreEqual(myIpv4Address, n4.Uri.Host);
        }

        [TestMethod]
        public void ConstructVehiclePositionMessage()
        {
            var v = new VehiclePositionMessage("0.0", "0.0", "0", "R1", "T1");
            Assert.AreEqual(0.0, v.GeoCoordinate.Latitude);
            Assert.AreEqual(0.0, v.GeoCoordinate.Longitude);
            Assert.AreEqual(0.0, v.GeoCoordinate.Altitude);
            Assert.AreEqual("R1", v.Route);
            Assert.AreEqual("T1", v.Trip);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructVehiclePositionMessage_FormatException()
        {
            var v = new VehiclePositionMessage(string.Empty, "0");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructVehiclePositionMessage_FormatException2()
        {
            var v = new VehiclePositionMessage(string.Empty, string.Empty);
        }

        [TestMethod]
        public void ConstructVehiclePositionMessageWithRouteAndTrip()
        {
            var v = new VehiclePositionMessage("0", "0", "0", "R1", "T1");
            Assert.AreEqual("R1", v.Route);
            Assert.AreEqual("T1", v.Trip);
        }

        /// <summary>The construct ximple response.</summary>
        [TestMethod]
        public void ConstructXimpleResponse()
        {
            var ximpleResponse = new XimpleResponse();

            var serializer = new XmlSerializer(typeof(XimpleResponse));
            var memoryStream = new MemoryStream();
            using (var writer = new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.None })
            {
                serializer.Serialize(writer, ximpleResponse);
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                Debug.WriteLine(s);
                writer.Close();
            }
        }

        /// <summary>The construct ximple response with ximple data.</summary>
        [TestMethod]
        public void ConstructXimpleResponseWithXimpleData()
        {
            var ximpleResponse = new XimpleResponse();
            ximpleResponse.Ximple.Cells.Add(new XimpleCell("1", 106, 0, 0));
            ximpleResponse.Ximple.Cells.Add(new XimpleCell("2", 106, 0, 1));
            ximpleResponse.Ximple.Cells.Add(new XimpleCell("3", 106, 0, 2));

            var serializer = new XmlSerializer(typeof(XimpleResponse));
            var memoryStream = new MemoryStream();
            using (var writer = new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.None })
            {
                serializer.Serialize(writer, ximpleResponse);
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                Debug.WriteLine(s);
                writer.Close();
            }
        }

        /// <summary>The construct ximple socket service from ximple config xml.</summary>
        // [TestMethod]
        public void ConstructXimpleSocketServiceFromXimpleConfigXml()
        {
            using (var s = CreateXimpleSocketService())
            {
                var config = XimpleConfig.Read("XimpleConfig.xml");
                Assert.IsNotNull(config);
                Assert.AreEqual(1600, XimpleConfig.DefaultPort, "Unit test default port miss-matched");
                Assert.AreEqual(1600, config.Port, "Port mismatch");
                Assert.AreEqual(true, config.Enabled);
                Assert.AreNotEqual("localhost", config.NetworkFtpSettings.Uri.Host);

                config.Port = GetFreeTcpPort(); // for testing find a free port to use
                s.Start(IPAddress.Any, config);
                s.Stop();
            }
        }

        /// <summary>The construct ximple socket service say hello.</summary>
        [TestMethod]
        public void ConstructXimpleSocketServiceSayHello()
        {
            using (var s = CreateXimpleSocketService())
            {
                const int TestPort = 9001;
                var eventSignaled = new ManualResetEvent(false);
                s.Start(IPAddress.Any, new XimpleConfig { Port = TestPort });
                s.BadXimple += (sender, args) =>
                    {
                        Debug.WriteLine("Bad Xml  = " + args.Xml);
                        eventSignaled.Set();
                    };

                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect("127.0.0.1", TestPort);
                    if (tcpClient.Connected)
                    {
                        Debug.WriteLine("Connected");
                        var buffer = Encoding.UTF8.GetBytes("HELLO");
                        Debug.WriteLine("Client TX HELLO");
                        tcpClient.Client.Send(buffer);

                        // stay connected for a few seconds
                        Thread.Sleep(5000); // REMOVE !!!
                        Debug.WriteLine("Client Close Socket");
                        tcpClient.Close();
                    }
                }

                Assert.IsTrue(eventSignaled.WaitOne(Debugger.IsAttached ? Timeout.Infinite : 1000));
                s.Stop();
            }
        }

        /// <summary>The construct ximple socket service with no dictionary.</summary>
        [TestMethod]
        public void ConstructXimpleSocketServiceWithNoDictionary()
        {
            using (var server = new XimpleSocketService(new XimpleConfig(), null))
            {
                Assert.IsNotNull(server.XimpleConfig);
                Assert.IsNull(server.DictionaryConfigManager);
            }
        }

        /// <summary>The create default config.</summary>
        [TestMethod]
        public void CreateDefaultXimpleConfig()
        {
            const string OutputConfigFile = @"C:\Temp\DefaultXimple.xml";
            File.Delete(OutputConfigFile);
            var configMgr = new ConfigManager<XimpleConfig> { FileName = OutputConfigFile };
            configMgr.CreateConfig();
            configMgr.SaveConfig();
            Assert.IsTrue(File.Exists(OutputConfigFile));
        }

        /// <summary>The create ximple infotainment volume settings.</summary>
        [DeploymentItem(@"Config\dictionary.xml")]
        [TestMethod]
        public void CreateXimpleInfoTainmentVolumeSettings()
        {
            var response = new AudioStatusResponse();
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;
            var table = dictionary.GetTableForNameOrNumber("InfoTainmentVolumeSettings");
            Assert.IsNotNull(table, "Table not found by name");
            Assert.IsNotNull(dictionary.GetTableForNameOrNumber("100"), "Table not found by Id");

            var ximple = new Ximple(Constants.Version2);
            if (response != null)
            {
                // The audio status refresh interval
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(
                        table,
                        "AudioStatusRefreshInterval",
                        response.RefreshIntervalMiliSeconds));

                // Interior Volume Settings
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "InteriorMaximumVolume", response.Interior.MaximumVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "InteriorMinimumVolume", response.Interior.MinimumVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "InteriorDefaultVolume", response.Interior.DefaultVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "InteriorCurrentVolume", response.Interior.CurrentVolume));

                // Exterior Volume Settings
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "ExteriorMaximumVolume", response.Exterior.MaximumVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "ExteriorMinimumVolume", response.Exterior.MinimumVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "ExteriorDefaultVolume", response.Exterior.DefaultVolume));
                ximple.Cells.Add(
                    dictionary.CreateXimpleCell(table, "ExteriorCurrentVolume", response.Exterior.CurrentVolume));

                Debug.WriteLine(ximple.ToXmlString());

                Assert.AreEqual(9, ximple.Cells.Count);
            }
        }

        /// <summary>The create ximple message network settings.</summary>
        [TestMethod]
        public void CreateXimpleMessageNetworkSettings()
        {
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;
            var table = dictionary.FindXimpleTable(XimpleConfig.NetworkFileAccessSettingsTableIndexDefault);
            if (table == null)
            {
                Logger.Warn(
                    "No Such table Idx={0} found in dictionary to process",
                    XimpleConfig.NetworkFileAccessSettingsTableIndexDefault);
                return;
            }

            Debug.WriteLine("Sending Client Shared Network Folder Settings");
            var networkFtpSettings = new NetworkFtpSettings("User", "1234", new Uri("ftp://192.1.2.3/Shared"));
            var ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Uri", networkFtpSettings.SharedUriString));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "UserName", networkFtpSettings.UserName));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Password", networkFtpSettings.Password));
        }

        /// <summary>The emulate audio status.</summary>
        [TestMethod]

        // Emulate a test of receiving Ximple to request the AudioStatus Table as Ximple over the TCP Socket
        public void EmulateAudioStatus()
        {
            var manualResetEvent = new ManualResetEvent(false);

            // simulate another Gorba App giving us back Audio Status data over Medi
            // from here the data would be serialized back by the XimpleSocketService and 
            // sent as Ximple over the connected socket.
            MessageDispatcher.Instance.Subscribe<AudioStatusRequest>(
                (sender, args) =>
                    {
                        Debug.WriteLine("Success AudioStatusRequest Medi Event Signaled");

                        // emulate another Gorba Application that will handle the true
                        // request. Its job is to fire a InfoTainmentAudioStatusMessage media message so that
                        // the normal response is to reply to the connected TCP client via Ximple
                        var audioStatus =
                            new AudioStatusMessage { InteriorNoiseLevel = 123, TestActive = true }; // add random data

                        // just for testing
                        MessageDispatcher.Instance.Broadcast(audioStatus);

                        manualResetEvent.Set();
                    });

            // create Ximple message to request a Ximple response
            var cfg = new ConfigManager<Dictionary>("dictionary.xml");
            var dictionary = cfg.Config;
            var table = dictionary.GetTableForNameOrNumber("InfoTainmentAudioStatus");
            Assert.IsNotNull(table);
            var tableIndex = table.Index;

            // expect table  named InfoTainmentAudioStatus with empty cells
            var ximple = new Ximple { Cells = new List<XimpleCell> { new XimpleCell(string.Empty, tableIndex, 0, 0) } };
            var xmlString = ximple.ToXmlString();

            const int TestPort = 9080;
            this.EmulateInfoTainmentAudioStatusMessage(TestPort, xmlString);

            var signaled = manualResetEvent.WaitOne(Debugger.IsAttached ? Timeout.Infinite : 5000);
            Assert.IsTrue(signaled);
        }

        /// <summary>The emulate infotainment audio status message.</summary>
        /// <param name="port">The port.</param>
        /// <param name="xml">The xml.</param>
        public void EmulateInfoTainmentAudioStatusMessage(int port, string xml = "")
        {
            // the Service should listen for this Medi message of InfoTainmentAudioStatusMessage
            // It will subscribe to this and if a client is connected via Sockets reply
            // with a Ximple message with data from that table and send a Event that we will
            // check here for test.
            var countdownEvent = new CountdownEvent(2);
            var serverRunning = new ManualResetEvent(false);
            var serverReady = new ManualResetEvent(false);
            Thread thread = null;
            try
            {
                ThreadPool.QueueUserWorkItem(
                    m =>
                        {
                            var server = CreateXimpleSocketService(port);
                            if (server != null)
                            {
                                using (server)
                                {
                                    server.Start(IPAddress.Any);

                                    // get a event from the server class and some TCP reply as Ximple expected
                                    server.AudioStatusMessageChanged += (sender, args) =>
                                        {
                                            Debug.WriteLine(
                                                "InfotainmentAudioStatusMessageChanged Created Successfully");
                                            countdownEvent.Signal();
                                        };
                                    serverReady.Set();
                                    serverRunning.WaitOne();
                                    server.Stop();
                                }
                            }
                            else
                            {
                                Assert.Fail("CreateXimpleSocketService Failed on Port=" + port);
                            }
                        });

                if (!serverReady.WaitOne(3000))
                {
                    Assert.Fail("Failed to start tcp test server");
                }

                using (var tcpClient = new TcpClient())
                {
                    tcpClient.Connect("127.0.0.1", port);
                    Assert.IsTrue(tcpClient.Connected);
                    Thread.Sleep(250); // allow time for server to accept the connection

                    // send medi message to start the action, this medi message will be sent from UpdateManager in future
                    if (string.IsNullOrEmpty(xml))
                    {
                        Debug.WriteLine("Test = Broadcasting Medi Message InfoTainmentAudioStatusMessage");
                        var o = new AudioStatusMessage();
                        MessageDispatcher.Instance.Broadcast(o);
                    }

                    // read the socket for Ximple Data that would go back to the client
                    ThreadPool.QueueUserWorkItem(
                        m =>
                            {
                                thread = Thread.CurrentThread;
                                if (tcpClient != null)
                                {
                                    try
                                    {
                                        tcpClient.SendTimeout = 30000;

                                        if (string.IsNullOrEmpty(xml) == false)
                                        {
                                            Debug.WriteLine("Test = TCP Client writting XML Ximple out to server....");
                                            var data = Encoding.UTF8.GetBytes(xml);
                                            tcpClient.Client.Send(data, 0, data.Length, SocketFlags.None);
                                            Thread.Sleep(500);
                                        }

                                        var buffer = new byte[2048];
                                        tcpClient.Client.Blocking = true;
                                        tcpClient.ReceiveBufferSize = buffer.Length;
                                        var bytesReady = tcpClient.Client.Available;
                                        tcpClient.ReceiveTimeout = Debugger.IsAttached ? Timeout.Infinite : 5000;
                                        var bytesReceived =
                                            tcpClient.Client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                                        if (bytesReceived > 0)
                                        {
                                            DebugOutputServerResponce(buffer, bytesReceived);
                                            countdownEvent.Signal();
                                            Thread.Sleep(
                                                500); // allow some time to process the server thread in the unit test above
                                        }
                                    }
                                    catch (SocketException s)
                                    {
                                        if (s.SocketErrorCode != SocketError.TimedOut)
                                        {
                                            throw;
                                        }
                                    }
                                }
                            });

                    var signaled = countdownEvent.Wait(Debugger.IsAttached ? Timeout.Infinite : 5000);
                    Assert.IsTrue(signaled);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                serverRunning.Set();
                thread.Join(1000);
            }
        }

        /// <summary>The emulate infotainment audio status message medi.</summary>
        [TestMethod]
        public void EmulateInfoTainmentAudioStatusMessageMedi()
        {
            const int TestPort = 9500;

            // the Service should listen for this Medi message of InfoTainmentAudioStatusMessage
            // It will subscribe to this and if a client is connected via Sockets reply
            // with a Ximple message with data from that table and send a Event that we will
            // check here for test.
            var countdownEvent = new CountdownEvent(1);
            var server = CreateXimpleSocketService(TestPort);

            try
            {
                server.Start(IPAddress.Any);

                // subscribe to server event that is fired on Medi message
                server.AudioStatusMessageChanged += (sender, args) =>
                    {
                        Debug.WriteLine("InfoTainmentAudioStatusMessageChanged Created Successfully");
                        countdownEvent.Signal();
                    };

                // emulate some app broadcasting the event we want our server to process
                MessageDispatcher.Instance.Broadcast(new AudioStatusMessage());

                var signaled = countdownEvent.Wait(Debugger.IsAttached ? Timeout.Infinite : 2000);
                Assert.IsTrue(signaled);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                server.Stop();
            }
        }

        /// <summary>The emulate infotainment audio status message.</summary>
        [TestMethod]
        public void EmulateInfoTainmentAudioStatusMessageTest()
        {
            const int TestPort = 9190;
            this.EmulateInfoTainmentAudioStatusMessage(TestPort);
        }

        /// <summary>The hardware manager medi message.</summary>
        [TestMethod]
        public void HardwareManagerMediMessage()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var mediaAddress = MessageDispatcher.Instance.LocalAddress;
            MessageDispatcher.Instance.Subscribe<AudioStatusRequest>(
                (sender, args) =>
                    {
                        Debug.WriteLine("AudioStatusRequest Medi Event Signaled");
                        manualResetEvent.Set();
                    });

            this.SendAudioStatusRequestMessage(mediaAddress.Application);

            var signaled = manualResetEvent.WaitOne(3000);
            Assert.IsTrue(signaled);
        }

        /// <summary>
        ///     Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
        }

        /// <summary>Test if the string xml is invalid.</summary>
        [TestMethod]
        public void IsValidXmlDocument_TestBadXml()
        {
            const string XmlString = @"<book genre='autobiography' publicationdate='1981-03-22' ISBN='1-861003-11-0'>
                    <title>The Autobiography of Benjamin Franklin</title>
                    <author>
                        <first-name>Benjamin</first-name>
                        <last-name>Franklin</last-name>
                    </author>
                    <price>8.99</price>
                </book>
            </bookstore>";
            IsValidXmlDocument(XmlString, 0);
        }

        /// <summary>The is valid xml document test.</summary>
        [TestMethod]
        public void IsValidXmlDocumentTest()
        {
            const string XmlString = @"<?xml version='1.0'?><bookstore>
                <book genre='autobiography' publicationdate='1981-03-22' ISBN='1-861003-11-0'>
                    <title>The Autobiography of Benjamin Franklin</title>
                    <author>
                        <first-name>Benjamin</first-name>
                        <last-name>Franklin</last-name>
                    </author>
                    <price>8.99</price>
                </book>
            </bookstore>";
            var result = IsValidXmlDocument(XmlString, 1);
            Assert.IsTrue(result);
        }

        /// <summary>The multiple connections test.</summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCount" /> is less than 0.</exception>
        [TestMethod]
        public void MultipleConnectionsTestBogusContent()
        {
            const int ExpectedCount = 3;
            var badDataCount = 0;
            const int Port = 9095;
            var running = new ManualResetEvent(false);
            var countdownEvent = new CountdownEvent(ExpectedCount);
            Thread thread = null;
            try
            {
                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            thread = Thread.CurrentThread;
                            try
                            {
                                // start the server in background to receive
                                var xmlConfig = new XimpleConfig();
                                using (var server = new XimpleSocketService(xmlConfig, null))
                                {
                                    server.BadXimple += (sender, args) =>
                                        {
                                            badDataCount++;
                                            countdownEvent.Signal();
                                            Debug.WriteLine("Server received [" + args.Xml + "] count=" + badDataCount);
                                        };
                                    server.Start(IPAddress.Any, new XimpleConfig { Port = Port });
                                    Debug.WriteLine("Server ready and waiting");

                                    running.WaitOne();
                                    Debug.WriteLine("Server Stop");
                                }
                            }
                            catch (ThreadAbortException)
                            {
                            }

                            Debug.WriteLine("Server Terminated");
                        },
                    running);

                Thread.Sleep(500); // so I can see the server waiting sleep, for debugging only

                for (var i = 0; i < ExpectedCount; i++)
                {
                    Debug.WriteLine("Client Connecting");
                    var tcpClient = new TcpClient();
                    tcpClient.Connect("127.0.0.1", Port);
                    Thread.Sleep(250);
                    if (tcpClient.Connected)
                    {
                        tcpClient.NoDelay = true;
                        Debug.WriteLine("Client is Connected");
                        {
                            var s = "Hello " + i;
                            Debug.WriteLine("Client Sending " + s);
                            var buffer = Encoding.UTF8.GetBytes(s);
                            var bytesSent = tcpClient.Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                            Assert.AreEqual(bytesSent, buffer.Length);
                            Thread.Sleep(100);
                        }

                        Debug.WriteLine("Shut client socket down");
                        tcpClient.Close();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                Debug.WriteLine("Unit Test Signal  Not Running");
                var signaled = countdownEvent.Wait(10000);
                Assert.AreEqual(ExpectedCount, badDataCount);
                Assert.IsTrue(signaled);
                running.Set();
                thread.Join(1000);
            }
        }

        /// <summary>
        ///     Create server then have multiple writes of Ximple data
        /// </summary>
        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        [TestMethod]
        public void MultipleXimpleWritesTestStayOpen()
        {
            {
                // lock (socketTestLock) only necessary if we share the same port
                const int ExpectedCount = 3;
                var validXimpleCount = 0;
                var badDataCount = 0;
                const int Port = 9010;
                var running = new ManualResetEvent(false);
                var serverDone = new ManualResetEvent(false);
                var countdownEvent = new CountdownEvent(ExpectedCount);
                var countdownXimpleEvent = new CountdownEvent(ExpectedCount);
                var serverStarted = new ManualResetEvent(false);
                var signaled = false;

                const string XmlString =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"0\" Row=\"0\">1 LUMINATOR INBOUND</Cell>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"8\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"9\" Row=\"0\">2</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"0\" Row=\"0\">11</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>\r\n</Cells>\r\n</Ximple>";
                try
                {
                    ThreadPool.QueueUserWorkItem(
                        o =>
                            {
                                // start the server in background to receive
                                using (var server = new XimpleSocketService())
                                {
                                    server.BadXimple += (sender, args) =>
                                        {
                                            Debug.WriteLine("Server  received Bogus Data [" + args.Xml + "]");
                                            countdownEvent.Signal();
                                            badDataCount++;
                                        };
                                    server.XimpleCreated += (sender, args) =>
                                        {
                                            validXimpleCount++;
                                            Debug.WriteLine(
                                                "Server Success got valid Ximple Count=" + validXimpleCount);
                                            countdownXimpleEvent.Signal();
                                        };

                                    server.Start(IPAddress.Any, new XimpleConfig(Port));
                                    Debug.WriteLine("Server ready and waiting");
                                    serverStarted.Set();

                                    // wait for server to be stopped by our test
                                    running.WaitOne();
                                    Debug.WriteLine("Server Stop");
                                    server.Stop();
                                }

                                Debug.WriteLine("Server Terminated");
                                serverDone.Set();
                            },
                        running);

                    serverStarted.WaitOne(3000); // so I can see the server waiting sleep, for debugging only

                    var tcpClient = new TcpClient();
                    tcpClient.Connect("127.0.0.1", Port);
                    tcpClient.NoDelay = true;
                    if (tcpClient.Connected)
                    {
                        Debug.WriteLine("Client is Connected");

                        for (var i = 0; i < ExpectedCount; i++)
                        {
                            Debug.WriteLine("Client Sending Ximple...String Length={0}, Try={1}", XmlString.Length, i);
                            var buffer = Encoding.UTF8.GetBytes(XmlString);

                            var bytesSent = tcpClient.Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                            Assert.AreEqual(bytesSent, buffer.Length);

                            Thread.Sleep(100); // blocks of time for writes to see Server not read and get data
                        }

                        signaled = countdownXimpleEvent.Wait(5000);

                        Debug.WriteLine("Shut client socket down");
                        tcpClient.Close();
                    }
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    Debug.WriteLine("Unit Test Signal  Not Running");
                    running.Set();
                    serverDone.WaitOne(1000);
                }

                Assert.AreEqual(ExpectedCount, validXimpleCount);
            }
        }

        /// <summary>The read update config.</summary>
        [TestMethod]
        public void ReadUpdate2Config()
        {
            var updateConfig = UpdateConfig.Read("XimpleConfigTest2.xml");
            Assert.IsNotNull(updateConfig);
            var ftpClient = updateConfig.Clients.FirstOrDefault(m => m.Name.Contains("FTP")) as FtpUpdateClientConfig;
            Assert.IsNotNull(ftpClient);
            Assert.IsFalse(string.IsNullOrEmpty(ftpClient.Name));
            Assert.IsFalse(string.IsNullOrEmpty(ftpClient.LocalFtpHomePath));
            Assert.AreEqual(@"D:\homeftp", ftpClient.LocalFtpHomePath);
        }

        /// <summary>The read update config.</summary>
        [TestMethod]
        public void ReadUpdateConfig()
        {
            var updateConfig = UpdateConfig.Read("Update.xml");
            Assert.IsNotNull(updateConfig);
            var ftpClient = updateConfig.Clients.FirstOrDefault(m => m.Name.Contains("FTP")) as FtpUpdateClientConfig;
            Assert.IsNotNull(ftpClient);
            Assert.IsFalse(string.IsNullOrEmpty(ftpClient.Name));
            Assert.IsFalse(string.IsNullOrEmpty(ftpClient.LocalFtpHomePath));
        }

        // I am testing here the use of the Update.xml as I will need this file/data later in Ximple Server
        // The unit test and source test file could be relocated to a different test project.

        /// <summary>The read update config xml.</summary>
        [TestMethod]
        public void ReadUpdateConfigXml()
        {
            Assert.IsTrue(File.Exists("Update.xml"));
            var updateConfig = UpdateConfig.Read("Update.xml");
            Assert.IsNotNull(updateConfig);
        }

        /// <summary>The read update config xml default.</summary>
        [TestMethod]
        public void ReadUpdateConfigXmlDefault()
        {
            var updateConfig = UpdateConfig.Read();
            Assert.IsNotNull(updateConfig);
        }

        /// <summary>The read update config xml instance.</summary>
        [TestMethod]
        public void ReadUpdateConfigXmlInstance()
        {
            var updateConfig = UpdateConfig.Read();
            Assert.IsNotNull(updateConfig);
            Assert.IsNotNull(updateConfig.Agent);
            Assert.IsNotNull(updateConfig.Clients);
            Assert.IsNotNull(updateConfig.Providers);
            Assert.IsNotNull(updateConfig.Visualization);
            Assert.IsNotNull(updateConfig.CacheLimits);
        }

        /// <summary>The read test ximple config.</summary>
        /// <exception cref="UriFormatException">The Uri passed from the constructor is invalid. </exception>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        [DeploymentItem(@"Config\XimpleConfigTest.xml")]
        public void ReadXimpleConfigFile()
        {
            var configManager =
                new ConfigManager<XimpleConfig>("XimpleConfigTest.xml") { XmlSchema = XimpleConfig.Schema };
            var config = configManager.Config;
            Assert.IsNotNull(config.NetworkFtpSettings);
            Assert.IsNotNull(config.AudioZonePresentationValues);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Interior);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Exterior);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Both);
            var uriFile = new Uri(@"file://192.168.1.0/Shared", UriKind.Absolute);

            Assert.IsNotNull(config);
            var uri = new Uri(@"ftp://192.1.2.3/Root", UriKind.Absolute);
            Assert.AreEqual("gorba", config.NetworkFtpSettings.UserName);
            Assert.AreEqual("Asdf1234", config.NetworkFtpSettings.Password);
            Assert.AreEqual(uri.ToString(), config.NetworkFtpSettings.Uri.ToString());
            Assert.AreEqual(@"ftp://192.1.2.3/Root", config.NetworkFtpSettings.SharedUriString);
            Assert.AreEqual(@"ftp://192.1.2.3/Root", config.NetworkFtpSettings.Uri.ToString());
            Assert.AreEqual(1600, config.Port);

            // verify this will construct
            config.NetworkFtpSettings.Uri = new Uri("ftp://localhost/Root");
            Assert.IsTrue(config.NetworkFtpSettings.Uri.IsWellFormedOriginalString());
            Assert.AreNotEqual("localhost", config.NetworkFtpSettings.Uri.Host);
        }

        /// <summary>The read ximple config helper test.</summary>
        [TestMethod]
        public void ReadXimpleConfigHelperTest()
        {
            var config = XimpleConfig.Read("XimpleConfig.xml");

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Port > 0, "Port is invalid");
            Assert.AreEqual(true, config.Enabled, "Protocol is Not Enabled");
            Assert.AreEqual(false, config.EnableResponse, "EnableResponse is disabled");
            Assert.AreEqual(false, config.EnableXimpleOnlyResponse, "EnableXimpleOnlyResponse is disabled");
            Assert.AreEqual(
                XimpleConfig.InfoTainmentVolumeSettingsTableIndexDefault,
                config.InfoTainmentVolumeSettingsTableIndex);
            Assert.AreEqual(
                XimpleConfig.InfoTainmentCannedMsgPlayTableIndexDefault,
                config.InfoTainmentCannedMsgPlayTableIndex);
            Assert.AreEqual(
                XimpleConfig.NetworkChangedMessageTableIndexDefault,
                config.NetworkChangedMessageTableIndex);
            Assert.AreEqual(
                XimpleConfig.NetworkFileAccessSettingsTableIndexDefault,
                config.NetworkFileAccessSettingsTableIndex);
            Assert.AreEqual(
                XimpleConfig.InfoTainmentAudioStatusTableIndexDefault,
                config.InfoTainmentAudioStatusTableIndex);

            Assert.IsNotNull(config.NetworkFtpSettings);
            Assert.IsNotNull(config.AudioZonePresentationValues);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Interior);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Exterior);
            Assert.AreNotEqual(string.Empty, config.AudioZonePresentationValues.Both);
            Assert.IsNotNull(config.NetworkFtpSettings);
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendGpsCoordinatesAsXimple()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("InfoTainmentSystemStatus");
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell("33.050769", table.Index, 0, 15, 0)); // latitude
            ximple.Cells.Add(new XimpleCell("-96.747944", table.Index, 0, 16, 0)); // longitude

            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            var manualEvent = new ManualResetEvent(false);
            MessageDispatcher.Instance.Subscribe<VehicleUnitInfo>(
                (sender, args) =>
                    {
                        if (args.Message?.VehiclePosition?.GeoCoordinate.Latitude == 33.050769
                            && args.Message.VehiclePosition.GeoCoordinate.Longitude == -96.747944)
                        {
                            Debug.WriteLine("Received medi VehiclePositionMessage event " + args.Message);
                            manualEvent.Set();
                        }
                    });

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));

            var signaled = manualEvent.WaitOne(5000);
            Assert.IsTrue(signaled);
        }

        /// <summary>Send multiple ximple messages together. Save Ximple as Xml docs for testing </summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendMultipleXimpleMessagesTogether()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"0\" Row=\"0\">1 LUMINATOR INBOUND</Cell>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"8\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"9\" Row=\"0\">2</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"0\" Row=\"0\">11</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"8\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"9\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"0\" Row=\"0\">1 LUMINATOR INBOUND</Cell>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"8\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"9\" Row=\"0\">2</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"0\" Row=\"0\">11</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"12\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"12\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"12\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"12\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">TECHNOLOGY &amp; RESOURCE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">CAPITAL &amp; N AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">SUMMIT &amp; STEWART</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; MATRIX</Cell>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">STEWART &amp; CAPITAL</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">SUMMIT &amp; STEWART</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">SUMMIT &amp; STEWART</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">SUMMIT &amp; STEWART</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">JUPITER RD &amp; SUMMIT AVE</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">SUMMIT &amp; MATRIX</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"5\" Row=\"0\">1</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">LUMINATOR</Cell>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>\r\n\r\n<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"11\" Column=\"8\" Row=\"0\">0</Cell>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"9\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"10\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"1\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"2\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"3\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"4\"/>\r\n  <Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"5\"/>\r\n</Cells>\r\n</Ximple>";
            Assert.IsFalse(IsValidXmlDocument(XmlString, 18));
            const bool WriteXimpleAsXmlFiles = false;
            if (WriteXimpleAsXmlFiles && Directory.Exists(@"C:\Temp\Ximple") == false)
            {
                Directory.CreateDirectory(@"C:\Temp\Ximple");
            }

            this.SendXmlAsXimple(
                XmlString,
                new XimpleProtocolTestConfig(18, 0, 1) { SaveToFile = WriteXimpleAsXmlFiles });
        }

        /// <summary>The send request for network settings as ximple.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        [DeploymentItem(@"Config\Update.xml")]
        [DeploymentItem(@"Config\XimpleConfig.xml")]
        public void SendRequestForNetworkFileAccessSettingsAsXimple()
        {
            // source test files needed
            Assert.IsTrue(
                File.Exists("dictionary.xml")); // used by the server to build the Ximple reply for this client
            Assert.IsTrue(File.Exists("Update.xml")); // Contains the settings we want 'SharedFolderConfig' 
            Assert.IsTrue(File.Exists("XimpleConfig.xml")); // Server Ximple runtime settings

            var signaledReceived = new ManualResetEvent(false);
            this.XimpleSocketDataReceived += (sender, s) =>
                {
                    Debug.WriteLine("Successful event, Server replied on the socket");
                    signaledReceived.Set();
                };
            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("NetworkFileAccessSettings");
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            // we will expect a reply from the server from our one positive Tx case
            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0, 1, true));
            Assert.IsTrue(signaledReceived.WaitOne(10000));
        }

        /// <summary>The send request for network shared folder settings as ximple without update xml file.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendRequestForNetworkFileAccessSettingsAsXimpleWithoutUpdateXmlFile()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("NetworkFileAccessSettings");
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell(string.Empty, table.Index, 0, 0, 0));

            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendRequestForVolumeSettings()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("InfoTainmentVolumeSettings");
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell(string.Empty, table.Index, 0, 0, 0));

            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendRequestForVolumeSettingsAutoRefreshInterval()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("InfoTainmentVolumeSettings");
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell("1000", table.Index, 0, 0, 0));

            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));
        }

        /// <summary>The send single valid ximple messages_ bad version_1_1.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]

        // <summary>The send single valid ximple messages_ bad version_1_1.</summary>
        public void SendSingleValidXimpleMessages_BadVersion_1_1()
        {
            // capture one bad xml event with a know bad version. 
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"1.1\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(0, 1));
        }

        /// <summary>The send valid xml ximple messages.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendSingleValidXimpleMessages_V2_0()
        {
            // capture one good xml event with a know 2.0 version.  Note: The true version is '2.0.0' so here we are relaxing the rule
            // with a code change to the Ximple class to accept 2.0 as 2.0.0
            // Change this test if we do not accept '2.0' and require '2.0.0'
            // see code  Gorba.Common.Protocols.Ximple.Utils.Constants.Version2
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(0, 1));
        }

        /// <summary>The send single valid ximple messages_ v 2_0_0.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendSingleValidXimpleMessages_V2_0_0()
        {
            // expect one good event
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(1, 0));
        }

        /// <summary>The send single valid ximple network changed messages_ v 2_0.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendSingleValidXimpleNetworkChangedMessages_V2_0()
        {
            // capture one good xml event with a know 2.0 version.  Note: The true version is '2.0.0' so here we are relaxing the rule
            // with a code change to the Ximple class to accept 2.0 as 2.0.0
            // Change this test if we do not accept '2.0' and require '2.0.0'
            // see code  Gorba.Common.Protocols.Ximple.Utils.Constants.Version2
            var ximple = new Ximple { Cells = new List<XimpleCell> { new XimpleCell("1", 102, 0, 0) } };
            var xml = ximple.ToXmlString();
            var manualEvent = new ManualResetEvent(false);

            MessageDispatcher.Instance.Subscribe<NetworkChangedMessage>(
                (sender, args) =>
                    {
                        Debug.WriteLine("Received medi NetworkChangedMessage event");
                        manualEvent.Set();
                    });

            // expect one good Ximple Created message, no bad
            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));

            var signaled = manualEvent.WaitOne(5000);
            Assert.IsTrue(signaled);
        }

        /// <summary>The send ximple two valid xml messages.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendTwoValidXimpleMessages()
        {
            // expect two good events
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple><?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(2, 0));
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendVehicleRouteAndTripAsXimple()
        {
            // source test files needed
            Assert.IsTrue(File.Exists("dictionary.xml"));

            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber(XimpleConfig.RouteTableIndex.ToString());
            Assert.IsNotNull(table, "Table in dictionary undefined");
            ximple.Cells.Add(new XimpleCell("R1", table.Index, 0, 9, 0)); // Route
            ximple.Cells.Add(new XimpleCell("T1", table.Index, 0, 6, 0)); // Trip

            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);

            var manualEvent = new ManualResetEvent(false);
            MessageDispatcher.Instance.Subscribe<VehicleUnitInfo>(
                (sender, args) =>
                    {
                        if (args.Message != null && args.Message.VehiclePosition.Route == "R1"
                            && args.Message.VehiclePosition.Trip == "T1")
                        {
                            Debug.WriteLine("Received medi VehiclePositionMessage event " + args.Message);
                            manualEvent.Set();
                        }
                    });

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));

            var signaled = manualEvent.WaitOne(5000);
            Assert.IsTrue(signaled);
        }

        /// <summary>The send ximple leading garbage in messages.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendXimpleLeadingGarbageInMessages()
        {
            const string XmlString =
                "GarbageData<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple><?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(2, 0));
        }

        /// <summary>The send ximple messages multiple calls.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendXimpleMessagesMultipleClientCalls()
        {
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            const int ExpectedXimpleCreated = 3;
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(ExpectedXimpleCreated, 0, 3));
        }

        /// <summary>The send ximple missing closeing tag.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SendXimpleMissingCloseingTag()
        {
            // Expect one good ximple creation
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple><?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Co";
            this.SendXmlAsXimple(XmlString, new XimpleProtocolTestConfig(1, 0));
        }

        /// <summary>The unit test helper to process xml as ximple.</summary>
        /// <param name="xmlString">The xml string.</param>
        /// <param name="testConfig">The test Config.</param>
        /// <exception cref="SocketException">An error occurred when attempting to access the socket.</exception>
        public void SendXmlAsXimple(string xmlString, XimpleProtocolTestConfig testConfig)
        {
            var expectedXimpleCreatedCount = testConfig.TestExpectedXimpleCreatedCount;
            var expectedBadXmlCount = testConfig.TestExpectedBadXmlCount;
            var repeatSocketWriteCount = testConfig.RepeatSocketWriteCount;
            var port = testConfig.Port;
            var saveXimpleToFile = testConfig.SaveToFile;
            var waitForSocketReply = testConfig.WaitForReply;

            // test values to count signaled events from server
            var ximpleCreatedCount = 0;
            var ximpleBadCount = 0;
            var expectedSignaled = false;

            // setup countdown events to wait for the server to respond
            var ximpleCreatedCountdownEvents = expectedXimpleCreatedCount > 0
                                                   ? new CountdownEvent(expectedXimpleCreatedCount)
                                                   : null;
            var expectedBadXmlCountEvents = expectedBadXmlCount > 0 ? new CountdownEvent(expectedBadXmlCount) : null;

            lock (socketTestLock)
            {
                if (port <= 0)
                {
                    port = GetFreeTcpPort();
                }

                // }

                // {
                Debug.WriteLine("Using Socket Port " + port);

                // one test at a time through the socket server test to avoid conflicts on the same port 
                Assert.IsTrue(port >= 1000, "Invalid test socket Port");

                Logger.Info(
                    "ProcessXmlAsXimple() Enter, Port={0}, Thread=0x{1:X}, totalTcpClientWrites={2}",
                    port,
                    Thread.CurrentThread.ManagedThreadId,
                    repeatSocketWriteCount);

                // create server to start listening for our Xml data
                using (var s = CreateXimpleSocketService(port))
                {
                    s.Start(IPAddress.Any, new XimpleConfig(port));
                    s.BadXimple += (sender, args) =>
                        {
                            // count events of bad XML
                            if (expectedBadXmlCountEvents != null)
                            {
                                expectedBadXmlCountEvents.Signal();
                            }

                            ximpleBadCount++;
                            Debug.WriteLine(
                                "Success - Test Received Event - Bad Xml!!!  = {0}, ximpleBadCount={1}",
                                args.Xml,
                                ximpleBadCount);
                        };

                    s.XimpleCreated += (sender, args) =>
                        {
                            // count good Ximple creations
                            var xml = args.Ximple.ToXmlString();
                            if (ximpleCreatedCountdownEvents != null)
                            {
                                if (ximpleCreatedCountdownEvents.CurrentCount > 0)
                                {
                                    ximpleCreatedCountdownEvents.Signal();
                                }
                            }

                            ximpleCreatedCount++;
                            Debug.WriteLine(
                                ximpleCreatedCount + " Test Received Event - Good Ximple  = " + xml
                                + "\r\n----------------------");

                            // Optionally save off as separate files for further testing usage
                            if (saveXimpleToFile)
                            {
                                if (!Directory.Exists(@"C:\Temp\Ximple"))
                                {
                                    Directory.CreateDirectory(@"C:\Temp\Ximple");
                                }

                                var xmlDocument = new XmlDocument();
                                xmlDocument.LoadXml(xml);
                                using (var fileStream =
                                    File.Create(
                                        Path.Combine(
                                            @"C:\Temp\Ximple",
                                            string.Format("Ximple{0}.xml", ximpleCreatedCount))))
                                {
                                    xmlDocument.Save(fileStream);
                                }
                            }

                            if (this.XimpleCreated != null)
                            {
                                this.XimpleCreated(sender, args);
                            }
                        };

                    s.AudioStatusMessageChanged += (sender, message) =>
                        {
                            Debug.WriteLine("Test Client received InfoTainment AudioStatusMessageChanged");
                            if (this.AudioStatusMessageChanged != null)
                            {
                                this.AudioStatusMessageChanged(sender, message);
                            }
                        };

                    // create a test client emulating traffic
                    using (var tcpClient = new TcpClient())
                    {
                        tcpClient.Connect("127.0.0.1", port);
                        if (tcpClient.Connected)
                        {
                            Debug.WriteLine(
                                "TCP Client Connected to Server on  " + tcpClient.Client.LocalEndPoint + " - "
                                + DateTime.Now.ToLongTimeString());
                            var buffer = Encoding.UTF8.GetBytes(xmlString);

                            var responseBuffer = new byte[32786];
                            tcpClient.Client.ReceiveTimeout = Debugger.IsAttached ? Timeout.Infinite : 3000;

                            // for testing allow multiple writes of the same data
                            for (var i = 0; i < repeatSocketWriteCount; i++)
                            {
                                Debug.WriteLine(
                                    "Test Client Sending XML data to server.... Pass " + (i + 1) + " bytes to Tx = "
                                    + buffer.Length + " - " + DateTime.Now.ToLongTimeString());
                                var bytesSent = tcpClient.Client.Send(buffer);
                                if (bytesSent > 0)
                                {
                                    // try to read ximple/socket reply in some cases we expect No reply!
                                    if (waitForSocketReply)
                                    {
                                        Debug.WriteLine("Waiting for Server to Reply...");
                                        try
                                        {
                                            var bytesReceived = tcpClient.Client.Receive(
                                                responseBuffer,
                                                0,
                                                responseBuffer.Length,
                                                SocketFlags.None);

                                            if (bytesReceived > 0)
                                            {
                                                var ximpleString =
                                                    Encoding.UTF8.GetString(responseBuffer, 0, bytesReceived);
                                                Debug.WriteLine("TCP Client Received XML as = [" + ximpleString + "]");
                                                if (this.XimpleSocketDataReceived != null)
                                                {
                                                    // expected event in test case if we want to expect a reply
                                                    this.XimpleSocketDataReceived(this, ximpleString);
                                                }
                                            }
                                        }
                                        catch (SocketException ex)
                                        {
                                            Debug.WriteLine(ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        // allow time for server to get the socket received and process
                                        Thread.Sleep(10);
                                    }
                                }
                            }

                            // Delay here to allow the server code time to execute else it will Dispose/Close and expect no work from it or events fired

                            // Debug and Test Results based on our expected events 
                            // based on our test expectations wait with a timeout for the signaled events
                            var timeout = Debugger.IsAttached ? Timeout.Infinite : 15000;

                            Debug.WriteLine(
                                "\r\n*** Client wait for server to signal Timeout = " + timeout + " - "
                                + DateTime.Now.ToLongTimeString());

                            if (expectedXimpleCreatedCount > 0 && expectedBadXmlCount <= 0)
                            {
                                Debug.WriteLine("Client Waiting for only all XimpleCreated events");
                                Debug.Assert(
                                    ximpleCreatedCountdownEvents != null,
                                    "ximpleCreatedCountdownEvents != null");
                                expectedSignaled = ximpleCreatedCountdownEvents.Wait(timeout);
                            }
                            else if (expectedXimpleCreatedCount <= 0 && expectedBadXmlCount > 0)
                            {
                                Debug.WriteLine("Client Waiting for only all Not or Bad Xml events");
                                Debug.Assert(expectedBadXmlCountEvents != null, "expectedBadXmlCountEvents != null");
                                expectedSignaled = expectedBadXmlCountEvents.Wait(timeout);
                            }
                            else
                            {
                                Debug.WriteLine("Client Waiting for both XimpleCreated & Bad Xml events");
                                Debug.Assert(
                                    ximpleCreatedCountdownEvents != null,
                                    "ximpleCreatedCountdownEvents != null");
                                Debug.Assert(expectedBadXmlCountEvents != null, "expectedBadXmlCountEvents != null");
                                if (ximpleCreatedCountdownEvents != null && expectedBadXmlCountEvents != null)
                                {
                                    expectedSignaled = WaitHandle.WaitAll(
                                        new[]
                                            {
                                                ximpleCreatedCountdownEvents.WaitHandle,
                                                expectedBadXmlCountEvents.WaitHandle
                                            },
                                        timeout);
                                }
                            }

                            Debug.WriteLine("End of run ximpleCreatedCount=" + ximpleCreatedCount);
                            Debug.WriteLine("ximpleBadCount=" + ximpleBadCount);
                            Debug.WriteLine(
                                "Client Signaled Event = {0} - {1}",
                                expectedSignaled,
                                DateTime.Now.ToLongTimeString());
                        }

                        Debug.WriteLine("Tcp Test Client socket closed");

                        tcpClient.Close();
                    }

                    // Dispose will call Stop() on the server
                    s.Dispose();
                }
            }

            Debug.WriteLine("---------------- Waiting for expected signals for test ---------------");
            Assert.IsTrue(expectedSignaled, "expectedSignaled WaitHandles not set. Expected events not received");
            Assert.AreEqual(
                expectedXimpleCreatedCount,
                ximpleCreatedCount,
                "expectedXimpleCreatedCount Expected Good Ximple Messages");
            Assert.AreEqual(expectedBadXmlCount, ximpleBadCount, "expectedBadXmlCount Expected Bad Ximple Messages");

            Trace.TraceInformation(
                "Client ProcessXmlAsXimple() Exit, Port={0}, Thread=0x{1:X}",
                port,
                Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>The serialize network changed message.</summary>
        [TestMethod]
        public void SerializeNetworkChagnedMessage()
        {
            var s = SerializeAsXml<NetworkChangedMessage>();
            Assert.IsNotNull(s);
        }

        /// <summary>The serialize NetworkFtpSettings class.</summary>
        [TestMethod]
        public void SerializeNetworkFtpSettings()
        {
            var s = SerializeAsXml<NetworkFtpSettings>();
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void SerializeVehiclePositionMessage()
        {
            var s = SerializeAsXml<VehiclePositionMessage>();
            Assert.IsNotNull(s);
        }

        /// <summary>The serialize volume settings response.</summary>
        [TestMethod]
        public void SerializeVolumeSettingsResponse()
        {
            var response = new AudioStatusResponse();
            Assert.IsNotNull(response.Interior);
            Assert.IsNotNull(response.Exterior);
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void SetXimpleCellValueTest()
        {
            var ximple = new Ximple(Constants.Version2);
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var dictionary = cfg.Config;

            var table = dictionary.GetTableForNameOrNumber("InfoTainmentCannedMsgPlay");
            Assert.IsNotNull(table);
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMessageID", "1"));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMessageZone", "0"));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMessageFileName", "test.mp3"));
            Assert.AreEqual(3, ximple.Cells.Count);

            var audioZoneValue = ximple.Cells.FindFirstXimpleCellValue(table, "CannedMessageZone");
            Assert.AreEqual("0", audioZoneValue);

            ximple.Cells.SetXimpleCellValue(table, "CannedMessageZone", "Interior");
            var audioZoneValue2 = ximple.Cells.FindFirstXimpleCellValue(table, "CannedMessageZone");
            Assert.AreEqual("Interior", audioZoneValue2);
        }

        /// <summary>The test XimpleConfig class Default de-serialization.</summary>
        /// <exception cref="FileNotFoundException">If the config file is not found</exception>
        [TestMethod]
        [DeploymentItem(@"Config\XimpleConfig.xml")]
        public void TestDeserializationXimpleConfig()
        {
            var configMgr =
                new ConfigManager<XimpleConfig> { FileName = "XimpleConfig.xml", XmlSchema = XimpleConfig.Schema };
            var config = configMgr.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Port > 0, "Port is invalid");
            Assert.AreEqual(true, config.Enabled, "Protocol is Not Enabled");
        }

        /// <summary>The test XimpleConfig class Default de-serialization.</summary>
        /// <exception cref="FileNotFoundException">If the config file is not found</exception>
        [TestMethod]
        [DeploymentItem("Config\\XimpleConfig.1.0.xml")]
        public void TestDeserializationXimpleConfig_1_0()
        {
            var configMgr =
                new ConfigManager<XimpleConfig> { FileName = "XimpleConfig.1.0.xml", XmlSchema = XimpleConfig.Schema };
            var config = configMgr.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.Port > 0, "Port is invalid");
            Assert.AreEqual(true, config.Enabled, "Protocol is Not Enabled");
            Assert.AreEqual(false, config.EnableResponse, "EnableResponse is Enabled");
            Assert.AreEqual(
                XimpleConfig.InfoTainmentAudioStatusTableIndexDefault,
                config.InfoTainmentAudioStatusTableIndex);
            Assert.AreEqual(
                XimpleConfig.NetworkChangedMessageTableIndexDefault,
                config.NetworkChangedMessageTableIndex);
            Assert.AreEqual(
                XimpleConfig.InfoTainmentVolumeSettingsTableIndexDefault,
                config.InfoTainmentVolumeSettingsTableIndex);
            Assert.AreEqual(
                XimpleConfig.NetworkFileAccessSettingsTableIndexDefault,
                config.NetworkFileAccessSettingsTableIndex);
            Assert.AreEqual(
                XimpleConfig.InfoTainmentAudioStatusTableIndexDefault,
                config.InfoTainmentAudioStatusTableIndex);
        }

        /// <summary>The test is not valid xml document.</summary>
        [TestMethod]
        public void TestIsNotValidXmlDocument()
        {
            var json = "{ \"result\": \"true\" }";
            int count;
            Assert.IsFalse(XmlHelpers.IsValidXmlDocument(json, out count));
        }

        /// <summary>The test valid xml.</summary>
        [TestMethod]
        public void TestValidXmlDocument()
        {
            int count;
            var xml = "<result>true</result>";
            Assert.IsTrue(XmlHelpers.IsValidXmlDocument(xml, out count));
        }

        /// <summary>The test ximple extensions.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void TestXimpleExtensions()
        {
            var ximple = new Ximple(Constants.Version2);

            var configManager =
                new ConfigManager<Dictionary>
                    {
                        FileName = PathManager.Instance.GetPath(
                            FileType.Config,
                            "dictionary.xml"),
                        EnableCaching = false,
                        XmlSchema = Dictionary.Schema
                    };

            var dictionary = configManager.Config;
            var table = dictionary.Tables.First(m => m.Name == "NetworkFileAccessSettings");
            var settings = new NetworkFtpSettings("Fred", "123456", new Uri(@"ftp://192.1.2.3/Shared"), true);
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Uri", settings.SharedUriString));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "UserName", settings.UserName));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "Password", settings.Password));
            Assert.AreEqual(3, ximple.Cells.Count(m => m.TableNumber == table.Index));
            var xml = ximple.ToXmlString();
            Debug.WriteLine(xml);
            Assert.AreEqual(3, ximple.Cells.Count);

            // test expected columns have the expected values
            Assert.IsNotNull(
                ximple.Cells.SingleOrDefault(m => m.ColumnNumber == 0 && m.Value == @"ftp://192.1.2.3/Shared"),
                "Uri wrong");
            Assert.IsNotNull(
                ximple.Cells.SingleOrDefault(m => m.ColumnNumber == 1 && m.Value == "Fred"),
                "User Name wrong");
            Assert.IsNotNull(
                ximple.Cells.SingleOrDefault(m => m.ColumnNumber == 2 && m.Value == "123456"),
                "Password wrong");
        }

        /// <summary>The test ximple extensions bogus column name.</summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestXimpleExtensionsBogusColumnName()
        {
            var ximple = new Ximple(Constants.Version2);

            var configManager =
                new ConfigManager<Dictionary>
                    {
                        FileName = PathManager.Instance.GetPath(
                            FileType.Config,
                            "dictionary.xml"),
                        EnableCaching = false,
                        XmlSchema = Dictionary.Schema
                    };

            var dictionary = configManager.Config;
            var table = dictionary.Tables.First(m => m.Index == 103);
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "BogusColumnName", "TEST"));
        }

        /// <summary>The test ximple extensions find column.</summary>
        [TestMethod]
        public void TestXimpleExtensionsFindColumn()
        {
            var configManager =
                new ConfigManager<Dictionary>
                    {
                        FileName = PathManager.Instance.GetPath(
                            FileType.Config,
                            "dictionary.xml"),
                        EnableCaching = false,
                        XmlSchema = Dictionary.Schema
                    };

            var dictionary = configManager.Config;
            var table = dictionary.FindXimpleTable("NetworkChangedMessage");
            Assert.IsNotNull(table);
            var column = table.FindColumn("Connected");
            Assert.IsNotNull(column);
            Assert.AreEqual("Connected", column.Name);

            int tableIndex;
            column = dictionary.FindTableAndColumn("NetworkChangedMessage", "Connected", out tableIndex);
            Assert.IsNotNull(column);
            Assert.AreEqual("Connected", column.Name);
            Assert.AreEqual(XimpleConfig.NetworkChangedMessageTableIndexDefault, tableIndex);
        }

        /// <summary>The test ximple extensions find table by name.</summary>
        [TestMethod]
        public void TestXimpleExtensionsFindTableByName()
        {
            var configManager =
                new ConfigManager<Dictionary>
                    {
                        FileName = PathManager.Instance.GetPath(
                            FileType.Config,
                            "dictionary.xml"),
                        EnableCaching = false,
                        XmlSchema = Dictionary.Schema
                    };

            // use PassengerMessages as a test any table should work too
            var dictionary = configManager.Config;
            var table = dictionary.FindXimpleTable("20");
            Assert.IsNotNull(table);
            Assert.AreEqual(20, table.Index);
            table = dictionary.FindXimpleTable(20);
            Assert.IsNotNull(table);
            Assert.AreEqual(20, table.Index);
            table = dictionary.FindXimpleTable("PassengerMessages");
            Assert.IsNotNull(table);
            Assert.AreEqual(20, table.Index);
        }

        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        public void WriteXimpleConfigFile()
        {
            var configFileName = @"C:\Temp\XimpleConfigTestWrite.xml";
            var config = new XimpleConfig();
            XimpleConfig.Write(config, configFileName);
            var outputPath = AppDomain.CurrentDomain.BaseDirectory;
            Assert.IsTrue(File.Exists(configFileName));
            Debug.WriteLine("File Out here => " + configFileName);
            var configManager = new ConfigManager<XimpleConfig>(configFileName) { XmlSchema = XimpleConfig.Schema };
            var config2 = configManager.Config;
            Assert.AreEqual(1600, config2.Port);
            Assert.IsNotNull(config2.NetworkFtpSettings);
            Assert.IsNotNull(config2.AudioZonePresentationValues);
            Assert.AreNotEqual(string.Empty, config2.AudioZonePresentationValues.Interior);
            Assert.AreNotEqual(string.Empty, config2.AudioZonePresentationValues.Exterior);
            Assert.AreNotEqual(string.Empty, config2.AudioZonePresentationValues.Both);
        }

        [TestMethod]
        public void XimpleCannedMessageZoneToEnumTest()
        {
            var audioZoneType = AudioZoneTypes.None;
            var cannedMessageZone = "1";
            if (Enum.TryParse(cannedMessageZone, out audioZoneType) == false)
            {
                Logger.Info("Unknown Audio Zone in Ximple, Default to Interior");
                Assert.Fail("Failed to determine AudioZone enum Interior");
            }

            Assert.AreEqual(AudioZoneTypes.Interior, audioZoneType);

            cannedMessageZone = "2";
            if (Enum.TryParse(cannedMessageZone, out audioZoneType) == false)
            {
                Logger.Info("Unknown Audio Zone in Ximple, Default to Interior");
                Assert.Fail("Failed to determine AudioZone enum Exterior");
            }

            Assert.AreEqual(AudioZoneTypes.Exterior, audioZoneType);

            cannedMessageZone = "3";
            if (Enum.TryParse(cannedMessageZone, out audioZoneType) == false)
            {
                Logger.Info("Unknown Audio Zone in Ximple, Default to Interior");
                Assert.Fail("Failed to determine AudioZone enum Both");
            }

            Assert.AreEqual(AudioZoneTypes.Both, audioZoneType);

            cannedMessageZone = "0";
            if (Enum.TryParse(cannedMessageZone, out audioZoneType) == false)
            {
                Logger.Info("Unknown Audio Zone in Ximple, Default to Interior");
                Assert.Fail("Failed to determine AudioZone enum ,");
            }

            Assert.AreEqual(AudioZoneTypes.None, audioZoneType);

            Assert.AreEqual(1, (int)AudioZoneTypes.Interior);
            Assert.AreEqual(2, (int)AudioZoneTypes.Exterior);
            Assert.AreEqual(3, (int)AudioZoneTypes.Both);
        }

        /// <summary>The invalid ximple version.</summary>
        [TestMethod]
        public void XimpleFromXml()
        {
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            int count;
            var result = XmlHelpers.IsValidXmlDocument(XmlString, out count);
            Assert.IsTrue(result);
            var ximple = XmlHelpers.ToXimple(XmlString);
        }

        /// <summary>The invalid ximple version expected invalid operation exception.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void XimpleFromXmlInvalidOperationException()
        {
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n</Ximple>";
            int count;
            var result = XmlHelpers.IsValidXmlDocument(XmlString, out count);
            Assert.IsTrue(result);
            var ximple = XmlHelpers.ToXimple(XmlString);
        }

        /// <summary>The invalid ximple version expected invalid operation exception.</summary>
        [TestMethod]
        [DeploymentItem(@"Config\dictionary.xml")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void XimpleFromXmlInvalidOperationExceptionEndTag()
        {
            const string XmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">\r\n<Cells>\r\n  <Cell Language=\"2\" Table=\"0\" Column=\"4\" Row=\"0\">1</Cell>\r\n</Cells>\r\n";
            int count;
            var result = XmlHelpers.IsValidXmlDocument(XmlString, out count);
            Assert.IsFalse(result);
            var ximple = XmlHelpers.ToXimple(XmlString);
        }

        /// <summary>The ximple infotainment canned msg play.</summary>
        [TestMethod]
        public void XimpleInfoTainmentCannedMsgPlay()
        {
            var configManager =
                new ConfigManager<Dictionary>
                    {
                        FileName = PathManager.Instance.GetPath(
                            FileType.Config,
                            "dictionary.xml"),
                        EnableCaching = false,
                        XmlSchema = Dictionary.Schema
                    };

            // use PassengerMessages as a test any table should work too
            var dictionary = configManager.Config;
            var table = dictionary.FindXimpleTable(XimpleConfig.InfoTainmentCannedMsgPlayTableIndexDefault);
            Assert.IsNotNull(table, "Table not found by Index");
            var ximple = new Ximple(Constants.Version2);
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMessageID", 1, 0));
            ximple.Cells.Add(dictionary.CreateXimpleCell(table, "CannedMessageZone", 1, 1));
            ximple.Cells.Add(
                dictionary.CreateXimpleCell(table, "CannedMessageEncoding", CannedMessageEncodingType.Mp3, 2));
            var xml = ximple.ToXmlString();
            int count;
            var result = XmlHelpers.IsValidXmlDocument(xml, out count);
            Assert.IsTrue(result);

            this.cannedMediMessageEvent.Reset();
            MessageDispatcher.Instance.Subscribe<CannedPlaybackMessage>(this.HandlerCannedMessages);

            this.SendXmlAsXimple(xml, new XimpleProtocolTestConfig(1, 0));

            MessageDispatcher.Instance.Unsubscribe<CannedPlaybackMessage>(this.HandlerCannedMessages);

            Assert.IsTrue(this.cannedMediMessageEvent.WaitOne(3000), "Medi message not broadcast");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void XimpleMesssageWithNoLanguage()
        {
            const string XmlStringNoLanguage = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                               + "<Ximple xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Version=\"2.0.0\">"
                                               + "<Cells>"
                                               + "<Cell Language=\"\" Table=\"11\" Column=\"0\" Row=\"0\">1 LUMINATOR INBOUND</Cell>"
                                               + "<Cell> Language=\"\" Table=\"10\" Column=\"0\" Row=\"0\">11</Cell>"
                                               + "<Cell Language=\"2\" Table=\"12\" Column=\"0\" Row=\"0\">PRECISION &amp; PROGRESS</Cell>"
                                               + "<Cell Language=\"\" Table=\"12\" Column=\"0\" Row=\"1\">PROGRESS &amp; 10TH</Cell>"
                                               + "<Cell Language=\"\" Table=\"12\" Column=\"0\" Row=\"2\">TECHNOLOGY &amp; RESOURCE</Cell>"
                                               + "<Cell Language=\"\" Table=\"12\" Column=\"0\" Row=\"3\">CAPITAL &amp; N AVE</Cell>"
                                               + "<Cell Language=\"\" Table=\"12\" Column=\"0\" Row=\"4\">STEWART &amp; CAPITAL</Cell>"
                                               + "<Cell Language=\"\" Table=\"12\" Column=\"0\" Row=\"5\">SUMMIT &amp; STEWART</Cell>"
                                               + "</Cells>" + "</Ximple>";
            int count;
            var result = XmlHelpers.IsValidXmlDocument(XmlStringNoLanguage, out count);
            Assert.AreEqual(1, count);
            Assert.AreEqual(true, result);

            var xml = XmlStringNoLanguage;
            if (xml.Contains("Language=\"\""))
            {
                Debug.WriteLine(
                    "!Ximple had missing missing Language='' Index={0}",
                    xml.IndexOf(".Language=\"\"", StringComparison.Ordinal));
                xml = xml.Replace("Language=\"\"", "Language=\"0\"");

                var found = xml.Contains("Language=\"\"");
                Assert.IsFalse(found);
            }
            else
            {
                Assert.Fail("Expected to find string");
            }

            var ximple = XmlHelpers.ToXimple(XmlStringNoLanguage);
        }

        private static XimpleSocketService CreateXimpleSocketService(int port = XimpleSocketService.DefaultPort)
        {
            var configMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            try
            {
                Debug.WriteLine("CreateXimpleSocketService() Enter Port={0}", port);

                // we use the dictionary to generate Ximple responses, test that we have it available.
                var dictionaryTest = configMgr.Config; // throws exception on file not found
                Assert.IsNotNull(dictionaryTest);
                Assert.IsTrue(dictionaryTest.Tables.Count > 0);
                var service = new XimpleSocketService(new XimpleConfig { Port = port }, configMgr);
                return service;
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                Assert.Fail("Dictionary.xml file {0} not found for unit testing needs", e.FileName);
            }

            return null;
        }

        private static string DebugOutputServerResponce(byte[] responseBuffer, int bytesReceived)
        {
            // TODO if we have one
            var s = Encoding.UTF8.GetString(responseBuffer, 0, bytesReceived);
            Debug.WriteLine("Unit Test TCP Bytes received = " + responseBuffer.Length + "\r\nString=" + s);
            return s;
        }

        private static int GetFreeTcpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }

        private static void InitMedi()
        {
            // use the same mechanics as the host apps so we can run them standalone and debug messages to/from them
            var configFileName = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            fileConfigurator = new FileConfigurator(configFileName, Environment.MachineName, Environment.MachineName);
            MessageDispatcher.Instance.Configure(fileConfigurator);
        }

        private static bool IsValidXmlDocument(string xmlString, int expectedCount)
        {
            int count;
            var valid = XmlHelpers.IsValidXmlDocument(xmlString, out count);
            Assert.AreEqual(expectedCount, count);
            return valid;
        }

        private static string SerializeAsXml<T>()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.None })
                {
                    var obj = Activator.CreateInstance<T>();
                    serializer.Serialize(writer, obj);
                    var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                    Debug.WriteLine(s);
                    return s;
                }
            }
        }

        private void HandlerCannedMessages(object sender, MessageEventArgs<CannedPlaybackMessage> messageEventArgs)
        {
            this.cannedMediMessageEvent.Set();
            Debug.WriteLine("CannedMsg Medi message board-casted");
        }

        private void SendAudioStatusRequestMessage(string appName)
        {
            var mediaAddress = MessageDispatcher.Instance.LocalAddress;
            mediaAddress.Application = appName;
            MessageDispatcher.Instance.Send(mediaAddress, new AudioStatusRequest());
        }

        private void SubscriptionOnXimpleCreated(object s, XimpleEventArgs e)
        {
            Debug.WriteLine("XimpeCreated Medi Event");
        }
    }
}