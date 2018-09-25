// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftpMailStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
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
    public class TftpMailStructTest : BaseStructTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// A test for the size in bytes of TftpMailStructStruct
        /// </summary>
        [TestMethod]
        public void TftpMailStructStructLength_Returns232()
        {
            const int TftpMailStructSize = 232;
            this.TestStructLength<TftpMailStruct>(TftpMailStructSize);
        }

        /// <summary>
        /// A test for the size in bytes of TftpStruct
        /// </summary>
        [TestMethod]
        public void TftpStructLength_Returns233()
        {
            const int TftpStructSize = 233;
            this.TestStructLength<TftpStruct>(TftpStructSize);
        }
        
        // ReSharper restore InconsistentNaming
        #endregion
    }
}