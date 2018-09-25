// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crc32Test.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Tests
{
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for Crc32
    /// </summary>
    [TestClass]
    public class Crc32Test
    {
        /// <summary>
        /// A test for Crc32.HashSize
        /// </summary>
        [TestMethod]
        public void HashSizeTest()
        {
            var crc = new Crc32();

            Assert.AreEqual(32, crc.HashSize);
        }

        /// <summary>
        /// A test for Crc32.Compute
        /// </summary>
        [TestMethod]
        public void ComputeTest()
        {
            var bytes = Encoding.ASCII.GetBytes("Hello World");

            var crc = new Crc32();
            var actual = crc.Compute(bytes);

            Assert.AreEqual(0x4A17B156u, actual);
        }

        /// <summary>
        /// A test for Crc32.Compute
        /// </summary>
        [TestMethod]
        public void ComputeTest2()
        {
            var bytes = Encoding.ASCII.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis commodo, mi eu ultrices iaculis, diam erat facilisis ante, hendrerit aliquam urna ante a neque. Ut id nisi et diam ultricies rhoncus facilisis at nibh. Phasellus vitae lorem ligula, eu tempor dui. Nullam ut enim risus, vitae vehicula justo. Ut ac tempor erat. Mauris ut neque non erat lobortis facilisis. Donec imperdiet nisl odio. Duis ac leo dui.");

            var crc = new Crc32();
            var actual = crc.Compute(bytes);

            Assert.AreEqual(0x986240A9u, actual);
        }

        /// <summary>
        /// A test for Crc32.TransformFinalBlock
        /// </summary>
        [TestMethod]
        public void TransformFinalBlockTest()
        {
            var bytes = Encoding.ASCII.GetBytes("Hello World");
            var crc = new Crc32();
            crc.Initialize();
            crc.TransformFinalBlock(bytes, 0, bytes.Length);
            var actual = crc.Hash;

            Assert.AreEqual((byte)0x4A, actual[0]);
            Assert.AreEqual((byte)0x17, actual[1]);
            Assert.AreEqual((byte)0xB1, actual[2]);
            Assert.AreEqual((byte)0x56, actual[3]);
        }
    }
}
