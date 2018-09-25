// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlMessageCodecTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for XmlMessageCodecTest and is intended
//   to contain all XmlMessageCodecTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Xml
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Tests.Messages;
    using Gorba.Common.Medi.Core.Transcoder.Xml;
    using Gorba.Common.Medi.Core.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for XmlMessageCodecTest and is intended
    ///  to contain all XmlMessageCodecTest Unit Tests
    /// </summary>
    [TestClass]
    public class XmlMessageCodecTest
    {
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Unit Test code")]
        private const string HelloAbXmlContent = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><MediMessage payloadType=\"Gorba.Common.Medi.Core.Tests.Messages.Hello\"><Source><Unit>U</Unit><App>A</App></Source><Destination><Unit>V</Unit><App>B</App></Destination><Hello xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" /></MediMessage>\0";

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Unit Test code")]
        private const string HelloBaXmlContent = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><MediMessage payloadType=\"Gorba.Common.Medi.Core.Tests.Messages.Hello\"><Source><Unit>V</Unit><App>B</App></Source><Destination><Unit>U</Unit><App>A</App></Destination><Hello xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" /></MediMessage>\0";

        #region Public Properties

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #region Public Methods

        /// <summary>
        /// A test for Decode
        /// </summary>
        [TestMethod]
        public void DecodeTest()
        {
            var target = CreateMessageCodec();

            var bytes = Encoding.UTF8.GetBytes(HelloAbXmlContent);
            var buffer = new MessageBuffer(bytes, 0, bytes.Length);
            var session = new MockTransportSession();
            var actual = target.Decode(buffer, new MessageReadResult(bytes.Length, session, null));

            Assert.AreEqual(new MediAddress("U", "A"), actual.Source);
            Assert.AreEqual(new MediAddress("V", "B"), actual.Destination);
            Assert.IsInstanceOfType(actual.Payload, typeof(Hello));
        }

        /// <summary>
        /// A test for Decode using multiple messages as input
        /// </summary>
        [TestMethod]
        public void DecodeMultipleTest()
        {
            var target = CreateMessageCodec();

            var bytes = Encoding.UTF8.GetBytes(HelloAbXmlContent);
            var buffer = new MessageBuffer(bytes, 0, bytes.Length);

            bytes = Encoding.UTF8.GetBytes(HelloBaXmlContent);
            buffer.Append(new MessageBuffer(bytes, 0, bytes.Length));

            Assert.AreEqual((HelloAbXmlContent.Length * 2) + 4, buffer.Count);

            var session = new MockTransportSession();
            var actual = target.Decode(buffer, new MessageReadResult(bytes.Length, session, null));

            Assert.AreEqual(0, buffer.Offset);
            Assert.AreEqual(HelloAbXmlContent.Length + 2, buffer.Count);

            Assert.AreEqual(new MediAddress("U", "A"), actual.Source);
            Assert.AreEqual(new MediAddress("V", "B"), actual.Destination);

            actual = target.Decode(buffer, new MessageReadResult(bytes.Length, session, null));
            Assert.AreEqual(0, buffer.Offset);
            Assert.AreEqual(0, buffer.Count);

            Assert.AreEqual(new MediAddress("V", "B"), actual.Source);
            Assert.AreEqual(new MediAddress("U", "A"), actual.Destination);
        }

        /// <summary>
        /// A test for Encode
        /// </summary>
        [TestMethod]
        public void EncodeTest()
        {
            var target = CreateMessageCodec();
            var message = new MediMessage
                {
                    Source = new MediAddress("U", "A"),
                    Destination = new MediAddress("V", "B"),
                    Payload = new Hello()
                };

            var provider = target.Encode(message);
            var buffers = provider.GetMessageBuffers(new MockTransportSession(), target.Identification).GetEnumerator();
            Assert.IsTrue(buffers.MoveNext());

            var otherBuffers =
                provider.GetMessageBuffers(new MockTransportSession(), target.Identification).GetEnumerator();
            Assert.IsTrue(otherBuffers.MoveNext());

            CollectionAssert.AreEqual(buffers.Current.Buffer, otherBuffers.Current.Buffer);

            var buffer = buffers.Current;
            Assert.AreEqual(0, buffer.Offset);
            Assert.AreEqual(HelloAbXmlContent.Length + 2, buffer.Count);

            var xml = Encoding.UTF8.GetString(buffer.Buffer, buffer.Offset, buffer.Count);
            Assert.AreEqual(HelloAbXmlContent, xml);

            Assert.IsFalse(buffers.MoveNext());
            Assert.IsFalse(otherBuffers.MoveNext());
        }

        /// <summary>
        /// A test for XmlMessageCodec Constructor
        /// </summary>
        [TestMethod]
        public void XmlMessageCodecConstructorTest()
        {
            var target = new XmlMessageCodec();
            Assert.AreEqual(target.Identification.Name, 'X');
            Assert.AreEqual(target.Identification.Version, 1);
        }

        private static XmlMessageCodec CreateMessageCodec()
        {
            var codec = new XmlMessageCodec();
            codec.Configure(new XmlCodecConfig());
            return codec;
        }

        #endregion

        private class MockTransportSession : ITransportSession, ISessionId
        {
            // disable warning about unused event
#pragma warning disable 67
            public event EventHandler Connected;

            public event EventHandler Disconnected;
#pragma warning restore 67

            public ISessionId SessionId
            {
                get
                {
                    return this;
                }
            }

            public GatewayMode LocalGatewayMode
            {
                get
                {
                    return GatewayMode.None;
                }
            }

            public IFrameController FrameController
            {
                get
                {
                    return null;
                }
            }

            public bool Equals(ISessionId other)
            {
                return ReferenceEquals(this, other);
            }
        }
    }
}