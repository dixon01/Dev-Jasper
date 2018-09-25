// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crc32.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System.Security.Cryptography;

    /// <summary>
    /// CRC32 calculator.
    /// </summary>
    public sealed class Crc32 : HashAlgorithm
    {
        /// <summary>
        /// The default polynomial.
        /// </summary>
        public const uint DefaultPolynomial = 0xedb88320;

        /// <summary>
        /// The default initial value.
        /// </summary>
        public const uint DefaultSeed = 0xFFFFFFFF;

        private readonly uint seedToUse;
        private readonly uint[] tableToUse;
        private static uint[] defaultTable;
        private uint hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        public Crc32()
        {
            this.tableToUse = InitializeTable(DefaultPolynomial);
            this.seedToUse = DefaultSeed;
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="seedToUse">
        /// The seedToUse.
        /// </param>
        public Crc32(uint polynomial, uint seedToUse)
        {
            this.tableToUse = InitializeTable(polynomial);
            this.seedToUse = seedToUse;
            this.Initialize();
        }

        /// <summary>
        /// Gets HashSize.
        /// </summary>
        public override int HashSize
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Computes the algorithm.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The code.
        /// </returns>
        public uint Compute(byte[] buffer)
        {
            return ~this.CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Computes the algorithm.
        /// </summary>
        /// <param name="seed">
        /// The seedToUse.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The code.
        /// </returns>
        public uint Compute(uint seed, byte[] buffer)
        {
            return ~this.CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Computes the algorithm.
        /// </summary>
        /// <param name="polynomial">
        /// The polynomial.
        /// </param>
        /// <param name="seed">
        /// The seedToUse.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The code.
        /// </returns>
        public uint Compute(uint polynomial, uint seed, byte[] buffer)
        {
            return ~this.CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Initializes this Crc32 calculator.
        /// </summary>
        public override void Initialize()
        {
            this.hash = this.seedToUse;
        }

        /// <summary>
        /// Calculates the hash.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        protected override void HashCore(byte[] buffer, int start, int length)
        {
            this.hash = this.CalculateHash(this.tableToUse, this.hash, buffer, start, length);
        }

        /// <summary>
        /// Finalizes the hash code.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        protected override byte[] HashFinal()
        {
            byte[] hashBuffer = this.UInt32ToBigEndianBytes(~this.hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        private static uint[] InitializeTable(uint polynomial)
        {
            if (polynomial == DefaultPolynomial && defaultTable != null)
            {
                return defaultTable;
            }

            var createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                var entry = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry = entry >> 1;
                    }
                }

                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
            {
                defaultTable = createTable;
            }

            return createTable;
        }

        private uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint crc = seed;
            for (int i = start; i < size; i++)
            {
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            }

            return crc;
        }

        private byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new[]
                {
                    (byte)((x >> 24) & 0xff), (byte)((x >> 16) & 0xff), (byte)((x >> 8) & 0xff), (byte)(x & 0xff) 
                };
        }
    }
}
