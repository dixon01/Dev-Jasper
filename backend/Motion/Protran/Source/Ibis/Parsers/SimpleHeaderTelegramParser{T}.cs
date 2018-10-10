// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleHeaderTelegramParser{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleHeaderTelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers.Verification;

    /// <summary>
    /// Telegram handler for simple telegrams that just contain a predefined header.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram this handler creates in its <see cref="TelegramParser{T}.Parse"/> method.
    /// </typeparam>
    public class SimpleHeaderTelegramParser<T> : TelegramParser<T>
        where T : Telegram, new()
    {
        private readonly byte[] header;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHeaderTelegramParser{T}"/> class.
        /// </summary>
        /// <param name="header">
        /// The header characters.
        /// </param>
        /// <param name="rules">
        /// The telegram checks used to check if the incoming telegram is valid.
        /// </param>
        public SimpleHeaderTelegramParser(string header, params ITelegramRule[] rules)
        {
            this.header = Encoding.ASCII.GetBytes(header);

            if (rules != null && rules.Length > 0)
            {
                var checkList = new List<ITelegramRule>(rules.Length + 1);
                checkList.Add(new Constant(header));
                checkList.AddRange(rules);
                this.Verifier = new TelegramVerifier(checkList);
            }
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header.
        /// </summary>
        public override int HeaderSize
        {
            get
            {
                return this.IdentifierSize;
            }
        }

        /// <summary>
        /// Gets the size in bytes of the telegram header's identifier part
        /// (the first characters).
        /// </summary>
        protected int IdentifierSize
        {
            get
            {
                return this.header.Length * this.ByteInfo.ByteSize;
            }
        }

        /// <summary>
        /// Gets or sets the verifier used to verify telegrams
        /// before parsing them. This can be null if on verification
        /// is required.
        /// </summary>
        protected TelegramVerifier Verifier { get; set; }

        /// <summary>
        /// Check whether this object can handle the given telegram.
        /// This method uses the header given in the constructor to
        /// check the first bytes of the telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given telegram.
        /// </returns>
        public override bool Accept(byte[] telegram)
        {
            if (telegram.Length < this.HeaderSize + this.FooterSize)
            {
                return false;
            }

            int byteSize = this.ByteInfo.ByteSize;
            for (int i = 0; i < this.IdentifierSize; i++)
            {
                if (byteSize == 2 && telegram[i++] != 0)
                {
                    return false;
                }

                if (telegram[i] != this.header[i / byteSize])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parses the given byte array into a telegram.
        /// </summary>
        /// <param name="data">
        /// The telegram including header, marker and checksum.
        /// </param>
        /// <returns>
        /// the telegram object containing the payload of the given telegram.
        /// </returns>
        /// <exception cref="TelegramVerificationException">
        /// If the <see cref="Verifier"/> is set up and the telegram data is not valid.
        /// </exception>
        protected override T Parse(byte[] data)
        {
            if (this.Verifier != null)
            {
                this.Verifier.ByteInfo = this.ByteInfo;
                this.Verifier.Verify(data);
            }

            return base.Parse(data);
        }
    }
}
