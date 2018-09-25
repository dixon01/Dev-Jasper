// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeExtensionsTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="DateTime"/> extension methods.
    /// </summary>
    [TestClass]
    public sealed class DateTimeExtensionsTest
    {
        /// <summary>
        /// Tests the <see cref="DateTimeExtensions.Truncate"/> extension method.
        /// </summary>
        [TestMethod]
        public void TruncateTest()
        {
            var truncatePrecision = TimeSpan.FromMinutes(1);
            var date = new DateTime(2014, 1, 6, 8, 13, 5, DateTimeKind.Utc);
            var truncated = date.Truncate(truncatePrecision);
            Assert.AreEqual(0, truncated.Second);
            Assert.AreNotEqual(date, truncatePrecision);
        }
    }
}