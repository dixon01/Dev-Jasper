// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutSessionDataTests.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutSessionDataTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationLogging.UnitTest.PresentationPlayDataTracking
{
    using System;

    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The layout session data tests.
    /// </summary>
    [TestClass]
    public class LayoutSessionDataTests
    {
        [TestMethod]
        public void UpdateItems_WillUseInfoItemGPSValues_IfNoGPSLocations_HaveBeenReportedFromLAM()
        {
            var currentEnvironment = new CurrentEnvironmentInfo();
            LayoutSessionData<InfotransitPresentationInfo> sessionData = new LayoutSessionData<InfotransitPresentationInfo>(currentEnvironment);

            var info = this.CreateFullInfotransitPresentationInfo();

            sessionData.UpdateItems(info);

            var result = sessionData.InfoItems[0];

            Assert.AreEqual(info.StartedLatitude, result.StartedLatitude);
            Assert.AreEqual(info.StartedLongitude, result.StartedLongitude);
        }

        [TestMethod]
        public void UpdateVideoPlayback_WillUpdatedStartedValues_IfPlaying()
        {
            // This test verifies that when we process an update to video playback,
            // it updates the video stats with the current environment's longitude/latitude values.
            var currentEnvironment = new CurrentEnvironmentInfo();
            LayoutSessionData<InfotransitPresentationInfo> sessionData = new LayoutSessionData<InfotransitPresentationInfo>(currentEnvironment);

            var info = this.CreateFullInfotransitPresentationInfo();
            sessionData.UpdateItems(info);

            var item = sessionData.InfoItems[0];
            
            // Just clear the data
            item.PlayStarted = null;
            sessionData.CurrentEnvironment.Longitude = "4.56";
            sessionData.CurrentEnvironment.Latitude = "7.89";
            
            sessionData.UpdateVideoPlayback("TestFile.jpg", true, DateTime.Now);
            
            Assert.AreEqual("4.56", item.StartedLongitude);
            Assert.AreEqual("7.89", item.StartedLatitude);
            Assert.IsNotNull(item.PlayStarted);
        }

        [TestMethod]
        public void UpdateVideoPlayback_WillUpdatedStopValues_IfNoLongerPlaying()
        {
            var currentEnvironment = new CurrentEnvironmentInfo();
            LayoutSessionData<InfotransitPresentationInfo> sessionData = new LayoutSessionData<InfotransitPresentationInfo>(currentEnvironment);

            var info = this.CreateFullInfotransitPresentationInfo();
            sessionData.UpdateItems(info);

            var item = sessionData.InfoItems[0];

            // Just clear the data
            item.PlayStopped = null;
            sessionData.CurrentEnvironment.Longitude = "4.56";
            sessionData.CurrentEnvironment.Latitude = "7.89";

            sessionData.UpdateVideoPlayback("TestFile.jpg", false, DateTime.Now);

            Assert.AreEqual("4.56", item.StoppedLongitude);
            Assert.AreEqual("7.89", item.StoppedLatitude);
            Assert.IsNotNull(item.PlayStopped);
            
            // Make sure it didn't also change the starting values.
            Assert.AreEqual(info.StartedLongitude, item.StartedLongitude);
            Assert.AreEqual(info.StartedLatitude, item.StartedLatitude);
            Assert.IsNotNull(item.PlayStarted);
        }

        private InfotransitPresentationInfo CreateFullInfotransitPresentationInfo()
        {
            var info = new InfotransitPresentationInfo
                           {
                               ResourceId = "123",
                               FileName = "TestFile.jpg",
                               Route = "Hawkins Route",
                               StartedLatitude = "1.23",
                               StartedLongitude = "1.45",
                               VehicleId = "Blue Bus",
                               Duration = 15,
                               UnitName = "TestUnit"
                           };

            return info;
        }
    }
}