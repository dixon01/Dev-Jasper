// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramVerifierTest.cs" company="Gorba AG">
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
    /// Unit tests for <see cref="TelegramVerifier"/> class.
    /// </summary>
    [TestClass]
    public class TelegramVerifierTest
    {
        /// <summary>
        /// Tests <see cref="TelegramVerifier.Verify"/> using a set of rules.
        /// </summary>
        [TestMethod]
        public void TestMixedVerification()
        {
            var telegram = new byte[] { 0x43, 0x41, 0x66, 0x3F, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(
                    new ITelegramRule[]
                        {
                            new CharRange(2, "ABCDEF"), new Constant("f"), new HexDigit(), new EndOfTelegram()
                        });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="TelegramVerifier.Verify"/> using a set of rules without an <see cref="EndOfTelegram"/>.
        /// </summary>
        [TestMethod]
        public void TestMixedOpenVerification()
        {
            var telegram = new byte[] { 0x43, 0x41, 0x66, 0x3F, 0x38, 0x47, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(
                    new ITelegramRule[]
                        {
                            new CharRange(2, "ABCDEF"), new Constant("f")
                        });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="TelegramVerifier.Verify"/> failing using a set of rules.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestMixedVerificationFail()
        {
            var telegram = new byte[] { 0x43, 0x41, 0x66, 0x3F, 0x38, 0x37, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(
                    new ITelegramRule[]
                        {
                            new CharRange(1, 3, "ABCDEF"), new Constant("f"), new HexDigit(1, 2), new EndOfTelegram()
                        });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }

        /// <summary>
        /// Tests <see cref="TelegramVerifier.Verify"/> failing
        /// using a set of rules without an <see cref="EndOfTelegram"/>.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(TelegramVerificationException), AllowDerivedTypes = false)]
        public void TestMixedOpenVerificationFail()
        {
            var telegram = new byte[] { 0x43, 0x41, 0x66, 0x4F, 0x38, 0x47, 0x0D, 0x78 };

            var verifier =
                new TelegramVerifier(
                    new ITelegramRule[]
                        {
                            new CharRange(1, 3, "ABCDEF"), new Constant("f"), new HexDigit()
                        });
            verifier.ByteInfo = ByteInfo.Ascii7;
            verifier.Verify(telegram);
        }
    }
}
