// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantTest.cs" company="Gorba AG">
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
    /// Unit tests for <see cref="Constant"/> class.
    /// </summary>
    [TestClass]
    public class ConstantTest
    {
        /// <summary>
        /// Tests <see cref="Constant"/> with a single character in ASCII.
        /// </summary>
        [TestMethod]
        public void TestSingleAscii()
        {
            var telegram = new byte[] { 0x48, 0x0D, 0x78 };
            var target = new Constant("H");
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 1);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> with a single character in Unicode.
        /// </summary>
        [TestMethod]
        public void TestSingleUnicode()
        {
            var telegram = new byte[] { 0x00, 0x48, 0x00, 0x0D, 0x00, 0x78 };
            var target = new Constant("H");
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 2);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> with a range of characters in ASCII.
        /// </summary>
        [TestMethod]
        public void TestRangeAscii()
        {
            var telegram = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x0D, 0x78 };
            var target = new Constant("Hello");
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 5);
            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> with a range of characters in Unicode.
        /// </summary>
        [TestMethod]
        public void TestRangeUnicode()
        {
            var telegram = new byte[]
                               {
                                   0x00, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00, 0x0D, 0x00, 0x78
                               };
            var target = new Constant("Hello");
            target.ByteInfo = ByteInfo.UnicodeBigEndian;
            var result = target.Check(telegram, 0, 10);
            Assert.AreEqual(10, result);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> with a range of characters in ASCII, leaving characters at the end.
        /// </summary>
        [TestMethod]
        public void TestRangeLeftOver()
        {
            var telegram = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x66, 0x5F, 0x0D, 0x78 };
            var target = new Constant("Hello");
            target.ByteInfo = ByteInfo.Ascii7;
            var result = target.Check(telegram, 0, 7);
            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> with a range of characters that doesn't match.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestRangeMissing()
        {
            var telegram = new byte[] { 0x48, 0x65, 0x6C, 0x6F, 0x66, 0x5F, 0x0D, 0x78 };
            var target = new Constant("Hello");
            target.ByteInfo = ByteInfo.Ascii7;
            target.Check(telegram, 0, 6);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        public void TestVerification()
        {
            var telegram = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x66, 0x5F, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(
                    new ITelegramRule[] { new Constant("Hello"), new Any(2), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="Constant"/> failing using a <see cref="TelegramVerifier"/>.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestVerificationFail()
        {
            var telegram = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x66, 0x5F, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(new ITelegramRule[] { new Constant("Hello"), new EndOfTelegram() });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }
    }
}
