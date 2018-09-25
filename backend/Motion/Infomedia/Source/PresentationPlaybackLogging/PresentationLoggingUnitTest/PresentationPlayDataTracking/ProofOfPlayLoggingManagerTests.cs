// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProofOfPlayLoggingManagerTests.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The proof of play logging manager tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationLogging.UnitTest.PresentationPlayDataTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Luminator.PresentationPlayLogging.Core.Interfaces;
    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;
    using Luminator.Utility.CsvFileHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The proof of play logging manager tests.
    /// </summary>
    [TestClass]
    public class ProofOfPlayLoggingManagerTests
    {

        private static IList<InfotransitPresentationInfo> logEntries = new List<InfotransitPresentationInfo>();

        /// <summary>
        /// Creates a mock csv logger.
        /// </summary>
        /// <returns>
        /// The <see cref="Mock"/>The logger mock
        /// </returns>
        public static Mock<ICsvLogging<InfotransitPresentationInfo>> CreateMockLogger()
        {
            return new Mock<ICsvLogging<InfotransitPresentationInfo>>();
        }

        /// <summary>
        /// Creates a mock csv logger.
        /// </summary>
        /// <returns>
        /// The <see cref="Mock"/>The logger mock
        /// </returns>
        public static Mock<ICsvLogging<InfotransitPresentationInfo>> CreateMockLoggerSetupToLogEntries()
        {
            logEntries.Clear();
            var mockLogger = new Mock<ICsvLogging<InfotransitPresentationInfo>>();
            mockLogger.Setup(m => m.WriteAsync(It.IsAny<InfotransitPresentationInfo>())).Callback<InfotransitPresentationInfo>(l => logEntries.Add(l));

            return mockLogger;
        }

        [TestMethod]
        public void StartNewLayoutSession_ShouldWrite_AccumulatedData()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            // loggingManager.StartNewLayoutSession(TestUnit);
            loggingManager.SetVehicleId("Bus1");
            loggingManager.SetRoute("Route66");

            // We've got an update on an image/video
            loggingManager.UpdateCurrentGpsPosition("123", "456");
            var message = CreateTestMessage();

            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);
            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);
            UpdateMessage(loggingManager, message, DrawableStatus.Disposing);

            // Get our internal data right quick, that WILL be logged in the future.
            var storedData = loggingManager.GetElementData(123).GetLogItems()[0];

            // A new layout is about to happen, log our current data.
            //loggingManager.StartNewLayoutSession("TestUnit"); // Triggers finalizing and writing the previous session info out to disk.

            // Sorry for the lengthy stuff...just verifying that we are writing out all the values we expect.
            Assert.AreEqual(1, logEntries.Count);
            var logEntry = logEntries.First();
            Assert.AreEqual("123", logEntry.ResourceId);
            Assert.AreEqual("TestFile.jpg", logEntry.FileName);
            //Assert.AreEqual(30, logEntry.Duration);
            Assert.AreEqual("TestUnit", logEntry.UnitName);
            Assert.AreEqual("123", logEntry.StartedLatitude);
            Assert.AreEqual("123", logEntry.StoppedLatitude);
            Assert.AreEqual("456", logEntry.StartedLongitude);
            Assert.AreEqual("456", logEntry.StoppedLongitude);
            Assert.AreEqual("Bus1", logEntry.VehicleId);
            Assert.AreEqual("Route66", logEntry.Route);

            // Check against our internal stored data.
            Assert.AreEqual(storedData.ResourceId, logEntry.ResourceId);
            Assert.AreEqual(storedData.FileName, logEntry.FileName);
            //Assert.AreEqual(storedData.Duration, logEntry.Duration, "The written record should have a duration of 30 seconds.");
            Assert.AreEqual(storedData.PlayStarted, logEntry.PlayStarted);
            var stringResult = storedData.PlayStarted.ToString();
            Assert.IsNotNull(logEntry.PlayStopped);
            Assert.AreEqual(storedData.VehicleId, logEntry.VehicleId);
            Assert.AreEqual(storedData.UnitName, logEntry.UnitName);
        }

        [TestMethod]
        public void StartingAndStoppingGPSCoordinates_WillDiffer_IfUnitLocationChangesOverTime()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            var message = CreateTestMessage();
            
            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);

            loggingManager.SetVehicleId("Bus1");
            loggingManager.SetRoute("Route66");

            // We've got an update on an image/video
            loggingManager.UpdateCurrentGpsPosition("123", "456"); // These should be in as the initial coordinates.
            
            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);

            loggingManager.UpdateCurrentGpsPosition("234", "567"); // Now they have changes. When we write out the entry, it should use these.

            UpdateMessage(loggingManager, message, DrawableStatus.Disposing);

            var lognEntry = logEntries.First();

            Assert.AreEqual("123", lognEntry.StartedLatitude);
            Assert.AreEqual("456", lognEntry.StartedLongitude);
            Assert.AreEqual("234", lognEntry.StoppedLatitude);
            Assert.AreEqual("567", lognEntry.StoppedLongitude);
        }

        [TestMethod]
        public void MultipleWrites_ShouldWork_Multithreaded()
        {
            // If the ProofOfPlayLoggingManager doesn't have locks on accessing the data, you will get intermittent exceptions 
            // so this is a little tricky to test. It comes down to timing, and things clearing the unit session data while other threads
            // are trying to access it. I'd like to find a cleaner way to test this.
            var mockLogger = CreateMockLoggerSetupToLogEntries();

            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
            loggingManager.Start(@"D:\PresentationPlayLogs\Multiple.csv", @"D:\PresentationPlayLogs", FileNameRolloverType.Numerical, 0, 10);
            List<Task> task = new List<Task>
                                  {
                                      this.WriteEntries(20, "Task1", loggingManager),
                                      this.WriteEntries(20, "Task2", loggingManager)
                                  };

            Task.WaitAll(task.ToArray());
            loggingManager.Stop();

            loggingManager.Dispose();
            Assert.IsTrue(logEntries.Count > 0); // This test is just verifying that the locking is working. Don't expect to get 40 total entries 
            // writter in the written entries count, it won't happen because of the way WriteEntries works. 
            // We shouldn't normally have a case where one thread is starting a layout session for the unit, then another comes along and
            // does the same thing which would erase the previous thread's data. Remember starting a new session layout will case any previous session
            // dat for that unit to write and clear data.
        }

        private Task WriteEntries(int count, string name, ProofOfPlayLoggingManager<InfotransitPresentationInfo> loggingManager)
        {
            return Task.Run(
                () =>
                    {
                        for (int j = 0; j < count; j++)
                        {
                            UpdateMessage(loggingManager, CreateTestMessage(), DrawableStatus.Initialized);

                            loggingManager.SetVehicleId("Bus1-" + name);
                            loggingManager.SetRoute("Route66");

                            // We've got an update on an image/video
                            loggingManager.UpdateCurrentGpsPosition("123", "456"); // These should be in as the initial coordinates.
                            UpdateMessage(loggingManager, CreateTestMessage(), DrawableStatus.Rendering);

                            loggingManager.UpdateCurrentGpsPosition("234", "567"); // Now they have changes. When we write out the entry, it should use these.
                            UpdateMessage(loggingManager, CreateTestMessage(), DrawableStatus.Disposing);
                        }
                    });
        }

        [TestMethod]
        public void WriteAsync_WillNotWrite_InvalidInfoEntries()
        {
            var mockLogger = CreateMockLogger();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
            var message = CreateTestMessage(elementFile: string.Empty);

            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);
            loggingManager.SetRoute("TestRoute");

            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);

            UpdateMessage(loggingManager, message, DrawableStatus.Disposing);

            mockLogger.Verify(m => m.WriteAsync(It.IsAny<InfotransitPresentationInfo>()), Times.Never);
        }

        [TestMethod]
        public void WriteAsync_WillWrite_ValidEntries()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
            var validMessage = CreateTestMessage();
            
            // This message will not get written, the element update doesn't provide a valid file name.
            var invalidMessage = CreateTestMessage(elementFile: string.Empty, elementId: 456);
            
            UpdateMessage(loggingManager, validMessage, DrawableStatus.Initialized);
            UpdateMessage(loggingManager, invalidMessage, DrawableStatus.Initialized);
            
            // Valid = must have a valid route, vehicle, unit and file name.
            loggingManager.SetRoute("TestRoute");
            loggingManager.SetVehicleId("Green Bus");
            
            UpdateMessage(loggingManager, validMessage, DrawableStatus.Rendering);
            UpdateMessage(loggingManager, invalidMessage, DrawableStatus.Rendering);

            loggingManager.SetRoute("TestRoute");
            loggingManager.SetVehicleId("Red Bus");

            UpdateMessage(loggingManager, validMessage, DrawableStatus.Disposing);
            UpdateMessage(loggingManager, invalidMessage, DrawableStatus.Disposing);
            
            Assert.AreEqual(1, logEntries.Count);
            Assert.AreEqual("TestFile.jpg", logEntries.First().FileName);
        }

        /// <summary>
        /// If the different elements are shown on multiple units, we will write two different log entries.
        /// </summary>
        [TestMethod]
        public void DifferentElement_DifferentUnites_WillBeTracked_Seperately()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
            var messageA = CreateTestMessage("UnitA");
            var messageB = CreateTestMessage("UnitB", elementId: 456);
            
            UpdateMessage(loggingManager, messageA, DrawableStatus.Initialized);
            UpdateMessage(loggingManager, messageB, DrawableStatus.Initialized);

            loggingManager.SetVehicleId("Bus1");
            loggingManager.SetRoute("Route66");

            UpdateMessage(loggingManager, messageA, DrawableStatus.Rendering);
            UpdateMessage(loggingManager, messageB, DrawableStatus.Rendering);

            UpdateMessage(loggingManager, messageA, DrawableStatus.Disposing);
            Assert.AreEqual(1, logEntries.Count); // Did we write an entry out?
            Assert.AreEqual("123", logEntries.First().ResourceId); // Just a quick check against the resource id.
            
            UpdateMessage(loggingManager, messageB, DrawableStatus.Disposing);

            Assert.AreEqual(2, logEntries.Count); // Did we write an entry out?
            Assert.AreEqual("456", logEntries[1].ResourceId); // Just a quick check against the resource id.
        }

        /// <summary>
        /// If the same elements are shown on multiple units, we will write two different log entries.
        /// </summary>
        [TestMethod]
        public void SameElement_DifferentUnites_WillBeTracked_Seperately()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            // Remember, initialized and disposed are signals from composers.generated.cs. We should only get one for all elements of the same ID from composer.
            // We get multiple render notifications because those happen on different units.
            var messageA = CreateTestMessage("UnitA"); 

            UpdateMessage(loggingManager, messageA, DrawableStatus.Initialized);

            loggingManager.SetVehicleId("Bus1");
            loggingManager.SetRoute("Route66");

            UpdateMessage(loggingManager, messageA, DrawableStatus.Rendering);

            var messageB = CreateTestMessage("UnitB");
            UpdateMessage(loggingManager, messageB, DrawableStatus.Rendering); // Unit B reports it has started rendering the message.
            
            // Composer says it's time to ditch the message.
            UpdateMessage(loggingManager, messageA, DrawableStatus.Disposing);
            
            Assert.AreEqual(2, logEntries.Count); // We should have two log entries then.
            Assert.AreEqual("123", logEntries[0].ResourceId);
            Assert.AreEqual("UnitA", logEntries[0].UnitName);
            
            Assert.AreEqual("123", logEntries[1].ResourceId);
            Assert.AreEqual("UnitB", logEntries[1].UnitName);
        }

        [TestMethod]
        public void Dispose_Should_Dispose_OurLogger()
        {
            var mockLogger = CreateMockLogger();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            loggingManager.Dispose();

            mockLogger.Verify(m => m.Dispose());
        }

        [TestMethod]
        public void Start_ShouldStartTheLogger_WithCorrectParameters()
        {
            var mockLogger = CreateMockLogger();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            loggingManager.Start("logfile.csv", @"D:\rolloverFolder", FileNameRolloverType.Numerical, 500, 100);

            mockLogger.Verify(m => m.Start("logfile.csv", @"D:\rolloverFolder", FileNameRolloverType.Numerical, 500, 100));
        }

        [TestMethod]
        public void Stop_ShouldStop_Logger()
        {
            var mockLogger = CreateMockLogger();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            loggingManager.Stop();

            mockLogger.Verify(m => m.Stop());
        }

        [TestMethod]
        public void ElementInitialized_ShouldRemovePreviousElementData_ForUnits()
        {
            var mockLogger = CreateMockLogger();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
            var message = CreateTestMessage();

            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);

            // This will data for tracking.
            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);
            Assert.AreEqual(1, loggingManager.GetElementData(message.ElementID).GetLogItems().Count);
            
            // This should clear that data.
            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);
            
            Assert.AreEqual(0, loggingManager.GetElementData(message.ElementID).GetLogItems().Count);
        }

        [TestMethod]
        public void WriteSessionData_WillUse_CurrentEnvironmentGPSValues_ForStoppedLatAndLong()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();

            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            var message = CreateTestMessage();
            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);
            
            loggingManager.UpdateCurrentGpsPosition("1.23", "4.56");
            loggingManager.SetRoute("Blue Route");
            loggingManager.SetVehicleId("Blue Bus");
            
            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);
            
            Assert.AreEqual(1, loggingManager.GetElementData(message.ElementID).GetLogItems().Count);
            
            UpdateMessage(loggingManager, message, DrawableStatus.Disposing);

            mockLogger.Verify(m => m.WriteAsync(It.IsAny<InfotransitPresentationInfo>()), Times.Once);
            var writtenInfo = logEntries.First();

            Assert.AreEqual("1.23", writtenInfo.StoppedLatitude);
            Assert.AreEqual("4.56", writtenInfo.StoppedLongitude);
        }

        //[TestMethod]
        //public void WritingLogEntries_WillCalculate_PlayDuration_UsingVideoEvents()
        //{
        //    var mockLogger = CreateMockLoggerSetupToLogEntries();
        //    var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

        //    loggingManager.StartNewLayoutSession("TestUnit");
        //    loggingManager.UpdateResource(123, string.Empty, "TestUnit", "LumMovie.mp4", "0", TimeSpan.FromSeconds(10), new VideoItem());

        //    var currentTime = DateTime.Now;
        //    loggingManager.HandleVideoPlaybackEvent("TestUnit", "LumMovie.mp4", true, 123, currentTime);
        //    loggingManager.HandleVideoPlaybackEvent("TestUnit", "LumMovie.mp4", false, 123, currentTime.AddSeconds(10));

        //    loggingManager.Stop();
        //    var logEntry = logEntries.First();
        //    Assert.IsTrue(logEntry.PlayedDuration >= 0 && logEntry.PlayedDuration <= 11, "Expected the play duration to be around 10 seconds.");
        //}

        [TestMethod]
        public void WritingLogEntries_WillCalculate_PlayDuration_IfStopped()
        {
            var mockLogger = CreateMockLoggerSetupToLogEntries();
            var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);

            var message = CreateTestMessage();

            UpdateMessage(loggingManager, message, DrawableStatus.Initialized);
            UpdateMessage(loggingManager, message, DrawableStatus.Rendering);
            
            Thread.Sleep(3000);

            UpdateMessage(loggingManager, message, DrawableStatus.Disposing);
            loggingManager.Stop();

            // We only played for about 5 seconds, make sure if we just call Stop() that it calculates the time correctly.
            Assert.AreEqual(3, logEntries.First().PlayedDuration, 1, "Expected the played duration to be around 1 seconds.");
        }

        //[TestMethod]
        //public void IsPlayInterrupted_ShouldBeTrue_IfPlayedDurationIsMuchLessThanExpected()
        //{
        //    var mockLogger = CreateMockLoggerSetupToLogEntries();
        //    var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
        //    loggingManager.StartNewLayoutSession("TestUnit");
        //    var testDuration = TimeSpan.FromSeconds(10);
        //    loggingManager.UpdateResource(123, string.Empty, "TestUnit", "LumMovie.mp4", "0", testDuration, new VideoItem());

        //    // Say we started playing 5 seconds ago.
        //    var currentTime = DateTime.Now.AddSeconds(-5);
        //    loggingManager.HandleVideoPlaybackEvent("TestUnit", "LumMovie.mp4", true, 123, currentTime);

        //    loggingManager.Stop();

        //    Assert.IsTrue(logEntries.First().IsPlayInterrupted, "Video only played 5 out of 10 seconds, IsPlayInterrupted should be true.");
        //}

        //[TestMethod]
        //public void IsPlayInterrupted_ShouldBeFalse_IfPlayedDurationIsAboutExpected()
        //{
        //    var mockLogger = CreateMockLoggerSetupToLogEntries();
        //    var loggingManager = this.CreateProofOfPlayLoggingManager(mockLogger.Object);
        //    loggingManager.StartNewLayoutSession("TestUnit");
        //    var testDuration = TimeSpan.FromSeconds(10);
        //    loggingManager.UpdateResource(123, string.Empty, "TestUnit", "LumMovie.mp4", "0", testDuration, new VideoItem());

        //    // Say we started playing 10 seconds ago.
        //    var currentTime = DateTime.Now.AddSeconds(-10);
        //    loggingManager.HandleVideoPlaybackEvent("TestUnit", "LumMovie.mp4", true, 123, currentTime);

        //    loggingManager.Stop();

        //    Assert.IsFalse(logEntries.First().IsPlayInterrupted, "Video only played about 10 out of 10 seconds, IsPlayInterrupted should be false.");
        //}

        private static DrawableComposerInitMessage CreateTestMessage(string unit = "TestUnit", string elementFile = "TestFile.jpg", int elementId = 123)
        {
            return new DrawableComposerInitMessage
                       {
                           UnitName = unit,
                           ElementFileName = elementFile,
                           ElementID = elementId
                       };
        }

        private ProofOfPlayLoggingManager<InfotransitPresentationInfo> CreateProofOfPlayLoggingManager(ICsvLogging<InfotransitPresentationInfo> mockLogger)
        {
            return new ProofOfPlayLoggingManager<InfotransitPresentationInfo>(mockLogger);
        }

        private static void UpdateMessage(ProofOfPlayLoggingManager<InfotransitPresentationInfo> loggingManager, DrawableComposerInitMessage message, DrawableStatus status)
        {
            message.Status = status;
            loggingManager.UpdateDrawableComposer(message);
        }
    }
}