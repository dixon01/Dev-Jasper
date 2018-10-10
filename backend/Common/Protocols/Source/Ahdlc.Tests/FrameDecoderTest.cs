// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameDecoderTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameDecoderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Tests
{
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Frames;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="FrameDecoder"/>.
    /// </summary>
    [TestClass]
    public class FrameDecoderTest
    {
        /// <summary>
        /// Test for decoding a <see cref="StatusRequestFrame"/>.
        /// </summary>
        [TestMethod]
        public void DecodeStatusRequest()
        {
            var data = new byte[] { 0x7E, 0x85, 0x7A, 0x7E };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as StatusRequestFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(5, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding a <see cref="StatusResponseFrame"/>.
        /// </summary>
        [TestMethod]
        public void DecodeStatusResponse()
        {
            // example taken from AHDLC protocol definition
            // 7E 02 48 4C 45 44 36 34 33 31 35 30 76 30 30 30 01 80 00 34 80 01 8F 7E
            var data = new byte[]
                               {
                                   0x7E, 0x02, 0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30,
                                   0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x8F, 0x7E
                               };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as StatusResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(2, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding a <see cref="SetupResponseFrame"/>.
        /// </summary>
        [TestMethod]
        public void DecodeSetupResponse()
        {
            var data = new byte[] { 0x7E, 0x17, 0x7E };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as SetupResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(7, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding a <see cref="OutputResponseFrame"/>.
        /// </summary>
        [TestMethod]
        public void DecodeOutputResponse()
        {
            var data = new byte[] { 0x7E, 0x2F, 0x7E };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as OutputResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(15, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding multiple messages.
        /// </summary>
        [TestMethod]
        public void DecodeMultiple()
        {
            var data = new byte[]
                           {
                               0x7E, 0x02, 0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30,
                               0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x8F, 0x7E, 0x7E, 0x03, 0x48, 0x4C, 0x45, 0x44,
                               0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80,
                               0x01, 0x8E, 0x7E
                           };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as StatusResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(2, frame.Address);

            Assert.IsTrue(en.MoveNext());
            frame = en.Current as StatusResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(3, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding a message after a partial frame.
        /// </summary>
        [TestMethod]
        public void DecodeAfterPartialFrame()
        {
            var data = new byte[]
                           {
                               0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30,
                               0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x8F, 0x7E, 0x7E, 0x03, 0x48, 0x4C, 0x45, 0x44,
                               0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80,
                               0x01, 0x8E, 0x7E
                           };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as StatusResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(3, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for decoding a message after a bad frame.
        /// </summary>
        [TestMethod]
        public void DecodeAfterBadFrame()
        {
            var data = new byte[]
                           {
                               0x7E, 0x02, 0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30,
                               0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x7E, 0x03, 0x48, 0x4C, 0x45, 0x44,
                               0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30, 0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80,
                               0x01, 0x8E, 0x7E
                           };
            var target = new FrameDecoder(false) { IgnoreFrameStart = true };

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsTrue(en.MoveNext());
            var frame = en.Current as StatusResponseFrame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(3, frame.Address);

            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test for a bad checksum.
        /// </summary>
        [TestMethod]
        public void BadChecksum()
        {
            var data = new byte[]
                               {
                                   0x7E, 0x02, 0x48, 0x4C, 0x45, 0x44, 0x36, 0x34, 0x33, 0x31, 0x35, 0x30, 0x76, 0x30,
                                   0x30, 0x30, 0x01, 0x80, 0x00, 0x34, 0x80, 0x01, 0x87, 0x7E
                               };
            var target = new FrameDecoder(false);

            var frames = target.AddBytes(data);
            var en = frames.GetEnumerator();
            Assert.IsFalse(en.MoveNext());
        }
    }
}