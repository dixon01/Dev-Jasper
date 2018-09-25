// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationLoggingUnitTest.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The presentation logging unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationLogging.UnitTest
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Luminator.PresentationPlayLogging.Config;
    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;
    using Luminator.Utility.CsvFileHelper;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>The presentation logging unit test.</summary>
    [TestClass]
    [DeploymentItem("medi.config")]
    [DeploymentItem("dictionary.xml")]
    [DeploymentItem("PresentationPlayLoggingConfig.xml")]
    public class PresentationLoggingUnitTest
    {
        public const string TestFolder = @"C:\temp";

        private const string TestRolloverFolder = @"C:\temp\Rollover";

        private static bool foundFixedDriveD;

        private static string outFolder;

        private static TestContext testContext;

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        public static Dictionary Dictionary { get; private set; }

        /// <summary>The class cleanup.</summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            UnInitMedi();
        }

        /// <summary>The class initialization.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testContext = context;
            outFolder = testContext.TestDeploymentDir;
            Directory.CreateDirectory(TestFolder);
            IntitMedi();
            foundFixedDriveD = DriveInfo.GetDrives().Any(m => m.Name == @"D:\" && m.DriveType == DriveType.Fixed);

            Dictionary = PresentationInfotransitCsvLogging.ReadDictionaryFile();
        }

        /// <summary>The construct infotransit presentation info.</summary>
        [TestMethod]
        public void ConstructInfotransitPresentationInfo()
        {
            var m = new InfotransitPresentationInfo();
            Assert.IsNotNull(m.Created);
            Assert.AreEqual(string.Empty, m.StartedLongitude);
            Assert.AreEqual(string.Empty, m.StartedLatitude);
            Assert.AreEqual(string.Empty, m.FileName);
            Assert.AreEqual(string.Empty, m.ResourceId);
            Assert.AreEqual(string.Empty, m.VehicleId);
            Assert.AreEqual(Environment.MachineName, m.UnitName);
            Assert.IsFalse(m.IsValid);
        }

        /// <summary>The construct valid infotransit presentation info.</summary>
        [TestMethod]
        public void ConstructValidInfotransitPresentationInfo()
        {
            var m = new InfotransitPresentationInfo
                        {
                            StartedLatitude = "33.019844",
                            StartedLongitude = "-96.698883",
                            FileName = @"C:\Foobar.jpg",
                            VehicleId = "112233",
                            Route = "B17"
                        };
            Assert.IsNotNull(m.Created);
            Assert.AreEqual("33.019844", m.StartedLatitude);
            Assert.AreEqual("-96.698883", m.StartedLongitude);
            Assert.AreEqual(@"C:\Foobar.jpg", m.FileName);
            Assert.AreEqual("112233", m.VehicleId);
            Assert.AreEqual("B17", m.Route);
            Assert.AreEqual(Environment.MachineName, m.UnitName);
            Assert.IsTrue(m.IsValid);
        }

        /// <summary>The create image item as item base.</summary>
        [TestMethod]
        public void CreateImageItemAsItemBase()
        {
            var imageItem = CreateMockImageItem("UnitTest.jpg");
            var itemBase = (ItemBase)imageItem;
            Assert.IsNotNull(itemBase);
        }

        /// <summary>
        ///     The presentation infotransit logging construct with bogus config file.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void PresentationInfotransitLoggingConstructWithBogusConfigFile()
        {
            using (var logging = new PresentationInfotransitCsvLogging("BogusPresentationPlayLoggingConfig.xml", this.CreateLoggingManager()))
            {
                Console.WriteLine("Constructed");
                Assert.IsNotNull(logging.Config);
            }
        }

        /// <summary>The presentation infotransit logging construct with config file.</summary>
        [TestMethod]
        public void PresentationInfotransitLoggingConstructWithConfigFile()
        {
            using (var logging = new PresentationInfotransitCsvLogging("PresentationPlayLoggingConfig.xml", this.CreateLoggingManager()))
            {
                Console.WriteLine("Constructed");
                Assert.IsNotNull(logging.Config);
                Assert.IsTrue(logging.Config.ServerConfig.WatchFileFilter.Contains("*.csv"));
            }
        }

        /// <summary>The presentation logging from config Construct.</summary>
        [DeploymentItem("PresentationPlayLoggingConfigV1.0.xml")]
        [TestMethod]
        public void PresentationInfotransitLoggingConvig_V1()
        {
            var fileName = Path.Combine(outFolder, "PresentationPlayLoggingConfigV1.0.xml");
            if (!File.Exists(fileName))
            {
                Assert.Fail("Test File not found " + fileName);
            }

            var config = PresentationPlayLoggingConfig.ReadConfig(fileName);
            Assert.IsNotNull(config);

            using (var logging = new PresentationInfotransitCsvLogging(config, this.CreateLoggingManager()))
            {
                Console.WriteLine("Constructed");
                Assert.IsNotNull(logging.Config);
                Assert.AreEqual("*.csv", logging.Config.ServerConfig.WatchFileFilter);
            }
        }

        /// <summary>The presentation logging empty Construct.</summary>
        [TestMethod]
        public void PresentationInfotransitLoggingEmptyConstruct()
        {
            var loggingMananager = this.CreateLoggingManager();
            using (var logging = new PresentationInfotransitCsvLogging(loggingMananager))
            {
                Console.WriteLine("Constructed");
                Assert.IsNotNull(logging.Config);
                Assert.IsTrue(logging.Config.ServerConfig.WatchFileFilter.Contains("*.csv"));
            }
        }

        private ILoggingManager CreateLoggingManager()
        {
            return new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(new CSVLogger<InfotransitPresentationInfo>());
        }

        /// <summary>The read presentation log file.</summary>
        [TestMethod]
        [DeploymentItem("Write1000InfotransitPresentationInfoRecords.csv")]
        public void ReadAllPresentationInfotransiCsvFile()
        {
            if (!File.Exists("Write1000InfotransitPresentationInfoRecords.csv"))
            {
                Assert.Inconclusive("Test File Missing");
            }

            var records = PresentationInfotransitCsvLogging.ReadAll("Write1000InfotransitPresentationInfoRecords.csv");
            Assert.IsNotNull(records);
            Assert.AreEqual(1000, records.Count);
        }

        /// <summary>The read presentation logging config.</summary>
        [TestMethod]
        [DeploymentItem("PresentationPlayLoggingConfig.xml")]
        public void ReadPresentationLoggingConfig()
        {
            const string ConfigFileName = "PresentationPlayLoggingConfig.xml";
            if (!File.Exists(ConfigFileName))
            {
                Assert.Inconclusive("Test File Missing");
            }

            var config = PresentationPlayLoggingConfig.ReadConfig(ConfigFileName);
            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void StartLoggingTestMediVideoPlaybackEvent()
        {
            using (var playLogging =
                new PresentationInfotransitCsvLogging(PresentationPlayLoggingConfig.ConfigFileName, this.CreateLoggingManager()))
            {
                var signaled = new ManualResetEvent(false);
                playLogging.OnVideoPlaybackEvent += (sender, args) =>
                    {
                        Assert.IsNotNull(args);
                        Console.WriteLine("OnVideoPlaybackEvent Event Received " + args.VideoUri);
                        signaled.Set();
                    };

                playLogging.Start();
                var message = new VideoPlaybackEvent { ItemId = 1, Playing = true, VideoUri = "test.mp4" };
                MessageDispatcher.Instance.Broadcast(message);
            }
        }

        /// <summary>The start presentation infotransit logging.</summary>
        [TestMethod]
        public void StartPresentationInfotransitLogging()
        {
            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            Assert.IsNotNull(config.ClientConfig);
            Assert.IsNotNull(config.ServerConfig);
            if (!foundFixedDriveD)
            {
                config.ClientConfig.RollOverLogOutputFolder = TestRolloverFolder;
            }
            Console.WriteLine("Creating files to " + config.ClientConfig.RollOverLogOutputFolder);
            var feedbackMessageEvent = new ManualResetEvent(false);
            var positionChangeEvent = new ManualResetEvent(false);
            var presentationInfoChangeEvent = new ManualResetEvent(false);

            using (var playLogging = new PresentationInfotransitCsvLogging(config, this.CreateLoggingManager()))
            {
                playLogging.Start();

                Assert.IsTrue(Directory.Exists(config.ClientConfig.RollOverLogOutputFolder), "Output folder missing");
                Assert.IsTrue(playLogging.IsStarted);

                // test medi message received to event
                playLogging.OnFeedbackMessageReceived += (sender, args) =>
                    {
                        Console.WriteLine("OnFeedbackMessageReceived Event Received");
                        Assert.IsNotNull(args);
                        feedbackMessageEvent.Set();
                    };

                playLogging.OnVehiclePositionChanged += (sender, args) =>
                    {
                        Console.WriteLine("OnVehiclePositionChanged Event Received");
                        Assert.IsNotNull(args);
                        positionChangeEvent.Set();
                    };

                //playLogging.OnEnvironmentInfoChanged += (sender, args) =>
                //    {
                //        Console.WriteLine("OnPresentationInfoChanged Event Received");
                //        Assert.IsNotNull(args);
                //        Console.WriteLine(
                //            "OnPresentationInfoChanged Vehicle={0}, Latitude={1}, Longitude={2}, Route={3}",
                //            args.VehicleId,
                //            args.Latitude,
                //            args.Longitude,
                //            args.Route);
                //        presentationInfoChangeEvent.Set();
                //    };

                playLogging.OnNewLogFileCreated += (sender, s) =>
                    {
                        Console.WriteLine("Event On New File created " + s);
                    };

                var mockXimple = CreateMockInfoTainmentSystemStatusXimple();
                MessageDispatcher.Instance.Broadcast(mockXimple);

                // simulate a medi message of mocked screen change
                var screenChange = CreateMockPresentationScreenChanges();
                var feedbackMessage = playLogging.CreateFeedbackScreenChangeMessage(screenChange, "TFT-1-2-3");
                MessageDispatcher.Instance.Broadcast(feedbackMessage);

                var signaled = feedbackMessageEvent.WaitOne(5000);
                Assert.IsTrue(signaled, "feedbackMessageEvent Not Signaled");

                var vehiclePositionMessage = new VehiclePositionMessage("33.019844", "-96.698883");
                Assert.AreEqual(true, vehiclePositionMessage.IsValid, "Geo Coordinated not valid");
                MessageDispatcher.Instance.Broadcast(vehiclePositionMessage);
                signaled = positionChangeEvent.WaitOne(5000);

                Assert.IsTrue(signaled, "vehicle PositionChangeEvent Not Signaled");

                //signaled = presentationInfoChangeEvent.WaitOne(5000);
                //Assert.IsTrue(signaled, "presentationInfoChangeEvent Not Signaled");

                playLogging.Stop();
            }
        }
        
        [TestMethod]
        public void StartPresentationLoggingAndVehiclePosition()
        {
            const string FileName = "StartPresentationLoggingSndVehiclePosition.csv";

            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            Assert.IsNotNull(config.ClientConfig);
            Assert.IsNotNull(config.ServerConfig);
            var outputFolder = TestFolder;
            Assert.IsFalse(string.IsNullOrEmpty(outputFolder));
            if (!foundFixedDriveD)
            {
                outputFolder = TestRolloverFolder;
            }

            var outputFile = Path.Combine(outputFolder, FileName);
            File.Delete(outputFile);
            Console.WriteLine("Creating files to " + outputFolder);
            var vehiclePositionChangedEvent = new ManualResetEvent(false);

            using (var playLogging = new PresentationInfotransitCsvLogging(config, this.CreateLoggingManager()))
            {
                playLogging.Start();
                Assert.IsTrue(Directory.Exists(outputFolder), "Output folder missing " + outputFolder);
                Assert.IsTrue(playLogging.IsStarted, "IsStarted Not True");

                // test medi message received to event
                playLogging.OnVehiclePositionChanged += (sender, message) =>
                    {
                        Console.WriteLine("OnVehiclePositionChanged Event Received Position = " + message.ToString());
                        vehiclePositionChangedEvent.Set();
                    };

                var vehiclePositionMessage = new VehiclePositionMessage("33.019811", "-96.698822");
                MessageDispatcher.Instance.Broadcast(vehiclePositionMessage);

                var signaled = vehiclePositionChangedEvent.WaitOne(5000);
                Assert.IsTrue(signaled, "No Successful VehiclePositionMessage of receiving feedback message");

                Assert.IsNotNull(playLogging.LastVehiclePositionMessage);
                Console.WriteLine("Last Vehicle Position Message = " + vehiclePositionMessage);
                Assert.AreEqual("33.019811", playLogging.LastVehiclePositionMessage.GeoCoordinate.Latitude.ToString());
                Assert.AreEqual(
                    "-96.698822",
                    playLogging.LastVehiclePositionMessage.GeoCoordinate.Longitude.ToString());
            }
        }

        [TestMethod]
        public void Start_SubscribesTo_VehiclePositionChanged_Event()
        {
            var vehiclePositionChangedEvent = new ManualResetEvent(false);
            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            var mockLoggingManager = this.CreateMockLoggingManager();
            mockLoggingManager.Setup(m => m.UpdateCurrentGpsPosition(It.IsAny<string>(), It.IsAny<string>())).Callback(() => vehiclePositionChangedEvent.Set());
            
            using (var playLogging = new PresentationInfotransitCsvLogging(config, mockLoggingManager.Object))
            {
                playLogging.Start();
                var vehiclePositionMessage = new VehiclePositionMessage("1", "2");
                Task.Run(() => { MessageDispatcher.Instance.Broadcast(vehiclePositionMessage); });

                vehiclePositionChangedEvent.WaitOne(5000);
                mockLoggingManager.Verify(m => m.UpdateCurrentGpsPosition("1", "2"));
            }
        }

        private Mock<ILoggingManager> CreateMockLoggingManager()
        {
            return new Mock<ILoggingManager>();
        }

        [TestMethod]
        public void MissingLogOutputPath_ShouldUse_TheDefaultValue()
        {
            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            Assert.IsNotNull(config.ClientConfig);
            Assert.IsNotNull(config.ServerConfig);

            var mockLoggingManager = this.CreateMockLoggingManager();
            
            config.ClientConfig.LogFilePath = string.Empty;
            if (!foundFixedDriveD)
            {
                config.ClientConfig.RollOverLogOutputFolder = TestRolloverFolder;
            }

            using (var playLogging = new PresentationInfotransitCsvLogging(config, mockLoggingManager.Object))
            {
                playLogging.Start();
            }

            // Verify we use the default log file path, we don't care about the other parameters in this test.
            mockLoggingManager.Verify(
                m => m.Start(
                    PresentationPlayCsvLogging<InfotransitPresentationInfo>.DefaultPresentationCsvPlayLogFileName,
                    It.IsAny<string>(),
                    It.IsAny<FileNameRolloverType>(),
                    It.IsAny<long>(),
                    It.IsAny<long>()));
        }

        [TestMethod]
        public void StartPresentationLoggingWriteViaMediMessage()
        {
            const string FileName = "StartPresentationLoggingWriteViaMediMessageTest.csv";

            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            Assert.IsNotNull(config.ClientConfig);
            Assert.IsNotNull(config.ServerConfig);
            var outputFolder = config.ClientConfig.RollOverLogOutputFolder;
            if (!foundFixedDriveD)
            {
                outputFolder = TestRolloverFolder;
            }

            var outputFile = Path.Combine(outputFolder, FileName);
            File.Delete(outputFile);
            Console.WriteLine("Creating files to " + outputFolder);
            config.ClientConfig.LogFilePath = outputFile;
            using (var playLogging = new PresentationInfotransitCsvLogging(config, this.CreateLoggingManager()))
            {
                playLogging.Start();
                Assert.IsTrue(Directory.Exists(outputFolder), "Output folder missing " + outputFolder);
                Assert.IsTrue(playLogging.IsStarted, "IsStarted Not True");

                // write entry via medi message
                var infotransitPresentationInfo =
                    new InfotransitPresentationInfo
                        {
                            StartedLatitude = "33.019811",
                            StartedLongitude = "-96.698822",
                            FileName = @"C:\Foobar5.jpg",
                            VehicleId = "112233",
                            Route = "ROUTE5",
                            UnitName = "TFT-1-2-3-4"
                        };
                Assert.IsTrue(infotransitPresentationInfo.IsValid);

                // subscribe to this message internally to allow writes optionally via medi
                var addInfotransitPresentationInfoEntry =
                    new AddInfotransitPresentationInfoEntry(infotransitPresentationInfo);
                MessageDispatcher.Instance.Broadcast(addInfotransitPresentationInfoEntry);
                playLogging.Stop();
            }
            
            var fileExist = File.Exists(outputFile);
            Assert.IsTrue(fileExist, "File output not found " + outputFile);
            var lines = File.ReadAllLines(outputFile).Length;
            Assert.AreEqual(2, lines);
        }

        [TestMethod]
        public void VerifyConnectionStringDefined()
        {
            var config = PresentationPlayLoggingConfig.ReadConfig(PresentationPlayLoggingConfig.ConfigFileName);
            Assert.IsNotNull(config.ClientConfig);
            Assert.IsNotNull(config.ServerConfig);
#if DEBUG
            Assert.IsNotNull(config.ServerConfig.ConnectionStringDebug);
#else
            Assert.IsNotNull(config.ServerConfig.ConnectionStringRelease);
#endif

            Assert.IsNotNull(config.ServerConfig.ConnectionString);
            Console.WriteLine(config.ServerConfig.ConnectionString);
        }

        ///// <summary>The write multiple on threads.</summary>
        //[TestMethod]
        //public void Write1000InfotransitPresentationInfoRecords()
        //{
        //    var testFile = Path.Combine(TestFolder, "Write1000InfotransitePresentationInfoRecords.csv");
        //    Console.WriteLine("WriteMultipleOnThreads Start testFile = " + testFile);
        //    File.Delete(testFile);
        //    const int MaxRecords = 1000;

        //    using (var logging = new PresentationInfotransitCsvLogging())
        //    {
        //        var config = logging.Config;
        //        Console.WriteLine("RollOverLogOutputFolder=" + config.ClientConfig?.RollOverLogOutputFolder);

        //        logging.Start(testFile);
        //        var records = new List<InfotransitPresentationInfo>();
        //        var r = new Random();
        //        var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
        //        var resourceId = Guid.NewGuid().ToString();

        //        for (var i = 0; i < MaxRecords; i++)
        //        {
        //            // create fake data
        //            var model = CreateMockInfotransitPresentationInfo(r, i, ref fileName, ref resourceId);
        //            records.Add(model);
        //        }

        //        logging.WriteAll(records);
        //    }

        //    Assert.IsTrue(File.Exists(testFile));
        //    Assert.IsTrue(IsFileSizeNonZero(testFile));
        //    var recordCount = GetCsvFileRecordCount<InfotransitPresentationInfo>(testFile);
        //    Console.WriteLine("Final file Records Cont = " + recordCount);
        //    Assert.AreNotEqual(0, recordCount, "Record Count in file != 0");
        //    Assert.AreEqual(1000, recordCount);

        //    var allRecords = PresentationInfotransitCsvLogging.ReadAll(testFile);
        //    Assert.AreEqual(MaxRecords, allRecords.Count);
        //}

        //[TestMethod]
        //public void WriteAndTestNumericalRollover()
        //{
        //    this.WriteAndTestTimestampRollover(FileNameRolloverType.Numerical);
        //}

        //[TestMethod]
        //public void WriteAndTestTimestampRollover()
        //{
        //    this.WriteAndTestTimestampRollover(FileNameRolloverType.TimeStampTicks);
        //}

        //public void WriteAndTestTimestampRollover(FileNameRolloverType fileNameRolloverType)
        //{
        //    var testFile = Path.Combine(TestFolder, fileNameRolloverType + "-UnitTestRollover.csv");
        //    Console.WriteLine("WriteMultipleOnThreads Start testFile = " + testFile);
        //    File.Delete(testFile);

        //    DirectoryInfo dirInfo = null;
        //    var searchPattern = fileNameRolloverType + "*.csv";
        //    const int MaxRecords = 100;

        //    using (var logging = new PresentationInfotransitCsvLogging())
        //    {
        //        var config = logging.Config;
        //        Assert.IsNotNull(config.ClientConfig);
        //        Assert.IsNotNull(config.ClientConfig.RollOverLogOutputFolder);
        //        Console.WriteLine("RollOverLogOutputFolder=" + config.ClientConfig?.RollOverLogOutputFolder);

        //        // delete any existing files for testing
        //        dirInfo = new DirectoryInfo(config.ClientConfig.RollOverLogOutputFolder);
        //        var files = dirInfo.GetFiles(searchPattern).ToList();
        //        foreach (var f in files)
        //        {
        //            File.Delete(f.FullName);
        //        }

        //        logging.SetFileRolloverMaxRecords(10, fileNameRolloverType);

        //        logging.Start(testFile);

        //        var resourceId = Guid.NewGuid().ToString();
        //        var r = new Random();
        //        var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
        //        for (var i = 0; i < MaxRecords; i++)
        //        {
        //            // create fake data
        //            var model = CreateMockInfotransitPresentationInfo(r, i, ref fileName, ref resourceId);
        //            logging.WriteAsync(model);
        //        }
        //    }

        //    var newFiles = dirInfo.GetFiles(searchPattern).ToList();
        //    Assert.AreEqual(10, newFiles.Count());
        //}

        /// <summary>The write presentation logging config.</summary>
        [TestMethod]
        public void WritePresentationLoggingConfig()
        {
            var config = new PresentationPlayLoggingConfig();
            var fileName = Path.Combine(TestFolder, PresentationPlayLoggingConfig.ConfigFileName);
            File.Delete(fileName);

            PresentationPlayLoggingConfig.WriteConfig(config, fileName);
            Console.WriteLine(fileName);
            Assert.IsTrue(File.Exists(fileName));
        }

        private static ImageItem CreateMockImageItem(string fileName)
        {
            var imageItem = new ImageItem
                                {
                                    Filename = fileName,
                                    Blink = false,
                                    ElementId = 1,
                                    Scaling = ElementScaling.Fixed,
                                    Height = 480,
                                    Width = 640,
                                    Id = 1,
                                    Visible = true
                                };
            return imageItem;
        }

        /// <summary>
        ///     Create fake Ximple data used to emulate Ximple data via Protran for InfoTainmentSystemStatus that comes from the
        ///     LAM/Prosys hardware on the bus
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public static Ximple CreateMockInfoTainmentSystemStatusXimple(
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

        public static InfotransitPresentationInfo CreateMockInfotransitPresentationInfo(
            Random r,
            int i,
            ref string fileName,
            ref string resourceId)
        {
            var model = new InfotransitPresentationInfo { Duration = 30, PlayStarted = DateTime.Now };

            model.PlayStopped = model.PlayStarted.Value.AddSeconds(model.Duration);
            if (r.Next(1, 5) % 2 == 0)
            {
                // randomize play stopped
                model.PlayStopped = model.PlayStarted.Value.AddSeconds(r.Next(1, 30));
            }

            model.VehicleId = "123456789";
            model.Route = "ROUTE_" + DimMock.Random.String();
            model.PlayedDuration = (long)(model.PlayStopped.Value.TimeOfDay - model.PlayStarted.Value.TimeOfDay)
                .TotalSeconds;
            model.IsPlayInterrupted = model.PlayedDuration < model.Duration;
            if (r.Next(1, 10) % 2 == 0)
            {
                fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".jpg";
                resourceId = Guid.NewGuid().ToString();
            }

            model.FileName = fileName;
            model.ResourceId = resourceId;
            model.StartedLatitude = "33.019844";
            model.StartedLongitude = "-96.698883";
            model.StoppedLongitude = "-96.698884";
            model.StoppedLatitude = "33.019847";
            if (i % 2 == 0)
            {
                model.UnitName = Environment.MachineName;
            }
            else
            {
                model.UnitName = Environment.MachineName + "-1";
            }

            return model;
        }

        private static ScreenChanges CreateMockPresentationScreenChanges()
        {
            var imageItem = CreateMockImageItem("UnitTest.jpg");

            var itemBase = (ItemBase)imageItem;
            Assert.IsNotNull(itemBase);

            var screenChanges = new ScreenChanges();
            var screenRoot = new ScreenRoot { Root = new RootItem { ElementId = 1, Height = 600, Width = 400 } };
            screenRoot.Root.Items.Add(imageItem);

            var screenChange = new ScreenChange
                                   {
                                       Screen = new ScreenId { Id = "123", Type = PhysicalScreenType.TFT },
                                       ScreenRoot = screenRoot,
                                       Animation =
                                           new PropertyChangeAnimation
                                               {
                                                   Duration = TimeSpan.FromSeconds(
                                                       10),
                                                   Type =
                                                       PropertyChangeAnimationType
                                                           .None
                                               }
                                   };
            screenChanges.Changes.Add(screenChange);
            return screenChanges;
        }

        /// <summary>Get the csv file record count.</summary>
        /// <param name="fileName">The csv file name.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="int" />.</returns>
        public static int GetCsvFileRecordCount<T>(string fileName)
            where T : class
        {
            return CsvFileHelper<T>.GetCsvFileRecordCount(fileName);
        }

        private static void IntitMedi()
        {
            // MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
            const string MediFileName = "medi.config";
            var fileConfigurator = new FileConfigurator(MediFileName, Environment.MachineName, Environment.MachineName);
            if (MessageDispatcher.Instance.IsValidLocalAddress == false)
            {
                MessageDispatcher.Instance.Configure(fileConfigurator);
            }
        }

        public static bool IsFileSizeNonZero(string fileName)
        {
            var f = new FileInfo(fileName);
            Console.WriteLine("File " + fileName + " Size = " + f.Length);
            return f.Exists && f.Length > 0;
        }

        private static void UnInitMedi()
        {
        }

        // }
        // return cycleReference;
        // };
        // IsInEditMode = true,
        // Reference = cycle,
        // {

        // var cycleReference = new StandardCycleRefConfigDataViewModel(this.shell)
        // });
        // Duration = { Value = TimeSpan.FromSeconds(10) }
        // Layout = firstLayout,
        // {
        // new StandardSectionConfigDataViewModel(this.shell)

        // cycle.Sections.Add(

        // var cycle = new StandardCycleConfigDataViewModel(this.shell) { Name = new DataValue<string>(cycleName) };
        // }
        // cycleName = MediaStrings.CycleController_NewCycleName + newNumber;
        // newNumber += 1;
        // {
        // while (cycleList.Any(c => c.Name.Value == cycleName))
        // var cycleName = MediaStrings.CycleController_NewCycleName + newNumber;

        // var newNumber = 1;
        // var cycleList = currentProject.InfomediaConfig.Cycles.StandardCycles;
        // {
        // LayoutConfigDataViewModel firstLayout)
        // MediaProjectDataViewModel currentProject,

        // private CycleRefConfigDataViewModelBase CreateStandardCycle(
    }
}