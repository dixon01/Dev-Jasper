// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeTests.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Tests
{
    using System;
    using Gorba.Common.Utility.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The date time tests.
    /// </summary>
    [TestClass]
    public class DateTimeTests
    {
        /// <summary>
        /// The test unix time conversion.
        /// </summary>
        [TestMethod]
        public void TestUnixTimeConversion()
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            const int UnixStartDifference = 0;
            DateTime evaluatedStart = DateTimeUtility.ConvertFromUnixTimestamp(UnixStartDifference);
            Assert.AreEqual(unixStart, evaluatedStart);
        }
    }
}