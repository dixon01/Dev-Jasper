// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseStructTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Define the base class used to test the qnet structures
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Tests
{
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Define the base class used to test the qnet structures
    /// </summary>
    public class BaseStructTest
    {
        /// <summary>
        /// The test struct length.
        /// </summary>
        /// <param name="expectedLen">
        /// The expected Len.
        /// </param>
        /// <typeparam name="T">
        /// Stucture to be tested
        /// </typeparam>
        protected void TestStructLength<T>(int expectedLen) where T : new()
        {
            int actualLen = Marshal.SizeOf(new T());

            Assert.AreEqual(expectedLen, actualLen);
        }
    }
}
