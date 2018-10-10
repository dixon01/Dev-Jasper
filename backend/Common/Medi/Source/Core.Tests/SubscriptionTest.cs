// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for SubscriptionTest and is intended
//   to contain all SubscriptionTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Network;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Subscription = Gorba.Common.Medi.Core.Subscription;

    /// <summary>
    /// This is a test class for SubscriptionTest and is intended
    ///  to contain all SubscriptionTest Unit Tests
    /// </summary>
    [TestClass]
    public class SubscriptionTest
    {
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
        /// A test for CanHandle
        /// </summary>
        [TestMethod]
        public void CanHandleTest()
        {
            var subscription = this.CreateSubscription(new MediAddress("U", "A"));
            Assert.IsTrue(subscription.CanHandle(MediAddress.Broadcast, TypeName.Of<string>()));

            Assert.IsTrue(subscription.CanHandle(new MediAddress("U", MediAddress.Wildcard), TypeName.Of<string>()));

            Assert.IsTrue(subscription.CanHandle(new MediAddress(MediAddress.Wildcard, "A"), TypeName.Of<string>()));

            // different application
            Assert.IsFalse(subscription.CanHandle(new MediAddress("U", "B"), TypeName.Of<string>()));

            // different unit
            Assert.IsFalse(subscription.CanHandle(new MediAddress("B", "A"), TypeName.Of<string>()));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create subscription.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// A new subscription
        /// </returns>
        internal virtual Subscription.Subscription CreateSubscription(MediAddress destination)
        {
            // Does not work because Mock can not access the base class
            /*var mock = new Mock<Subscription>(address);
            mock.Setup(
                s =>
                s.Handle(
                  It.IsAny<MessageDispatcher>(), It.IsAny<MediAddress>(), It.IsAny<MediAddress>(), It.IsAny<object>()));
            return mock.Object;*/
            return new MockSubscription(destination);
        }

        #endregion

        private class MockSubscription : Subscription.Subscription
        {
            public MockSubscription(MediAddress destination)
                : base(destination, TypeName.Of<string>())
            {
            }

            public override void Handle(
                IMessageDispatcherImpl medi,
                ISessionId sourceSessionId,
                MediAddress source,
                MediAddress destination,
                object message)
            {
                throw new NotSupportedException();
            }
        }
    }
}