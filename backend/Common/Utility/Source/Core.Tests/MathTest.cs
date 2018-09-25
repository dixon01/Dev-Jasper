// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for MathTest and is intended
//   to contain all MathTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for MathTest and is intended
    /// to contain all MathTest Unit Tests
    /// </summary>
    [TestClass]
    public class MathTest
    {
        /// <summary>
        /// A test for Fibonacci
        /// </summary>
        [TestMethod]
        public void FibonacciTest()
        {
            var n = 0;
            var expected = 0;
            var actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(0) failed");

            expected = 1;
            n = 1;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(1) failed");

            expected = 1;
            n = 2;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(2) failed");

            expected = 2;
            n = 3;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(3) failed");

            expected = 3;
            n = 4;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(4) failed");

            expected = 5;
            n = 5;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(5) failed");

            expected = 8;
            n = 6;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(6) failed");

            expected = 21;
            n = 8;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(8) failed");

            expected = 39088169;
            n = 38;
            actual = Math.Fibonacci(n);
            Assert.AreEqual(expected, actual, "Fibonacci(38) failed");
        }
    }
}
