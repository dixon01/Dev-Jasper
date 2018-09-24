// --------------------------------------------------------------------------------------------------------------------
// <copyright file="I2CBus.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the I2CBus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.I2C
{
    using System;

    /// <summary>
    /// The I2C bus.
    /// </summary>
    public class I2CBus
    {
        private readonly IntPtr handle;

        private readonly int index;

        private I2CType? type;

        /// <summary>
        /// Initializes a new instance of the <see cref="I2CBus"/> class.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        internal I2CBus(IntPtr handle, int index)
        {
            this.handle = handle;
            this.index = index;
        }

        /// <summary>
        /// Gets the type of this bus.
        /// </summary>
        public I2CType Type
        {
            get
            {
                if (this.type.HasValue)
                {
                    return this.type.Value;
                }

                this.type = (I2CType)NativeMethods.JidaI2CType(this.handle, this.index);
                return this.type.Value;
            }
        }

        /// <summary>
        /// Reads a number of bytes from this I2C bus.
        /// </summary>
        /// <param name="address">
        /// The I2C address to read from.
        /// </param>
        /// <param name="data">
        /// The data array to be filled by the read operation.
        /// </param>
        /// <param name="offset">
        /// The offset into the data array.
        /// </param>
        /// <param name="length">
        /// The number of bytes to read.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <see cref="offset"/> and/or <see cref="length"/> are wrong.
        /// </exception>
        /// <exception cref="JidaException">
        /// If the read operation was unsuccessful.
        /// </exception>
        public unsafe void Read(byte address, byte[] data, int offset, int length)
        {
            if (data.Length < offset + length)
            {
                throw new IndexOutOfRangeException();
            }

            address |= 1;
            fixed (byte* dataPtr = data)
            {
                if (!NativeMethods.JidaI2CRead(this.handle, this.index, address, dataPtr + offset, length))
                {
                    throw new JidaException(string.Format("Couldn't read from I2C bus at 0x{0:X2}", address));
                }
            }
        }

        /// <summary>
        /// Reads a single byte from a given address.
        /// </summary>
        /// <param name="address">
        /// The I2C address to read from.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> read from the given address.
        /// </returns>
        /// <exception cref="JidaException">
        /// If the read operation was unsuccessful.
        /// </exception>
        public unsafe byte ReadByte(byte address)
        {
            address |= 1;
            byte result = 0;
            if (!NativeMethods.JidaI2CRead(this.handle, this.index, address, &result, 1))
            {
                throw new JidaException(string.Format("Couldn't read a byte from I2C bus at 0x{0:X2}", address));
            }

            return result;
        }

        /// <summary>
        /// Writes a number of bytes to this I2C bus.
        /// </summary>
        /// <param name="address">
        /// The I2C address to write to.
        /// </param>
        /// <param name="data">
        /// The data array to be written.
        /// </param>
        /// <param name="offset">
        /// The offset into the data array.
        /// </param>
        /// <param name="length">
        /// The number of bytes to write.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <see cref="offset"/> and/or <see cref="length"/> are wrong.
        /// </exception>
        /// <exception cref="JidaException">
        /// If the write operation was unsuccessful.
        /// </exception>
        public unsafe void Write(byte address, byte[] data, int offset, int length)
        {
            if (data.Length < offset + length)
            {
                throw new IndexOutOfRangeException();
            }

            address &= 0xFE;
            fixed (byte* dataPtr = data)
            {
                if (!NativeMethods.JidaI2CWrite(this.handle, this.index, address, dataPtr + offset, length))
                {
                    throw new JidaException(string.Format("Couldn't write to I2C bus at 0x{0:X2}", address));
                }
            }
        }

        /// <summary>
        /// Writes a single byte to the given address.
        /// </summary>
        /// <param name="address">
        /// The I2C address to write to.
        /// </param>
        /// <param name="data">
        /// The data to write.
        /// </param>
        /// <exception cref="JidaException">
        /// If the write operation was unsuccessful.
        /// </exception>
        public unsafe void WriteByte(byte address, byte data)
        {
            address &= 0xFE;
            if (!NativeMethods.JidaI2CWrite(this.handle, this.index, address, &data, 1))
            {
                throw new JidaException(string.Format("Couldn't write a byte to I2C bus at 0x{0:X2}", address));
            }
        }

        /// <summary>
        /// Writes and reads a byte from the given address in a single operation.
        /// </summary>
        /// <param name="address">
        /// The I2C address.
        /// </param>
        /// <param name="writeData">
        /// The data to write.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> read from the given address.
        /// </returns>
        /// <exception cref="JidaException">
        /// If the write/read operation was unsuccessful.
        /// </exception>
        public unsafe byte WriteReadByte(byte address, byte writeData)
        {
            address &= 0xFE;
            byte readData = 0;
            if (!NativeMethods.JidaI2CWriteReadCombined(this.handle, this.index, address, &writeData, 1, &readData, 1))
            {
                throw new JidaException(string.Format("Couldn't read/write a byte to I2C bus at 0x{0:X2}", address));
            }

            return readData;
        }
    }
}
