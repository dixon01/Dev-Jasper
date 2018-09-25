// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameEncoderTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameEncoderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Tests
{
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Frames;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="FrameEncoder"/>.
    /// </summary>
    [TestClass]
    public class FrameEncoderTest
    {
        /// <summary>
        /// Test for encoding a <see cref="StatusRequestFrame"/>.
        /// </summary>
        [TestMethod]
        public void EncodeStatusRequest()
        {
            var target = new FrameEncoder(false);
            var frame = new StatusRequestFrame { Address = 5 };

            var data = target.Encode(frame);
            Assert.IsNotNull(data);
            Assert.AreEqual(4, data.Length);
            Assert.AreEqual(0x7E, data[0]);
            Assert.AreEqual(0x85, data[1]);
            Assert.AreEqual(0x7A, data[2]); // 0xFF xor 0x85
            Assert.AreEqual(0x7E, data[3]);
        }

        /// <summary>
        /// Test for encoding a <see cref="StatusResponseFrame"/>.
        /// </summary>
        [TestMethod]
        public void EncodeStatusResponse()
        {
            // example taken from AHDLC protocol definition
            // 7E 02 48 4C 45 44 36 34 33 31 35 30 76 30 30 30 01 80 00 34 80 01 8F 7E
            var payload = new byte[]
                              {
                                  0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30, 0x30,
                                  0x01, 0x80, 0x00, 0x34, 0x80, 0x01
                              };
            var target = new FrameEncoder(false);
            var frame = new StatusResponseFrame(payload) { Address = 2 };

            var data = target.Encode(frame);
            Assert.IsNotNull(data);
            Assert.AreEqual(24, data.Length);
            var expected = new byte[]
                               {
                                   0x7E, 0x02, 0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30,
                                   0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x8F, 0x7E
                               };
            CollectionAssert.AreEqual(expected, data);
        }

        /// <summary>
        /// Test for encoding a <see cref="OutputCommandFrame"/>.
        /// </summary>
        [TestMethod]
        public void EncodeOutputCommand()
        {
            var content = new byte[]
                              {
                                  0x15, 0x17, 0x38, 0x22, 0x87, 0xC3
                              };
            var target = new FrameEncoder(false);
            var frame = new OutputCommandFrame { Address = 4, BlockNumber = 1, Data = content };

            var data = target.Encode(frame);
            Assert.IsNotNull(data);
            Assert.AreEqual(11, data.Length);
            var expected = new byte[]
                               {
                                   0x7E, 0xA4, 0x01, 0x15, 0x17, 0x38, 0x22, 0x87, 0xC3, 0x06, 0x7E
                               };
            CollectionAssert.AreEqual(expected, data);
        }

        /// <summary>
        /// Test for encoding a <see cref="OutputCommandFrame"/> in high-speed mode.
        /// </summary>
        [TestMethod]
        public void EncodeOutputCommandHighSpeed()
        {
            var content = new byte[]
                              {
                                  0x15, 0x17, 0x38, 0x22, 0x87, 0xC3
                              };
            var target = new FrameEncoder(true);
            var frame = new OutputCommandFrame { Address = 4, BlockNumber = 1, Data = content };

            var data = target.Encode(frame);
            Assert.IsNotNull(data);
            Assert.AreEqual(12, data.Length);
            var expected = new byte[]
                               {
                                   0x7E, 0xA4, 0x00, 0x01, 0x15, 0x17, 0x38, 0x22, 0x87, 0xC3, 0x06, 0x7E
                               };
            CollectionAssert.AreEqual(expected, data);
        }
    }
}
