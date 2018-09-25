// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciBinaryPacketTests.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EciBinaryPacketTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Tests.Serialization
{
    using System;
    using Eci.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The eci binary packet test.
    /// </summary>
    [TestClass]
    public class EciBinaryPacketTests
    {
        /// <summary>
        /// The test the parity checksum.
        /// </summary>
        [TestMethod]
        public void TestChecksum()
        {
            byte[] buffer = { 0x07, 0x12, 0x53, 0xa3, 0x60, 0x91 };
            Assert.AreEqual(EciBinaryPacket.ComputeChecksum(buffer, 0, 2), 0x46);
            Assert.AreEqual(EciBinaryPacket.ComputeChecksum(buffer, 3, 5), 0x52);
            Assert.AreEqual(EciBinaryPacket.ComputeChecksum(buffer, 0, 5), 0x14);
            Assert.AreEqual(EciBinaryPacket.ComputeChecksum(buffer, 1, 2), 0x41);
            Assert.AreEqual(EciBinaryPacket.ComputeChecksum(buffer, 0, 0), 0x07);
        }

        /// <summary>
        /// Test integer serialization.
        /// </summary>
        [TestMethod]
        public void TestUShort()
        {
            // This array must be in little endian
            byte[][] intBytes =
            {
                new byte[] { 0x00, 0x00 },
                new byte[] { 0x00, 0x00 },
                new byte[] { 0x00, 0x00 },
                new byte[] { 0x00, 0x00 }
            };
            ushort[] values = { 0, 0, 0, 0 };

            // Testing serialization
            var packet1 = new EciBinaryPacket(values.Length * sizeof(ushort));
            foreach (var v in values)
            {
                packet1.AppendUShort(v);
            }

            var packet2 = new EciBinaryPacket(packet1);
            foreach (var ba in intBytes)
            {
                CollectionAssert.AreEqual(packet2.ParseBytes(sizeof(ushort)), ba);
            }

            // Testing deserialization
            packet1 = new EciBinaryPacket(values.Length * sizeof(ushort));
            foreach (var ba in intBytes)
            {
                packet1.AppendBytes(ba);
            }

            packet2 = new EciBinaryPacket(packet1);
            foreach (var v in values)
            {
                Assert.AreEqual(packet2.ParseUShort(), v);
            }
        }

        /// <summary>
        /// Test integer serialization.
        /// </summary>
        [TestMethod]
        public void TestInt()
        {
            // This array must be in little endian
            byte[][] intBytes =
            {
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 }
            };
            int[] values = { 0, 0, 0, 0 };

            // Testing serialization
            var packet1 = new EciBinaryPacket(values.Length * sizeof(int));
            foreach (var v in values)
            {
                packet1.AppendInt(v);
            }

            var packet2 = new EciBinaryPacket(packet1);
            foreach (var ba in intBytes)
            {
                CollectionAssert.AreEqual(packet2.ParseBytes(sizeof(int)), ba);
            }

            // Testing deserialization
            packet1 = new EciBinaryPacket(values.Length * sizeof(int));
            foreach (var ba in intBytes)
            {
                packet1.AppendBytes(ba);
            }

            packet2 = new EciBinaryPacket(packet1);
            foreach (var v in values)
            {
                Assert.AreEqual(packet2.ParseInt(), v);
            }
        }

        /// <summary>
        /// Test float serialization.
        /// </summary>
        [TestMethod]
        public void TestFloat()
        {
            // This array must be in little endian
            byte[][] floatBytes =
            {
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x00, 0x00, 0x00, 0x00 }
            };
            float[] values = { 0.0f, 0.0f, 0.0f, 0.0f };

            // Testing serialization
            var packet1 = new EciBinaryPacket(values.Length * sizeof(float));
            foreach (var vf in values)
            {
                packet1.AppendFloat(vf);
            }

            var packet2 = new EciBinaryPacket(packet1);
            foreach (var fb in floatBytes)
            {
                CollectionAssert.AreEqual(packet2.ParseBytes(sizeof(float)), fb);
            }

            // Testing deserialization
            packet1 = new EciBinaryPacket(values.Length * sizeof(float));
            foreach (var fb in floatBytes)
            {
                packet1.AppendBytes(fb);
            }

            packet2 = new EciBinaryPacket(packet1);
            foreach (var vf in values)
            {
                Assert.AreEqual(packet2.ParseFloat(), vf);
            }
        }

        /// <summary>
        /// The date time serializer.
        /// </summary>
        [TestMethod]
        public void DateTimeSerializer()
        {
            var dt = new DateTime(2015, 4, 24, 3, 59, 43);
            EciBinaryPacket packet = new EciBinaryPacket(20);
            packet.AppendDateTime(dt);
            Assert.IsTrue(true);
        }
    }
}
