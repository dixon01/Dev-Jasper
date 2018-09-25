﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HexDigitTest.cs" company="Gorba AG">
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
    /// Unit tests for <see cref="HexDigit"/> class.
    /// </summary>
    [TestClass]
    public class HexDigitTest
    {
        /// <summary>
        /// Tests <see cref="HexDigit"/> with a single digit in ASCII.
        /// </summary>
        [TestMethod]
        public void TestSingleAscii()
        {
            var telegram = new byte[] { 0x3B, 0x0D, 0x78 };
            var target = new HexDigit();
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 1);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> with a single digit in Unicode.
        /// </summary>
        [TestMethod]
        public void TestSingleUnicode()
        {
            var telegram = new byte[] { 0x00, 0x3F, 0x00, 0x0D, 0x00, 0x78 };
            var target = new HexDigit();
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 2);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> with a range of digits in ASCII.
        /// </summary>
        [TestMethod]
        public void TestRangeAscii()
        {
            var telegram = new byte[] { 0x39, 0x31, 0x3C, 0x3A, 0x0D, 0x78 };
            var target = new HexDigit(2, 4);
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 4);
            Assert.AreEqual(4, result);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> with a range of digits in Unicode.
        /// </summary>
        [TestMethod]
        public void TestRangeUnicode()
        {
            var telegram = new byte[] { 0x00, 0x3C, 0x00, 0x31, 0x00, 0x3F, 0x00, 0x37, 0x00, 0x0D, 0x00, 0x78 };
            var target = new HexDigit(2, 4);
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 8);
            Assert.AreEqual(8, result);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> with a range of digits in ASCII, leaving characters at the end.
        /// </summary>
        [TestMethod]
        public void TestRangeLeftOver()
        {
            var telegram = new byte[] { 0x34, 0x3F, 0x66, 0x5F, 0x0D, 0x78 };
            var target = new HexDigit(2, 4);
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 4);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Tests <see cref="Digit"/> with a range of digits that doesn't match.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestRangeMissing()
        {
            var telegram = new byte[] { 0x3F, 0x34, 0x5C, 0x31, 0x0D, 0x78 };
            var target = new HexDigit(3, 5);
            target.ByteInfo = ByteInfo.Ascii7;
            target.Check(telegram, 0, 4);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        public void TestVerification()
        {
            var telegram = new byte[] { 0x39, 0x3C, 0x34, 0x3A, 0x49, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(new ITelegramRule[] { new HexDigit(2, 5), new Any(), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="HexDigit"/> failing using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestVerificationFail()
        {
            var telegram = new byte[] { 0x36, 0x3D, 0x46, 0x5F, 0x0D, 0x78 };

            var verifier = new TelegramVerifier(new ITelegramRule[] { new HexDigit(2, 5), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }
    }
}
