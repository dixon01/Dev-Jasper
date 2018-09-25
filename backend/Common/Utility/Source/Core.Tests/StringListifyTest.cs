// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringListifyTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringListifyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="StringListify"/>.
    /// </summary>
    [TestClass]
    public class StringListifyTest
    {
        /// <summary>
        /// Simple Test
        /// </summary>
        [TestMethod]
        public void SimpleTest()
        {
            Test(@"Hello;World;!", new[] { "Hello", "World", "!" });
        }

        /// <summary>
        /// Test containing an empty element
        /// </summary>
        [TestMethod]
        public void EmptyElementTest()
        {
            Test(@"Hello;;!", new[] { "Hello", string.Empty, "!" });
        }

        /// <summary>
        /// Test containing an escaped delimiter.
        /// </summary>
        [TestMethod]
        public void EscapedDelimTest()
        {
            Test(@"Hello;Wor\;ld;!", new[] { "Hello", "Wor;ld", "!" });
        }

        /// <summary>
        /// Test containing an escaped escape character.
        /// </summary>
        [TestMethod]
        public void EscapedEscapeTest()
        {
            Test(@"Hello;Wor\\ld;!", new[] { "Hello", @"Wor\ld", "!" });
        }

        /// <summary>
        /// Test containing an complex escapes.
        /// </summary>
        [TestMethod]
        public void ComplexEscapeTest()
        {
            Test(@"He\;llo;Wor\\ld\\;!;\\", new[] { "He;llo", @"Wor\ld\", "!", @"\" });
        }

        /// <summary>
        /// Test for the FormatException if an escape is not properly terminated.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BadEscapeTest()
        {
            var target = new StringListify(';', '\\');
            target.ToList(@"Hello;Wor\\ld;!\");
        }

        private static void Test(string input, string[] list)
        {
            var target = new StringListify(';', '\\');
            var actual = target.ToList(input);

            CollectionAssert.AreEqual(list, actual);

            var actualString = target.FromList(list);

            Assert.AreEqual(input, actualString);
        }

        // ReSharper restore InconsistentNaming
    }
}
