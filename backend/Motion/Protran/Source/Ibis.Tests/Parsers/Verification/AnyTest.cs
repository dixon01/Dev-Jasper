// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnyTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnyTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Parsers.Verification
{
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="Any"/> class.
    /// </summary>
    [TestClass]
    public class AnyTest
    {
        /// <summary>
        /// Tests <see cref="Any"/> with a single character in ASCII.
        /// </summary>
        [TestMethod]
        public void TestSingleAscii()
        {
            var telegram = new byte[] { 0x5C, 0x0D, 0x78 };
            var target = new Any();
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 1);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests <see cref="Any"/> with a single character in Unicode.
        /// </summary>
        [TestMethod]
        public void TestSingleUnicode()
        {
            var telegram = new byte[] { 0x00, 0x7D, 0x00, 0x0D, 0x00, 0x78 };
            var target = new Any();
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 2);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Tests <see cref="Any"/> with a range of characters in ASCII.
        /// </summary>
        [TestMethod]
        public void TestRangeAscii()
        {
            var telegram = new byte[] { 0x5C, 0x41, 0x66, 0x5F, 0x0D, 0x78 };
            var target = new Any(2, 4);
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 4);
            Assert.AreEqual(4, result);
        }

        /// <summary>
        /// Tests <see cref="Any"/> with a range of characters in Unicode.
        /// </summary>
        [TestMethod]
        public void TestRangeUnicode()
        {
            var telegram = new byte[] { 0x00, 0x5C, 0x00, 0x41, 0x00, 0x66, 0x00, 0x5F, 0x00, 0x0D, 0x00, 0x78 };
            var target = new Any(2, 4);
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 8);
            Assert.AreEqual(8, result);
        }

        /// <summary>
        /// Tests <see cref="Any"/> with a range of characters in ASCII, leaving characters at the end.
        /// </summary>
        [TestMethod]
        public void TestRangeLeftOver()
        {
            var telegram = new byte[] { 0x5C, 0x41, 0x66, 0x5F, 0x0D, 0x78 };
            var target = new Any(1, 3);
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 4);
            Assert.AreEqual(3, result);
        }

        /// <summary>
        /// Tests <see cref="Any"/> with a range of characters that is greater than the telegram.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestRangeMissing()
        {
            var telegram = new byte[] { 0x5C, 0x41, 0x0D, 0x78 };
            var target = new Any(3, 5);
            target.ByteInfo = ByteInfo.Ascii7;
            target.Check(telegram, 0, 2);
        }

        /// <summary>
        /// Tests <see cref="Any"/> using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        public void TestVerification()
        {
            var telegram = new byte[] { 0x5C, 0x41, 0x66, 0x5F, 0x0D, 0x78 };

            var verifier = new TelegramVerifier(new ITelegramRule[] { new Any(2, 5), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="Any"/> failing using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestVerificationFail()
        {
            var telegram = new byte[] { 0x5C, 0x41, 0x66, 0x5F, 0x0D, 0x78 };

            var verifier = new TelegramVerifier(new ITelegramRule[] { new Any(1, 3), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }
    }
}
