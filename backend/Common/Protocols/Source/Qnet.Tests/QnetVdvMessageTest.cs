// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetVdvMessageTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System;

    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for QnetVdvMessageTest and is intended
    /// to contain all QnetVdvMessageTest Unit Tests
    /// </summary>
    [TestClass]
    public class QnetVdvMessageTest
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
        /// A simplest test for SetSpecialLineTextDeletion
        /// </summary>
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void
            SetSpecialLineTextDeletion_CreateQnetVdvMessageAndSetSpecialLineTextDeletion_SetsSpecialLineTextDeletion()
        {
            // ReSharper restore InconsistentNaming
            var target = new QnetVdvMessage();
            const ushort ExpectedItcsId = 1;
            const uint ExpectedLineNumber = 2;
            const ushort ExpectedDestinationId = 3;

            target.SetSpecialLineTextDeletion(ExpectedItcsId, ExpectedLineNumber, ExpectedDestinationId);
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineTextDeletion.ITCSId, ExpectedItcsId, "ItcsId not assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineTextDeletion.LineNumber, 
                ExpectedLineNumber, 
                "LineNumber not assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineTextDeletion.DestinationId, 
                ExpectedDestinationId, 
                "ItcDirectionIdsId not assigned.");
        }

        /// <summary>
        /// A simplest test for SetSpecialLineText
        /// </summary>
        [TestMethod]
        public void SetSpecialLineTextTest()
        {
            var target = new QnetVdvMessage();
            const ushort ExpectedItcsId = 1;
            const uint ExpectedLineNumber = 2;
            const ushort ExpectedDestinationId = 3;
            DateTime expectedExpirationTime = DateTime.Now;
            const string ExpectedDisplayMessage = "This is a test";

            target.SetSpecialLineText(
                ExpectedItcsId, 
                ExpectedLineNumber, 
                ExpectedDestinationId, 
                expectedExpirationTime, 
                ExpectedDisplayMessage);
            Assert.AreEqual(target.VdvMessage.Dta.VdvMsg.SpecialLineText.ITCSId, ExpectedItcsId, "ItcsId not assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.LineNumber, ExpectedLineNumber, "LineNumber not assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.DestinationId, 
                ExpectedDestinationId, 
                "ItcDirectionIdsId not assigned.");

            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Day, 
                expectedExpirationTime.Day, 
                "The day component of the part date of ExpirationTime is not well assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Month, 
                expectedExpirationTime.Month, 
                "The Month component of the part date of ExpirationTime is not well assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Year, 
                expectedExpirationTime.Year, 
                "The Year component of the date part of ExpirationTime is not well assigned.");

            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Hour, 
                expectedExpirationTime.Hour, 
                "The hour component of part time of ExpirationTime is not well assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Minute, 
                expectedExpirationTime.Minute, 
                "The Minute component of part time of ExpirationTime is not well assigned.");
            Assert.AreEqual(
                target.VdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Second, 
                expectedExpirationTime.Second, 
                "The Second component of part time ExpirationTime is not well assigned.");

            string actualTargetText = target.VdvMessage.Dta.VdvMsg.SpecialLineText.DisplayText;
            Assert.AreEqual(ExpectedDisplayMessage, actualTargetText, "The display message is not assigned.");
        }

        #endregion
    }
}