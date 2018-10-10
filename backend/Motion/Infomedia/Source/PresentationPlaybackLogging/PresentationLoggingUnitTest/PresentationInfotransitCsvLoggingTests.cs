namespace Luminator.PresentationLogging.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;
    using Luminator.Utility.CsvFileHelper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [DeploymentItem("medi.config")]
    [TestClass]
    public class PresentationInfotransitCsvLoggingTests
    {
        private static ICollection<InfotransitPresentationInfo> logEntries = new List<InfotransitPresentationInfo>();
        
        public const string TestFolder = @"C:\temp";

        private const string TestRolloverFolder = @"C:\temp\Rollover";
        private static string outFolder;
        private static bool foundFixedDriveD;
        private static TestContext testContext;

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        public static Dictionary Dictionary { get; private set; }

        /// <summary>The class init.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testContext = context;
            outFolder = testContext.TestDeploymentDir;
            Directory.CreateDirectory(TestFolder);
            foundFixedDriveD = DriveInfo.GetDrives().Any(m => m.Name == @"D:\" && m.DriveType == DriveType.Fixed);

            Dictionary = PresentationInfotransitCsvLogging.ReadDictionaryFile();
        }

        /// <summary>
        /// Creates a mock csv logger.
        /// </summary>
        /// <returns>
        /// The <see cref="Mock"/>The logger mock
        /// </returns>
        public static Mock<ICsvLogging<InfotransitPresentationInfo>> CreateMockLoggerSetupToLogEntries()
        {
            logEntries = null;
            var mockLogger = new Mock<ICsvLogging<InfotransitPresentationInfo>>();
            mockLogger.Setup(m => m.WriteAll(It.IsAny<ICollection<InfotransitPresentationInfo>>())).Callback<ICollection<InfotransitPresentationInfo>>(l => logEntries = l);

            return mockLogger;
        }

        [TestMethod]
        [DeploymentItem("medi.config")]
        public void IntegrationTest_WriteAsync_WillWrite_AllEntries_ToFile()
        {
            const string FileName = "StartPresentationPlayLogTest.csv";

            var config = CreateConfig();

            var outputFolder = PresentationLoggingUnitTest.TestFolder;

            if (!foundFixedDriveD)
            {
                outputFolder = TestRolloverFolder;
            }

            var outputFile = Path.Combine(outputFolder, FileName);
            File.Delete(outputFile);
            config.ClientConfig.LogFilePath = outputFile;

            Console.WriteLine("Creating files to " + outputFolder);
            using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config))
            {
                infotransitLogger.Start();

                this.SendVehiclePositionUpdate("1.23", "4.56");
                var info1 = new InfotransitPresentationInfo
                {
                    StartedLatitude = "33.019844",
                    StartedLongitude = "-96.698883",
                    StoppedLatitude = "33.019846",
                    StoppedLongitude = "-96.698886",
                    FileName = @"C:\Foobar1.jpg",
                    VehicleId = "112233",
                    Route = "ROUTE1",
                    UnitName = "TFT-1-2-3-4",
                    Duration = 10
                };

                this.SendInfotransitPresentationInfoMessage(info1);

                var info2 = new InfotransitPresentationInfo
                {
                    StartedLatitude = "33.019844",
                    StartedLongitude = "-96.698883",
                    StoppedLatitude = "33.019846",
                    StoppedLongitude = "-96.698886",
                    FileName = @"C:\Foobar2.jpg",
                    VehicleId = "112233",
                    Route = "ROUTE2",
                    UnitName = "TFT-1-2-3-4",
                    Duration = 20
                };

                this.SendInfotransitPresentationInfoMessage(info2);

                var info3 = new InfotransitPresentationInfo
                {
                    StartedLatitude = "33.019811",
                    StartedLongitude = "-96.698822",
                    FileName = @"C:\Foobar3.jpg",
                    VehicleId = "112233",
                    Route = "ROUTE3",
                    UnitName = "TFT-1-2-3-4",
                    Duration = 10
                };

                this.SendInfotransitPresentationInfoMessage(info3);
                this.SendInfotransitPresentationInfoMessage(new InfotransitPresentationInfo());
                infotransitLogger.Stop();
            }

            var fileExist = File.Exists(outputFile);
            Assert.IsTrue(fileExist, "File output not found " + outputFile);
            var lines = File.ReadAllLines(outputFile).Length;
            Assert.AreEqual(4, lines);
        }

        [TestMethod]
        [DeploymentItem("medi.config")]
        public void Start_WillSubscribe_To_VehiclePositionMessages()
        {
            var config = CreateConfig();
            var mockLoggingManager = this.CreateMockLoggingManager();
            using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config, mockLoggingManager.Object))
            {
                infotransitLogger.Start();

                this.SendXimpleUpdateMessage();
            }

            mockLoggingManager.Verify(m => m.SetVehicleId("112233"));
        }

        [TestMethod]
        [DeploymentItem("medi.config")]
        public void Start_WillSubscribe_To_VehicleUnitInfoMessages()
        {
            var config = CreateConfig();
            var mockLoggingManager = this.CreateMockLoggingManager();
            using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config, mockLoggingManager.Object))
            {
                infotransitLogger.Start();
                this.SendVehicleUnitInfoMessage();
            }

            mockLoggingManager.Verify(m => m.UpdateVehicleUnitInfo(It.IsAny<VehicleUnitInfo>()));
        }

        //[TestMethod]
        //[DeploymentItem("medi.config")]
        //public void VehicleUnitInfoMessages_WillInclude_RouteInformation_InLogs()
        //{
        //    var config = CreateConfig();
        //    var mockLogger = CreateMockLoggerSetupToLogEntries();
            
        //    var loggingManager = new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(mockLogger.Object);
            
        //    using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config, loggingManager))
        //    {
        //        infotransitLogger.Start();
        //        var unit = MessageDispatcher.Instance.LocalAddress.ToString();
        //        loggingManager.StartNewLayoutSession(unit);
        //        this.SendVehicleUnitInfoMessage();
        //        loggingManager.UpdateResource(123, "TestLayout", unit, "TestFile.jpg", "456", TimeSpan.FromSeconds(10), new ImageItem());
                
        //        infotransitLogger.Stop();
        //    }
            
        //    // We only had one resource reported (TestFile.jpg)
        //    Assert.AreEqual(1, logEntries.Count);
        //    Assert.AreEqual("R1", logEntries.First().Route);
        //}

        //[TestMethod]
        //public void NoGPSUpdates_WillWrite_EmptyStrings_AsGPSCoordinates_ToLogs()
        //{
        //    var config = CreateConfig();
        //    var mockLogger = CreateMockLoggerSetupToLogEntries();

        //    var loggingManager = new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(mockLogger.Object);

        //    using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config, loggingManager))
        //    {
        //        infotransitLogger.Start();
        //        var unit = MessageDispatcher.Instance.LocalAddress.ToString();
        //        loggingManager.StartNewLayoutSession(unit);

        //        // If a LAM is not attached, we won't get GEO data. Before the fix from this test,
        //        // we were writing NaN for the geo coordinates.
        //        this.SendVehicleUnitInfoMessageWithNoGeoData();
        //        loggingManager.UpdateResource(123, "TestLayout", unit, "TestFile.jpg", "456", TimeSpan.FromSeconds(10), new ImageItem());

        //        infotransitLogger.Stop();
        //    }

        //    Assert.AreEqual(string.Empty, logEntries.First().StartedLatitude);
        //    Assert.AreEqual(string.Empty, logEntries.First().StartedLongitude);
        //    Assert.AreEqual(string.Empty, logEntries.First().StoppedLatitude);
        //    Assert.AreEqual(string.Empty, logEntries.First().StoppedLongitude);
        //}


        [TestMethod]
        [DeploymentItem("medi.config")]
        public void Start_WillSubscribe_To_VehicleIdMessages()
        {
            var config = CreateConfig();
            var mockLoggingManager = this.CreateMockLoggingManager();
            using (var infotransitLogger = this.CreatePresentationInfotransitCsvLogging(config, mockLoggingManager.Object))
            {
                infotransitLogger.Start();

                this.SendVehiclePositionUpdate("1", "2");
            }
            mockLoggingManager.Verify(m => m.UpdateCurrentGpsPosition("1", "2"));
        }

        
        /// <summary>
        ///     Create fake Ximple data used to emulate Ximple data via Protran for InfoTainmentSystemStatus that comes from the
        ///     LAM/Prosys hardware on the bus
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        private static Ximple CreateMockInfoTainmentSystemStatusXimple(
            string latitude = "33.019844",
            string longitude = "-96.698883",
            string vehicleId = "112233")
        {
            var table = Dictionary?.FindXimpleTable("InfoTainmentSystemStatus");
            Assert.IsNotNull(table);
            var ximple = new Ximple("2.0.0");
            ximple.Cells.Add(new XimpleCell { TableNumber = table.Index });
            ximple.Cells.Add(Dictionary.CreateXimpleCell(table, "VehicleId", vehicleId));
            ximple.Cells.Add(Dictionary.CreateXimpleCell(table, "Latitude", latitude));
            ximple.Cells.Add(Dictionary.CreateXimpleCell(table, "Longitude", longitude));

            var routeTable = Dictionary?.FindXimpleTable("Route");
            ximple.Cells.Add(Dictionary.CreateXimpleCell(routeTable, "Route", "B17"));
            return ximple;
        }

        private Mock<ILoggingManager> CreateMockLoggingManager()
        {
            return new Mock<ILoggingManager>();
        }

        private void SendVehiclePositionUpdate(string lat, string lon)
        {
            VehiclePositionMessage message = new VehiclePositionMessage(lat, lon);
            MessageDispatcher.Instance.Broadcast(message);
        }

        private void SendInfotransitPresentationInfoMessage(InfotransitPresentationInfo info)
        {
            AddInfotransitPresentationInfoEntry entry = new AddInfotransitPresentationInfoEntry(info);
            MessageDispatcher.Instance.Broadcast(entry);
        }

        private void SendXimpleUpdateMessage()
        {
            var ximpleMessage = CreateMockInfoTainmentSystemStatusXimple();
            MessageDispatcher.Instance.Broadcast(ximpleMessage);
        }

        private void SendVehicleUnitInfoMessage()
        {
            var vehicleInfo = new VehicleInfo("BUS123", new List<string> { MessageDispatcher.Instance.LocalAddress.ToString() });
            var vehicleUnitInfo = new VehicleUnitInfo(
                vehicleInfo,
                new VehiclePositionMessage("33.019844", "-96.698883", "0", "R1", "T1"));
            MessageDispatcher.Instance.Broadcast(vehicleUnitInfo);
        }

        private void SendVehicleUnitInfoMessageWithNoGeoData()
        {
            var vehicleInfo = new VehicleInfo("BUS123", new List<string> { MessageDispatcher.Instance.LocalAddress.ToString() });
            var vehicleUnitInfo = new VehicleUnitInfo(
                vehicleInfo,
                new VehiclePositionMessage());
            MessageDispatcher.Instance.Broadcast(vehicleUnitInfo);
        }

        /// <summary>
        /// Return the real deal
        /// </summary>
        /// <param name="config">The configuration to use</param>
        /// <returns>Presentation logging for proof of play, using a CSV logger.</returns>
        private PresentationInfotransitCsvLogging CreatePresentationInfotransitCsvLogging(PresentationPlayLoggingConfig config)
        {
            return new PresentationInfotransitCsvLogging(config, new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(new CSVLogger<InfotransitPresentationInfo>()));
        }

        private PresentationInfotransitCsvLogging CreatePresentationInfotransitCsvLogging(PresentationPlayLoggingConfig config, ILoggingManager loggingManager)
        {
            return new PresentationInfotransitCsvLogging(config, loggingManager);
        }

        public static PresentationPlayLoggingConfig CreateConfig(int maxRecords = 1000, int maxFileSize = 0)
        {
            var realConfig = new PresentationPlayLoggingConfig
            {
                ClientConfig = new PresentationPlayLoggingClientConfig
                {
                    LogFilePath = "Test.csv",
                    FileNameRolloverType = FileNameRolloverType.None,
                    MaxFileSize = maxFileSize,
                    MaxRecords = maxRecords,
                    RollOverLogOutputFolder = @"D:\PresentationPlayLogs"
                },

            };

            return realConfig;
        }
    }
}

