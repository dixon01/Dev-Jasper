// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteBusStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for dosDateTimeTest and is intended
//   to contain all dosDateTimeTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using Gorba.Common.Protocols.Qnet;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for dosDateTimeTest and is intended
    /// to contain all dosDateTimeTest Unit Tests
    /// </summary>
    [TestClass]
    public class RemoteBusStructTest : BaseStructTest
    {
        #region Public Properties
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// A test for the size in bytes of BusLineWindowDirectRemoteControlStruct
        /// </summary>
        [TestMethod]
        public void TestTheSizeOfBusLineWindowDirectRemoteControlStruct_Returns107()
        {
            const int BusLineWindowDirectRemoteControlStructSize = 107;
            this.TestStructLength<BusLineWindowDirectRemoteControlStruct>(BusLineWindowDirectRemoteControlStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of RemoteBusStruct
        /// </summary>
        [TestMethod]
        public void TestTheSizeOfRemoteBusStruct_Returns115()
        {
            const int BusLineWindowDirectRemoteControlStructSize = 115;
            this.TestStructLength<RemoteBusStruct>(BusLineWindowDirectRemoteControlStructSize);
        }

        #endregion

        // ReSharper restore InconsistentNaming
    }
}