// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramVerifier.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramVerifier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Class that verifies the contents of a telegram
    /// against a list of rules.
    /// </summary>
    public class TelegramVerifier
    {
        private readonly List<ITelegramRule> rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramVerifier"/> class.
        /// </summary>
        /// <param name="rules">
        /// The rules that have to be followed by the telegram to pass verification.
        /// </param>
        public TelegramVerifier(IEnumerable<ITelegramRule> rules)
        {
            this.rules = new List<ITelegramRule>(rules);
        }

        /// <summary>
        /// Gets or sets the byte information for this verifier.
        /// This is set before any other methods of this class
        /// are called.
        /// </summary>
        public ByteInfo ByteInfo { get; set; }

        /// <summary>
        /// Verifies the contents of the telegram with the given rules.
        /// </summary>
        /// <param name="telegram">
        /// The telegram data including header, marker and checksum.
        /// </param>
        /// <exception cref="TelegramVerificationException">
        /// If the verification was unsuccessful.
        /// </exception>
        public void Verify(byte[] telegram)
        {
            var checkEnum = this.rules.GetEnumerator();

            // remove marker and checksum
            int length = telegram.Length - (2 * this.ByteInfo.ByteSize);

            int offset = 0;
            while (checkEnum.MoveNext() && offset < length)
            {
                Debug.Assert(checkEnum.Current != null, "Current can't be null");
                checkEnum.Current.ByteInfo = this.ByteInfo;

                try
                {
                    offset = checkEnum.Current.Check(telegram, offset, length - offset);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new TelegramVerificationException(checkEnum.Current, "end of telegram", telegram, offset - 1);
                }
            }

            if (checkEnum.Current == null)
            {
                return;
            }

            if (!(checkEnum.Current is EndOfTelegram))
            {
                // we expected something else than the end of the telegram
                throw new TelegramVerificationException(checkEnum.Current, "end of telegram", telegram, offset);
            }
        }
    }
}
