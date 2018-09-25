// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciSerializerTests.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Tests.Serialization
{
    using System;
    using Eci.Serialization;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The eci serializer test.
    /// </summary>
    [TestClass]
    public class EciSerializerTests
    {
        /// <summary>
        /// The test position message.
        /// </summary>
        [TestMethod]
        public void TestPositionMessage()
        {
            EciPositionMessage msg1 = new EciPositionMessage
            {
                VehicleId = 5423,
                AlarmState = EciAlarmState.Free,
                StopId = 6474,
                PositionEvent = PositionEvent.Gps,
                Latitude = 45.24,
                Longitude = 7.65,
                LineId = 8354,
                GpsNumberSats = 5,
                BlockId = 987364,
                TripId = 7654876,
                SpeedKmS = 54,
                Direction = 54,
                TimeStamp = DateTime.Now,
                AlarmId = 14
            };
            var packet1 = EciSerializer.CreatePacket(msg1);
            Assert.IsTrue(true);
        }
    }
}
