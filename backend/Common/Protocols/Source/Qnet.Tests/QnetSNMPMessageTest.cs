// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetSNMPMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;

    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QnetSNMPMessageTest and is intended
    /// to contain all QnetSNMPMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetSNMPMessageTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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
        #region Public Methods and Operators

        /// <summary>
        /// A test for QnetSNMPMessage Constructor
        /// </summary>
        [TestMethod]
        public void QnetSNMPMessageConstructorTest()
        {
            var target = new QnetSNMPMessage();
            Assert.IsNotNull(target.QSNMP, "The QSNMP has to be instanciated into the constructor");
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for QnetSNMPMessage Constructor
        /// </summary>
        [TestMethod]
        public void QnetSNMPMessageConstructorTest_with_source_and_dest_addresses()
        {
            const ushort SourceAddress = 0; // TODO: Initialize to an appropriate value
            const ushort DestAddress = 0; // TODO: Initialize to an appropriate value
            const ushort GatewayAddress = 0; // TODO: Initialize to an appropriate value
            var target = new QnetSNMPMessage(SourceAddress, DestAddress, GatewayAddress);
            Assert.IsNotNull(target.QSNMP, "The QSNMP has to be instanciated into the constructor");
        }

        /// <summary>
        /// A test for QnetSNMPMessage.RespondTimeSync method
        /// </summary>
        [TestMethod]

        public void QnetSNMPMessage_RespondTimeSyncTest()
        {
            const short CMD_TIME_SYNC_RESPONSE = 5;
            var target = new QnetSNMPMessage();
            DateTime originateTime = DateTime.Now.AddSeconds(-2);
            DateTime receiveTime = originateTime.AddSeconds(1).AddHours(1).AddMinutes(1);

            target.RespondTimeSync(receiveTime, originateTime);

            Assert.AreEqual(target.QSNMP.hdr.cmd, CMD_TIME_SYNC_RESPONSE);

            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.ReceiveTime.Hour,
                receiveTime.Hour, 
                "The receive hour of the request must equals to sync hour");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.ReceiveTime.Minute,
                receiveTime.Minute, 
                "The receive min of the request must equals to sync min");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.ReceiveTime.Second,
                receiveTime.Second, 
                "The receive sec of the request must equals to sync sec");

            // Todo with a DateTime provider, test the transmit
            /*
             * 
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitTime.Hour, 
                receiveTime.Hour, 
                "The transmit hour of the request must equals to receive hour");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitTime.Minute, 
                receiveTime.Minute, 
                "The transmit min of the request must equals to receive min");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitTime.Second, 
                receiveTime.Second, 
                "The transmit sec of the request must equals to receive sec");

            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitDate.Day, 
                receiveTime.Day, 
                "The transmit day of the request must equals to receive day");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitDate.Month, 
                receiveTime.Month, 
                "The transmit month of the request must equals to receive month");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.TransmitDate.Year, 
                receiveTime.Year, 
                "The transmit year of the request must equals to receive year");
            */
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.OriginateTime.Hour, 
                originateTime.Hour, 
                "The originateTime hour of the request must equals to OriginateTime hour of the request");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.OriginateTime.Minute, 
                originateTime.Minute, 
                "The originateTime min of the request must equals to OriginateTime min  of the request");
            Assert.AreEqual(
                target.QSNMP.msg.timeSync.qntp.OriginateTime.Second, 
                originateTime.Second, 
                "The originateTime sec of the request must equals to OriginateTime sec  of the request");
        }

        #endregion
        // ReSharper restore InconsistentNaming
    }
}