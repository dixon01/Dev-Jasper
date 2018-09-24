// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Serialization;

    using AutoMapper;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Protocols.Ximple.Utils;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Protran.Core.Protocols;

    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Models;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1305:FieldNamesMustNotUseHungarianNotation",
        Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK here.")]
    [DeploymentItem(@"Config\medi.config")]
    [DeploymentItem(@"Config\dictionary.xml")]
    [DeploymentItem("AdHocMessagingProtocolConfig.xml")]
    [TestClass]
    public class AdHocMessagingProtocolUnitTest
    {
        public const string DefaultAdHocMessageApiUrl = "http://swdevicntrweb.luminatorusa.com/";

        public const string DefaultDestinationsApiUrl = "http://swdevicntrapp.luminatorusa.com/";

        private static AdHocMessageService tempAdHocMessageService;

        private const bool VeroseXimpleDebug = false;

        static AdHocMessagingProtocolUnitTest()
        {
            tempAdHocMessageService = new AdHocMessageService();
        }

        public static ProtocolHost ProtocolHost { get; private set; }

        private static Dictionary Dictionary { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));
            var cfg = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            Dictionary = cfg.Config;
            ProtocolHost = CreateProtocolHost(Dictionary);
        }

        [TestMethod]
        public void AdHocService_GetUnitMessages()
        {
            var request = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            var mock = new Mock<IAdhocManager>();
            mock.Setup(m => m.GetAllMessagesForUnitAsync("TFT-1-2-3", "R1", It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message>()));

            Assert.IsNotNull(mock.Object);
            var service = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);
            var unitAdHocMessages = service.GetUnitAdHocMessages(request);
            Assert.IsNotNull(unitAdHocMessages);
            Assert.IsNotNull(unitAdHocMessages.Messages);
            Assert.IsTrue(unitAdHocMessages.Messages.Count == service.Config.MaxAdHocMessages);
            Assert.AreEqual(unitAdHocMessages.Status, HttpStatusCode.OK);
        }

        [TestMethod]
        public void AutoMapModels()
        {
            var now = DateTime.Now;
            var message = new Message { Text = "text", Title = "title", StartDate = now, EndDate = now.AddDays(1) };
            var adHocMessage = Mapper.Map<XimpleAdHocMessage>(message);
            Assert.IsNotNull(adHocMessage);
            Assert.AreEqual(adHocMessage.Text, message.Text);
            Assert.AreEqual(adHocMessage.Title, message.Title);
            Assert.AreEqual(adHocMessage.StartDate, message.StartDate);
            Assert.AreEqual(adHocMessage.EndDate, message.EndDate);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.UnIntiMidi();
            ProtocolHost.Dispose();
        }

        [TestMethod]
        public void ConstructAdHocGetMessagesRequest()
        {
            var m = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            Assert.AreEqual("TFT-1-2-3", m.FirstUnit);
            Assert.AreEqual("TFT-1-2-3", m.Units.First().Name);
            Assert.AreEqual("BUS123", m.VehicleId);
            Assert.AreEqual("R1", m.Route);
            Assert.AreEqual(1, m.Units.Count);
            Assert.IsNotNull(m.UnitLocalTimeStamp);
            Assert.AreEqual(TimeSpan.Parse("0:0:0"), m.UnitLocalTimeStamp.Value.TimeOfDay);
        }

        [TestMethod]
        public void ConstructAdHocGetMessagesRequest_DefaultUnitName()
        {
            var m = new AdHocGetMessagesRequest("R1", string.Empty, "BUS123");
            var defaultUnitName = MessageDispatcher.Instance.LocalAddress.Unit;
            Assert.AreEqual(defaultUnitName, m.FirstUnit);
            Assert.AreEqual("BUS123", m.VehicleId);
            Assert.AreEqual("R1", m.Route);
            Assert.AreEqual(1, m.Units.Count);
            Assert.IsNotNull(m.UnitLocalTimeStamp);
            Assert.AreEqual(TimeSpan.Parse("0:0:0"), m.UnitLocalTimeStamp.Value.TimeOfDay);
        }

        [TestMethod]
        public void ConstructAdHocGetMessagesRequest2()
        {
            var m = new AdHocGetMessagesRequest(
                "BUS123",
                "R1",
                new List<IAdHocUnit> { new AdHocUnit("TFT-1-2-3"), new AdHocUnit("TFT-7-7-7") });
            Assert.AreEqual("TFT-1-2-3", m.FirstUnit);
            Assert.AreEqual("TFT-1-2-3", m.Units.First().Name);
            Assert.AreEqual("TFT-7-7-7", m.Units[1].Name);
            Assert.AreEqual("BUS123", m.VehicleId);
            Assert.AreEqual("R1", m.Route);
            Assert.AreEqual(2, m.Units.Count);
        }

        [TestMethod]
        public void ConstructInValidAdHocRegisterRequestMissingUnits()
        {
            var m = new AdHocRegisterRequest("BUS123", new List<IAdHocUnit>());
            Assert.IsFalse(m.IsValid);
        }

        [TestMethod]
        public void ConstructInValidAdHocRegisterRequestMissingVehicleId()
        {
            var m = new AdHocRegisterRequest(string.Empty, new List<IAdHocUnit> { new AdHocUnit("TFT-1-2-3", "Test") });
            Assert.IsFalse(m.IsValid);
        }

        public void ConstructInValidAdHocRegisterRequestMissingUnitAndVehicleId()
        {
            var m = new AdHocRegisterRequest(string.Empty, string.Empty);
            Assert.AreEqual(MessageDispatcher.Instance.LocalAddress.Unit, m.FirstUnit);
            Assert.IsFalse(m.IsValid);  // VehicleId is missing so invalid
        }

        [TestMethod]
        public void ConstructInValidAdHocRegisterRequestWithInvalidUnitName()
        {
            var m = new AdHocRegisterRequest("BUS123", string.Empty);
            Assert.AreEqual(MessageDispatcher.Instance.LocalAddress.Unit, m.FirstUnit);
            Assert.IsTrue(m.IsValid);   // ComputerName will be used
        }

        [TestMethod]        
        public void AdHocServiceRegistrationWithEmptyStringForUnitName_ExpectSuccess()
        {
            var config = CreateAdHocMessageServiceConfig();
            Assert.IsNotNull(config);
            var service = new AdHocMessageService(config, null);
            Assert.IsNotNull(service.Config);
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;
            var request = new AdHocRegisterRequest("BUS123", string.Empty);
            Assert.IsTrue(request.IsValid); // we know it's bad, default to medi local address name
            Assert.AreEqual(unitName, request.FirstUnit);

            var mock = new Mock<IAdhocManager>();
            mock.Setup(m => m.RegisterVehicleAndUnitAsync("BUS123", unitName, It.IsAny<string>(), It.IsAny<string>(), true))
                .Returns(Task.FromResult(HttpStatusCode.OK));
            
            var registrationResponse = service.RegisterVehicleAndUnit(request);
            Assert.IsNotNull(registrationResponse);
            Assert.IsTrue(registrationResponse.IsRegistered);

            var logFile = PathManager.Instance.CreatePath(FileType.Data, "AdHocRegistration.txt");
            Assert.IsTrue(File.Exists(logFile));
            Debug.WriteLine(File.ReadAllText(logFile));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructTest()
        {
            Assert.IsNotNull(Dictionary, "dictionary.xml missing");
            new AdHocMessagingProtocolImpl(null, null, Dictionary);
        }

        [TestMethod]
        public void ConstructValidAdHocRegisterRequest()
        {
            var m = new AdHocRegisterRequest("BUS123", new List<IAdHocUnit> { new AdHocUnit("TFT-1-2-3", "Test") });
            Assert.IsTrue(m.IsValid);
        }

        [TestMethod]
        public void CreateAdHocMessageServiceConfigTest()
        {
            var c = CreateAdHocMessageServiceConfig();
            Assert.IsNotNull(c);
            Assert.AreEqual(DefaultAdHocMessageApiUrl, c.AdHocApiUri.OriginalString);
            Assert.AreEqual(DefaultDestinationsApiUrl, c.DestinationsApiUri.OriginalString);
            Assert.IsTrue(c.ApiTimeout > 0);
            Assert.IsTrue(c.MaxAdHocMessages > 0);
            Assert.IsTrue(c.MaxRegistrationAttempts > 0);
        }

        [TestMethod]
        public void CreateMessageProtocolImpl_Start_ExpectStartedEvent()
        {
            var signaledEvents = new CountdownEvent(1);
            AdHocMessagingProtocolImpl impl = null;
            Task.Run(
                () =>
                    {
                        var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
                        config.AdHocMessageTimerSettings = new TimerSettings(1);
                        impl = CreateMessagingProtocolImpl(config);
                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("Started");
                                signaledEvents.Signal();
                            };
                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });
            Assert.IsTrue(signaledEvents.Wait(2000));
            impl?.Stop();
        }

        [TestMethod]
        public void CreateMessageProtocolImpl_StartDefaultState()
        {
            var signaledEvents = new CountdownEvent(1);
            var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
            config.AdHocMessageTimerSettings = new TimerSettings(1);

            var impl = new AdHocMessagingProtocolImpl();
            impl.Configure(Dictionary, config, null);
            Assert.IsNotNull(impl.Dictionary, "Dictionary not set");
            Assert.IsNotNull(impl.Config, "Config not set");
            Assert.IsNotNull(impl.AdHocMessageService);

            Task.Run(
                () =>
                    {
                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("Started");
                                signaledEvents.Signal();
                            };

                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });
            Assert.IsTrue(signaledEvents.Wait(3000), "Started event missed");
            Assert.AreEqual(ServiceRunState.RegisterUnit, impl.ServiceRunState);
            impl.Stop();
        }

        [TestMethod]
        public void CreateMessageProtocolImplMoq()
        {
            var signaledEvents = new CountdownEvent(1);

            var mockAdHocService = new Mock<IAdHocMessageService>();
            var request = new AdHocGetMessagesRequest(
                "BUS123",
                "R1",
                new List<IAdHocUnit> { new AdHocUnit("TFT-1-2-3"), new AdHocUnit("TFT-2-3-4") });
            mockAdHocService.Setup(m => m.GetUnitAdHocMessages(request)).Returns(this.CreateMocAdhocMessage());

            var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
            config.AdHocMessageTimerSettings = new TimerSettings(1);
            var impl = CreateMessagingProtocolImpl(config);
            impl.ServiceRunState = ServiceRunState.RequestAdHocMessages;

            Task.Run(
                () =>
                    {
                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("Started");
                                signaledEvents.Signal();
                            };
                        impl.OnXimpleAdHocMessageCreated += (sender, list) =>
                            {
                                Debug.WriteLine("OnXimpleAdHocMessageCreated Event");
                                signaledEvents.Signal();
                            };
                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });
            Assert.IsTrue(signaledEvents.Wait(2000));
            impl?.Stop();
        }

        [ExpectedException(typeof(UriFormatException))]
        [TestMethod]
        public void CreateMessageProtocolImplWithInvalidHostUriExpectException()
        {
            var config = new AdHocMessagingProtocolConfig
            {
                AdHocApiUri = null // Known bad empty uri
            };
            var impl = new AdHocMessagingProtocolImpl();
            impl.Configure(Dictionary, config, null);
        }

        [TestMethod]
        public void CreateMessagingProtocolImplVerifyConfigure()
        {
            var impl = new AdHocMessagingProtocolImpl();
            var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
            var adHocMessageService = new AdHocMessageServiceConfig(config);
            var service = new AdHocMessageService(adHocMessageService, null);
            impl.Configure(Dictionary, config, service);
            Assert.IsNotNull(impl.Dictionary);
            Assert.IsNotNull(impl.Config);
            Assert.IsNotNull(impl.AdHocMessageService);
        }

        [TestMethod]
        public void CreateMessagingProtocolImplVerifyConfigure2()
        {
            var impl = new AdHocMessagingProtocolImpl();
            impl.Configure(Dictionary);
            Assert.IsNotNull(impl.Dictionary);
            Assert.IsNotNull(impl.Config);
            Assert.IsNotNull(impl.AdHocMessageService);
        }

        [TestMethod]
        public void GetMediAddresses()
        {
            var myAddress = MessageDispatcher.Instance.LocalAddress;
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;
            Assert.IsFalse(string.IsNullOrEmpty(unitName));
            var session = MessageDispatcher.Instance.RoutingTable.GetSessionId(myAddress);
            var entries = MessageDispatcher.Instance.RoutingTable.GetEntries(id => !id.Equals(session));
            var units = entries.Select(m => m.Address.Unit).Distinct();
            Assert.IsTrue(units != null);
        }

        [TestMethod]
        public void GetUnitAdHocMessages_ShouldCatchException_EmptyMessages()
        {
            var request = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            var mock = new Mock<IAdhocManager>();
            mock.Setup(m => m.GetAllMessagesForUnitAsync("TFT-1-2-3", "R1", It.IsAny<DateTime>()))
                .Throws(new HttpException((int)HttpStatusCode.NotFound, "Not Found Test"));
            var service = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);
            var unitAdHocMessages = service.GetUnitAdHocMessages(request);
            Assert.AreEqual(unitAdHocMessages.Status, HttpStatusCode.NotFound);
            Assert.IsTrue(unitAdHocMessages.Messages.Count == 0);
        }

        [TestMethod]
        public void GetUnitAdHocMessages_ShouldReturn_DefaultXimpleMessage_MaxMessagesReturned()
        {
            var request = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            var mock = new Mock<IAdhocManager>();
            var testMessage = this.CreateAdhocTestMessage();
            mock.Setup(m => m.GetAllMessagesForUnitAsync("TFT-1-2-3", "R1", It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message> { testMessage }));

            var service = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);
            var unitAdHocMessages = service.GetUnitAdHocMessages(request);
            Assert.AreEqual(unitAdHocMessages.Status, HttpStatusCode.OK);
            Assert.IsTrue(unitAdHocMessages.Messages.Count == service.Config.MaxAdHocMessages);
            var message = unitAdHocMessages.Messages.First();
            Assert.IsInstanceOfType(message, typeof(XimpleAdHocMessage));
            Assert.AreEqual("TestText", message.Text);
            Assert.AreEqual("Title", message.Title);
        }

        [TestMethod]
        public void GetUnitAdHocMessages_ShouldReturn_DefaultXimpleMessage_MaxMessagesReturned_AsEmptyText()
        {
            var request = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            var mock = new Mock<IAdhocManager>();
            mock.Setup(m => m.GetAllMessagesForUnitAsync("TFT-1-2-3", "R1", It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message>()));

            var service = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);
            var unitAdHocMessages = service.GetUnitAdHocMessages(request);
            Assert.AreEqual(unitAdHocMessages.Status, HttpStatusCode.OK);
            Assert.IsTrue(unitAdHocMessages.Messages.Count == service.Config.MaxAdHocMessages);
            foreach (var message in unitAdHocMessages.Messages)
            {
                Assert.AreEqual(string.Empty, message.Text);
                Assert.AreEqual(string.Empty, message.Route);
                Assert.AreEqual(string.Empty, message.Title);
            }
        }

        [TestMethod]
        public void GetUnitAdHocMessages_ShouldReturn_MessagesMappedToXimpleAdHocMessages()
        {
            var request = new AdHocGetMessagesRequest("R1", "TFT-1-2-3", "BUS123");
            var mock = new Mock<IAdhocManager>();
            var testMessage = this.CreateAdhocTestMessage();
            mock.Setup(m => m.GetAllMessagesForUnitAsync("TFT-1-2-3", "R1", It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message> { testMessage }));

            var service = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);

            var unitAdHocMessages = service.GetUnitAdHocMessages(request);
            Assert.IsTrue(unitAdHocMessages.Messages.Count == service.Config.MaxAdHocMessages);
            var message = unitAdHocMessages.Messages[0];
            Assert.AreEqual(testMessage.Text, message.Text);
            Assert.AreEqual(testMessage.Title, message.Title);
        }

        public AdHocMessagingProtocolConfig ReadConfig(string configFile)
        {
            return AdHocMessagingProtocolConfig.ReadConfig(configFile);
        }

        [DeploymentItem(@"Config\AdHocMessagingProtocolConfigV1.xml")]
        [TestMethod]
        public void ReadConfig_V1()
        {
            Assert.IsTrue(File.Exists("AdHocMessagingProtocolConfigV1.xml"));
            var config = this.ReadConfig("AdHocMessagingProtocolConfigV1.xml");
            Assert.IsNotNull(config);
            Assert.IsTrue(config.MaxAdHocMessages > 0);
            Assert.IsNotNull(config.AdHocMessageTimerSettings);
            Assert.IsNotNull(config.RegisterUnitTimerSettings);
            Assert.IsNotNull(config.RequestUnitInfoTimerSettings);
        }

        [DeploymentItem(@"Config\AdHocMessagingProtocolConfigV2.xml")]
        [TestMethod]
        public void ReadConfig_V2()
        {
            Assert.IsTrue(File.Exists("AdHocMessagingProtocolConfigV2.xml"));
            var config = this.ReadConfig("AdHocMessagingProtocolConfigV2.xml");
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.AdHocMessageTimerSettings);
            Assert.IsNotNull(config.RegisterUnitTimerSettings);
            Assert.IsNotNull(config.RequestUnitInfoTimerSettings);
            Assert.IsTrue(config.MaxAdHocMessages > 0);
            Assert.AreEqual(@"http://swdevicntrweb.luminatorusa.com/", config.AdHocApiUri.HostUri.ToString());
            Assert.AreEqual(@"http://swdevicntrapp.luminatorusa.com/", config.DestinationsApiUri.HostUri.ToString());
        }

        [TestMethod]
        public void ReadWriteAdHocMessagingProtocolConfig()
        {
            const string ConfigFile = @"C:\Temp\AdHocMessagingProtocolConfig.xml";
            var config = new AdHocMessagingProtocolConfig
            {
                AdHocApiUri = new UriSettings(DefaultAdHocMessageApiUrl),
                DestinationsApiUri =
                                     new UriSettings(DefaultDestinationsApiUrl),
                AdHocMessageTimerSettings =
                                     new TimerSettings(TimeSpan.FromMinutes(5)),
                RegisterUnitTimerSettings =
                                     new TimerSettings(TimeSpan.FromMinutes(1)),
                RequestUnitInfoTimerSettings =
                                     new TimerSettings(TimeSpan.FromMinutes(1)),
                EnableUnitRegistration = false
            };

            File.Delete(ConfigFile);
            AdHocMessagingProtocolConfig.WriteConfig(config, ConfigFile);
            Assert.IsTrue(File.Exists(ConfigFile));
            var config2 = this.ReadConfig(ConfigFile);
            Assert.IsNotNull(config2);
            Assert.AreEqual(config.AdHocApiUri.UriXml, config2.AdHocApiUri.UriXml);
            Assert.AreEqual(config.DestinationsApiUri.UriXml, config2.DestinationsApiUri.UriXml);
            Assert.AreEqual(config.ApiTimeout, config2.ApiTimeout);
            Assert.IsTrue(config.ApiTimeout > 0);
            Assert.IsTrue(config.MaxAdHocMessages > 0);
            Assert.AreEqual("PT5M", config2.AdHocMessageTimerSettings.IntervalXml);
            Assert.AreEqual("PT1M", config2.RegisterUnitTimerSettings.IntervalXml);
            Assert.AreEqual("PT1M", config2.RequestUnitInfoTimerSettings.IntervalXml);
            Assert.AreEqual(false, config.EnableUnitRegistration);
        }

        [TestMethod]
        public void StartProtocol_ConfigureNoRegistration_VehicleIdChange_ExpectRegistration()
        {
            var mock = new Mock<IAdhocManager>();
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;

            // Simulate Messages
            var testMessage = this.CreateAdhocTestMessage();
            mock.Setup(m => m.GetAllMessagesForUnitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message> { testMessage }));
            var initialVehicleInfo = new VehicleInfo("BUS123", unitName);
            mock.Setup(m => m.RegisterVehicleAndUnitAsync(It.IsAny<string>(), unitName, It.IsAny<string>(), It.IsAny<string>(), true))
                .Returns(Task.FromResult(HttpStatusCode.OK));

            // indicate we do not care to register, override default settings from our file
            var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
            config.EnableUnitRegistration = true;
            config.RegisterUnitTimerSettings = new TimerSettings(1);    // faster timings for unit testing
            config.AdHocMessageTimerSettings = new TimerSettings(1);

            var adHocMessageService = new AdHocMessageService(CreateAdHocMessageServiceConfig(config), mock.Object);

            var impl = CreateMessagingProtocolImpl(config, adHocMessageService);

            // Set the initial VehicleId then we will change it to verify the re-registration state change
            impl.UpdateVehicleInfo(initialVehicleInfo);

            var signaledEvents = new CountdownEvent(2);
            var receivedAdHocEvents = new CountdownEvent(2);
            var vehicleRegistered = new CountdownEvent(2);
            var adhocPostRegistrationEvent = new ManualResetEvent(false);
            bool newVehicleIdRegstered = false;

            Task.Run(
                () =>
                    {
                        // setup to be in retrieving messages as if we have already registered the Unit & Vehicle
                        impl.ServiceRunState = ServiceRunState.RequestAdHocMessages;
                        impl.Started += (sender, args) =>
                            {
                                // First Event
                                signaledEvents.Signal();
                                Debug.WriteLine("*Started event");
                            };
                        impl.OnRegisterCompleted += (sender, vehicleInfo) =>
                            {
                                // Second event
                                vehicleRegistered.Signal();
                                Debug.WriteLine("*VehicleId Registered = " + vehicleInfo.VehicleId);
                            };
                        impl.OnAdHocMessagesReceived += (sender, args) =>
                            {
                                // After regisration expect this
                                // we do not expect this event if exceptions occur on the REST calls     
                                if (!receivedAdHocEvents.IsSet)
                                {
                                    receivedAdHocEvents.Signal();
                                }
                                if (newVehicleIdRegstered)
                                {
                                    adhocPostRegistrationEvent.Set();
                                }
                                Debug.WriteLine("*OnAdHocMessagesReceived event");
                            };
                        impl.VehicleInfoChanged += (sender, vehicleInfo) =>
                            {
                                signaledEvents.Signal();
                                Debug.WriteLine("VehicleId Changed = " + vehicleInfo.VehicleId);

                                if (vehicleInfo.VehicleId == "BUS777")
                                {
                                    newVehicleIdRegstered = true;
                                }
                            };

                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });

            // simulate Ximple message of Vehicle change 
            var vehicleUnitInfo = new VehicleUnitInfo(new VehicleInfo("BUS777", unitName), new VehiclePositionMessage { Route = "R1" });
            Assert.AreEqual("BUS777", vehicleUnitInfo.VehicleInfo.VehicleId);
            Assert.IsTrue(vehicleUnitInfo.IsValidVehicleId);
            Assert.IsTrue(vehicleUnitInfo.IsValidVehicleInfo);

            var adHocReceived = receivedAdHocEvents.Wait(15000);
            Assert.IsTrue(adHocReceived, "AdHoc Message not received in the test yet");

            // cause the vehicle id to be changed and we set the run state back to register
            Debug.WriteLine("MessageDispatcher.Instance.Broadcast(vehicleUnitInfo)");
            MessageDispatcher.Instance.Broadcast(vehicleUnitInfo);

            var registered = vehicleRegistered.Wait(10000);
            Assert.IsTrue(registered, "not registered as expected Count = " + vehicleRegistered.CurrentCount);

            Assert.IsTrue(signaledEvents.Wait(10000));
            Assert.IsTrue(newVehicleIdRegstered);

            var signaled = adhocPostRegistrationEvent.WaitOne(2000);
            Assert.IsTrue(signaled, "AdHoc Messagess Post New vehicle registraion not signaled");
            impl?.Stop();
        }


        [TestMethod]
        public void StartProtocol_ConfigureNoRegistration_CatchException_ExpectNoEventsOrAdHocMessages()
        {
            var signaledEvents = new CountdownEvent(1);
            AdHocMessagingProtocolImpl impl = null;
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;
            var messageReceived = false;
            var mock = new Mock<IAdhocManager>();

            // Simulate Exception
            mock.Setup(m => m.GetAllMessagesForUnitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Throws(new HttpException((int)HttpStatusCode.NotFound, "Nobody Home Exception Test"));

            var adHocMessageService = new AdHocMessageService(CreateAdHocMessageServiceConfig(), mock.Object);

            Task.Run(
                () =>
                    {
                        // indicate we do not care to register, override default settings from our file
                        var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
                        config.EnableUnitRegistration = false;
                        config.RegisterUnitTimerSettings = new TimerSettings(0);
                        config.AdHocMessageTimerSettings = new TimerSettings(1);
                        impl = CreateMessagingProtocolImpl(config, adHocMessageService);
                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("*Started event");
                                signaledEvents.Signal();
                            };
                        impl.OnAdHocMessagesReceived += (sender, args) =>
                            {
                                // we do not expect this event for exceptions
                                Debug.WriteLine("*OnAdHocMessagesReceived event");
                                messageReceived = true;
                            };
                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });
            Assert.IsTrue(signaledEvents.Wait(15000));
            Thread.Sleep(3000);
            impl?.Stop();
            Assert.IsFalse(messageReceived);
        }

        [TestMethod]
        public void StartProtocol_ConfigureNoRegistration_ExpectGettingAdHocMessages()
        {
            var signaledEvents = new CountdownEvent(3);
            AdHocMessagingProtocolImpl impl = null;
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;

            var mock = new Mock<IAdhocManager>();
            var testMessage = this.CreateAdhocTestMessage();
            mock.Setup(m => m.GetAllMessagesForUnitAsync(unitName, string.Empty, It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message> { testMessage }));
            Assert.IsFalse(string.IsNullOrEmpty(testMessage.Text));
            int adhocMessageCount = 0;
            bool registrationCompleted = false;

            Task.Run(
                () =>
                    {
                        // indicate we do not care to register, override default settings from our file
                        var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
                        config.EnableUnitRegistration = false;
                        config.AdHocMessageTimerSettings = new TimerSettings(1); // adjust for fast interval to unit test
                        var adHocMessageService = new AdHocMessageService(CreateAdHocMessageServiceConfig(config), mock.Object);

                        impl = CreateMessagingProtocolImpl(config, adHocMessageService);
                        Assert.AreEqual(1, impl.Config.AdHocMessageTimerSettings.Interval.TotalSeconds);

                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("*Started event");
                                signaledEvents.Signal();
                            };
                        impl.OnRegisterCompleted += (sender, info) =>
                            {
                                // we do not expect this    config.EnableUnitRegistration = false;
                                registrationCompleted = true;
                            };
                        impl.OnAdHocMessagesReceived += (sender, args) =>
                            {
                                Debug.WriteLine("* {0} OnAdHocMessagesReceived event AdHoc Messages Count={1}", adhocMessageCount, args.Messages.Count);
                                if (args.Messages.Count == config.MaxAdHocMessages)
                                {
                                    foreach (var m in args.Messages)
                                    {
                                        Debug.WriteLine("   Text=[{0}], Title=[{1}]", m.Text, m.Title);
                                    }

                                    if (args.Messages.First().Text == testMessage.Text && args.Messages.First().Title == testMessage.Title)
                                    {
                                        Debug.WriteLine("*Success");
                                        signaledEvents.Signal();
                                        adhocMessageCount++;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("! Failed to Signal Text != " + testMessage.Text);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("! args.Messages.Count {0} != config.MaxAdHocMessages. config.MaxAdHocMessages {1}", args.Messages.Count, config.MaxAdHocMessages);
                                }
                            };
                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                                            
                    });


            Assert.IsTrue(signaledEvents.Wait(10000), "Missed 2 Events Count=" + signaledEvents.CurrentCount);

            impl?.Stop();

            Assert.AreEqual(false, registrationCompleted);
            Assert.AreEqual(2, adhocMessageCount);
        }

        [TestMethod]
        public void StartProtocol_ConfigureNormalToRegister_ExpectRegistrationAndGettingAdHocMessages()
        {
            var unitName = MessageDispatcher.Instance.LocalAddress.Unit;

            var mock = new Mock<IAdhocManager>();

            // setup mocked return for adhoc messages
            var testMessage = this.CreateAdhocTestMessage();
            mock.Setup(m => m.GetAllMessagesForUnitAsync(unitName, string.Empty, It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<Message> { testMessage }));

            Assert.IsFalse(string.IsNullOrEmpty(testMessage.Text));

            var registrationRequest = new AdHocRegisterRequest("BUS123", unitName);
            Assert.AreEqual("BUS123", registrationRequest.VehicleId);
            Assert.AreEqual(unitName, registrationRequest.FirstUnit);

            mock.Setup(m => m.RegisterVehicleAndUnitAsync(registrationRequest.VehicleId, unitName, It.IsAny<string>(), It.IsAny<string>(), true)).Returns(Task.FromResult(HttpStatusCode.OK));

            // indicate we do not care to register, override default settings from our file
            var config = AdHocMessagingProtocolConfig.ReadConfig("AdHocMessagingProtocolConfig.xml");
            config.EnableUnitRegistration = true;
            config.MaxAdhocRegistrationAttempts = 3;
            config.RegisterUnitTimerSettings = new TimerSettings(1);
            Assert.AreEqual(1, config.RegisterUnitTimerSettings.Interval.TotalSeconds);
            config.AdHocMessageTimerSettings = new TimerSettings(1);
            Assert.AreEqual(1, config.AdHocMessageTimerSettings.Interval.TotalSeconds);

            var adHocServiceConfig = CreateAdHocMessageServiceConfig(config);
            var adHocMessageService = new AdHocMessageService(adHocServiceConfig, mock.Object);
            var vehicleInfo = new VehicleInfo("BUS123", unitName);

            var signaledEvents = new CountdownEvent(5);

            var impl = CreateMessagingProtocolImpl(config, adHocMessageService);
            var vehicleRegistered = new ManualResetEvent(false);
            Task.Run(
                () =>
                    {
                        impl.VehicleInfoChanged += (sender, info) =>
                            {
                                Debug.WriteLine("*VehicleInfoChanged event");
                                signaledEvents.Signal();
                            };

                        impl.Started += (sender, args) =>
                            {
                                Debug.WriteLine("*Started event");
                                signaledEvents.Signal();
                            };

                        impl.OnRegisterCompleted += (sender, v) =>
                            {
                                if (v != null)
                                {
                                    signaledEvents.Signal();
                                    Debug.WriteLine("*OnRegisterCompleted event " + v.ToString());
                                    vehicleRegistered.Set();
                                }
                            };

                        impl.OnAdHocMessagesReceived += (sender, args) =>
                            {
                                Debug.WriteLine("OnAdHocMessagesReceived event Count=" + args.Messages.Count);
                                if (args.Messages.Count == config.MaxAdHocMessages)
                                {
                                    foreach (var m in args.Messages)
                                    {
                                        Debug.WriteLine("Text=[{0}], Title=[{1}]", m.Text, m.Title);
                                    }

                                    if (args.Messages.First().Text == testMessage.Text && args.Messages.First().Title == testMessage.Title)
                                    {
                                        signaledEvents.Signal();
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("! args.Messages.Count {0} != config.MaxAdHocMessages. config.MaxAdHocMessages {1}", args.Messages.Count, config.MaxAdHocMessages);
                                }
                            };

                        // setup the initial current vehicle info with a non empty string for the bus/vehicle id
                        impl.UpdateVehicleInfo(vehicleInfo);
                        Assert.AreEqual(ServiceRunState.RegisterUnit, impl.ServiceRunState);
                        Assert.IsNotNull(ProtocolHost);
                        impl.Run(ProtocolHost); // blocks                        
                    });

            // simulate  Medi message in to update and set the VehicleInfo

            Assert.IsTrue(signaledEvents.Wait(5000), "Not signaled");

            //  MessageDispatcher.Instance.Broadcast(vehicleInfo);

            impl?.Stop();
        }


        [TestInitialize]
        public void TestInit()
        {
            this.InitializeMedi();
        }

        [TestMethod]
        public void ReadWriteVehiclePositionFile()
        {
            var vehicleInfo = new VehicleInfo("BUS123", Environment.MachineName);
            var file = @"C:\Temp\VehiclePosition.xml";

            // var file = PathManager.Instance.CreatePath(FileType.Data, "VehiclePosition.xml");
            Debug.WriteLine("Writing Config file " + file);
            using (var fileStream = File.Create(file))
            {
                var s = new XmlSerializer(typeof(VehicleInfo));
                s.Serialize(fileStream, vehicleInfo);
            }

            Assert.IsTrue(File.Exists(file));
            var m = this.ReadVehiclePositionFile(file);
            Assert.IsNotNull(m);
            Assert.AreEqual(m.VehicleId, vehicleInfo.VehicleId);
            Assert.AreEqual(m.FirstUnitName, vehicleInfo.FirstUnitName);
        }

        public VehicleInfo ReadVehiclePositionFile(string file)
        {
            using (var fileStream = File.OpenRead(file))
            {
                var s = new XmlSerializer(typeof(VehicleInfo));
                return s.Deserialize(fileStream) as VehicleInfo;
            }
        }

        private IAdHocMessageServiceConfig CreateAdHocMessageServiceConfig(IAdHocMessagingProtocolConfig config)
        {
            var adHocMessageServiceConfig = CreateAdHocMessageServiceConfig();
            adHocMessageServiceConfig.MaxAdHocMessages = config.MaxAdHocMessages;
            adHocMessageServiceConfig.MaxRegistrationAttempts = config.MaxAdhocRegistrationAttempts;
            adHocMessageServiceConfig.DestinationsApiUri = config.DestinationsApiUri.HostUri;
            adHocMessageServiceConfig.AdHocApiUri = config.AdHocApiUri.HostUri;
            adHocMessageServiceConfig.ApiTimeout = config.ApiTimeout;
            return adHocMessageServiceConfig;
        }


        private static AdHocMessageServiceConfig CreateAdHocMessageServiceConfig(
            string adHocUrl = DefaultAdHocMessageApiUrl,
            string destinationsUrl = DefaultDestinationsApiUrl,
            int maxAdHocMessages = 3,
            int maxRegistationAttempts = 3,
            int timeout = 5000)
        {
            var config = AdHocMessageServiceConfig.CreateAdHocMessageServiceConfig(adHocUrl, destinationsUrl, timeout, maxAdHocMessages, maxRegistationAttempts);
            Assert.AreEqual(3, config.MaxAdHocMessages);
            Assert.AreEqual(3, config.MaxRegistrationAttempts);
            Assert.AreEqual(5000, config.ApiTimeout);
            return config;
        }

        private static AdHocMessagingProtocolImpl CreateMessagingProtocolImpl(
            IAdHocMessagingProtocolConfig config = null,
            IAdHocMessageService adHocMessageService = null)
        {
            var configExist = File.Exists(AdHocMessagingProtocolConfig.DefaultConfigFileName);
            Assert.IsNotNull(Dictionary, "dictionary.xml missing");
            Assert.IsTrue(configExist);

            var impl = new AdHocMessagingProtocolImpl(config, adHocMessageService, Dictionary);
            Assert.IsNotNull(impl.Dictionary);
            Assert.IsNotNull(impl.Config);
            Debug.WriteLine("MaxAdHoc Messages = " + config.MaxAdHocMessages);
            Assert.IsNotNull(impl.AdHocMessageService);
            return impl;
        }

        private static ProtocolHost CreateProtocolHost(Dictionary dictionary)
        {
            var config = new ProtranConfig();
            config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "MessagingProtocol" });
            return new ProtocolHost(config.Protocols, dictionary);
        }

        private Message CreateAdhocTestMessage(string test = "TestText", string title = "Title")
        {
            return new Message { Id = 123, IsActive = true, Text = test, Title = title, Type = "Information" };
        }

        private AdHocMessages CreateMocAdhocMessage(string route = "")
        {
            var m = new AdHocMessages();
            m.Messages.Add(
                new XimpleAdHocMessage
                {
                    Title = "Test",
                    Text = "Hello World",
                    Type = "Information",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMinutes(5),
                    Route = route
                });
            return m;
        }

        private void InitializeMedi()
        {
            // use the same mechanics as the host apps so we can run them standalone and debug messages to/from them
            var configFileName = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            Assert.IsTrue(File.Exists(configFileName));
            var fileConfigurator = new FileConfigurator(
                configFileName,
                Environment.MachineName,
                Environment.MachineName);
            MessageDispatcher.Instance.Configure(fileConfigurator);
            MessageDispatcher.Instance.Subscribe<Ximple>(this.XimpleReceived);
        }

        private void UnIntiMidi()
        {
            MessageDispatcher.Instance.Unsubscribe<Ximple>(this.XimpleReceived);
        }

        private void XimpleReceived(object sender, MessageEventArgs<Ximple> e)
        {
            var ximple = e.Message;

            if (VeroseXimpleDebug)
#pragma warning disable 162
            {
                foreach (var ximpleCell in ximple.Cells)
                {
                    Debug.WriteLine(ximpleCell.ToString());
                }
            }
#pragma warning restore 162


        }
    }
}