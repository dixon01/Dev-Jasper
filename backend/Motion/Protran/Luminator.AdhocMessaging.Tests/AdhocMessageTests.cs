using Luminator.AdhocMessaging;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Luminator.AdhocMessaging.Tests
{
    using System.Diagnostics;

    using NLog;

    using Logger = Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger;

    [TestClass]
    public class AdhocMessageTests
    {

        private const string DestinationsApiUrl = "http://swdevapi/";

        private const string MessageApiUrl = "http://gohere/";
        #region Static Fields

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static TestContext testContext;

        #endregion
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            testContext = context;
            var nlogTraceListner = new NLogTraceListener { TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.ThreadId };
            Debug.Listeners.Add(nlogTraceListner);
        }

       
        [TestMethod()]
        public void AdhocManagerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AdhocManagerTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterUnitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UnitExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void VehicleExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterVehicleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllUnitsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllVechiclesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterVehicleAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllVechiclesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllMessagesForUnitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllMessagesForVechicleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllMessagesForUnitOnRouteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllMessagesForVechicleOnRouteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterUnitAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAllUnitsAsyncTest()
        {
            Assert.Fail();
        }
    }
}
