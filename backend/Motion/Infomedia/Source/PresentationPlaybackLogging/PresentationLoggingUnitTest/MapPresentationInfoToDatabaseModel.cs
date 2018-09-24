using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Luminator.PresentationLogging.UnitTest
{
    using AutoMapper;

    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Core.Models;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    [TestClass]
    public class MapPresentationInfoToDatabaseModel
    {
        [TestMethod]
        public void AutoMapPresentationInfo()
        {

            //var cfg = cfg => cfg.CreateMap<InfotransitPresentationInfo, PresentationLogItem>();
            var m = new InfotransitPresentationInfo()
                        {
                            Duration = 10,
                            PlayedDuration = 9,
                            FileName = "test.jpg",
                            Created = DateTime.Now,
                            PlayStarted = DateTime.Now,
                            PlayStopped = DateTime.Now.AddSeconds(10),
                            Route = "123",
                            IsPlayInterrupted = false,
                            StartedLatitude = "StartLat",
                            StartedLongitude = "StartLon",
                            StoppedLatitude = "StartLat",
                            StoppedLongitude = "StopLon",
                            VehicleId = "BUS7",  
                            ResourceId = "RESOURCE123",
                            Trip = "Trip1234"
                        };
            Mapper.Initialize(cfg => cfg.CreateMap<InfotransitPresentationInfo, PresentationPlayLoggingItem>());

            var logItem = Mapper.Map<PresentationPlayLoggingItem>(m);
            Assert.AreEqual(m.FileName, logItem.FileName);
            Assert.AreEqual(m.Duration, logItem.Duration);
            Assert.AreEqual(m.PlayedDuration, logItem.PlayedDuration);
            Assert.AreEqual(m.Created, logItem.Created);
            Assert.AreEqual(m.PlayStarted, logItem.PlayStarted);
            Assert.AreEqual(m.PlayStopped, logItem.PlayStopped);
            Assert.AreEqual(m.IsPlayInterrupted, logItem.IsPlayInterrupted);
            Assert.AreEqual(m.StartedLatitude, logItem.StartedLatitude);
            Assert.AreEqual(m.StoppedLatitude, logItem.StoppedLatitude);
            Assert.AreEqual(m.StoppedLatitude, logItem.StoppedLatitude);
            Assert.AreEqual(m.StoppedLongitude, logItem.StoppedLongitude);
            Assert.AreEqual(m.VehicleId, logItem.VehicleId);
            Assert.AreEqual(m.ResourceId, logItem.ResourceId);
            Assert.AreEqual(m.Trip, logItem.Trip);

            //Mapper.Configuration.AssertConfigurationIsValid();

        }
    }
}
