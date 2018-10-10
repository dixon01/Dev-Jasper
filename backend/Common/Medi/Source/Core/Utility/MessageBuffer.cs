// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBuffer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageBuffer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Utility
{
    using System;
    using System.IO;

    /// <summary>
    /// Reusable buffer used for transmitting messages.
    /// </summary>
    internal class MessageBuffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBuffer"/> class
        /// with a default buffer size of 4096 bytes.
        /// </summary>
        public MessageBuffer()
            : this(4096)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBuffer"/> class.
        /// </summary>
        /// <param name="size">
        /// The size of this buffer.
        /// </param>
        public MessageBuffer(int size)
        {
            this.Buffer = new byte[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBuffer"/> class.
        /// </summary>
        /// <param name="buffer">
        /// A buffer that will be used in this object.
        /// </param>
        /// <param name="offset">
        /// The offset into the buffer where data can be found.
        /// </param>
        /// <param name="count">
        /// The number of bytes in the buffer, starting from offset.
        /// </param>
        public MessageBuffer(byte[] buffer, int offset, int count)
        {
            this.Buffer = buffer;
            this.Offset = offset;
            this.Count = count;
        }

        /// <summary>
        /// Gets the raw underlying buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the offset where data can be found in this buffer.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes in this buffer, starting from <see cref="Offset"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets the size of the underlying buffer.
        /// </summary>
        public int Size
        {
            get
            {
                return this.Buffer.Length;
            }
        }

        /// <summary>
        /// Opens a stream to start reading from this buffer at the given offset.
        /// </summary>
        /// <param name="offset">
        /// The offset, starting from position 0 in the stream, not from <see cref="Offset"/>!
        /// </param>
        /// <returns>
        /// A stream that can be used to read from this buffer.
        /// </returns>
        public Stream OpenRead(int offset)
        {
            return this.OpenRead(offset, this.Count - offset);
        }

        /// <summary>
        /// Opens a stream to start reading from this buffer at the given offset for the given number of bytes.
        /// </summary>
        /// <param name="offset">
        /// The offset, starting from position 0 in the stream, not from <see cref="Offset"/>!
        /// </param>
        /// <param name="count">
        /// The number of bytes that will be available to read, counting from offset.
        /// </param>
        /// <returns>
        /// A stream that can be used to read from this buffer.
        /// </returns>
        public Stream OpenRead(int offset, int count)
        {
            return new MemoryStream(this.Buffer, offset, count, false, true);
        }

        /// <summary>
        /// Resets this buffer (<see cref="Offset"/> and <see cref="Count"/> are set to 0).
        /// The data in the buffer is not cleared/reset.
        /// </summary>
        public void Clear()
        {
            this.Offset = 0;
            this.Count = 0;
        }

        /// <summary>
        /// Appends the contents from the given buffer to the end of this buffer.
        /// If necessary this buffer will be enlarged to hold all data.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to append at this buffer's end (= <see cref="Offset"/> + <see cref="Count"/>).
        /// </param>
        public void Append(MessageBuffer buffer)
        {
            this.EnsureSize(this.Offset + this.Count + buffer.Count);

            Array.Copy(buffer.Buffer, buffer.Offset, this.Buffer, this.Offset + this.Count, buffer.Count);
            this.Count += buffer.Count;
        }

        /// <summary>
        /// Removes <see cref="position"/> bytes from the beginning of this buffer.
        /// </summary>
        /// <param name="position">the position to which (excluding) to remove all bytes.</param>
        public void Remove(int position)
        {
            if (position > this.Count)
            {
                throw new ArgumentOutOfRangeException("position", "Can't be greater than Count.");
            }

            this.Offset += position;
            this.Count -= position;

            if (this.Count > 0)
            {
                Array.Copy(this.Buffer, this.Offset, this.Buffer, 0, this.Count);
            }

            this.Offset = 0;
        }

        /// <summary>
        /// Ensures that this buffer's size is at least
        /// <see cref="newSize"/> bytes big.
        /// </summary>
        /// <param name="newSize">
        /// The new minimal size.
        /// </param>
        public void EnsureSize(int newSize)
        {
            if (newSize <= this.Size)
            {
                return;
            }

            newSize = Math.Max(newSize, 2 * this.Size);

            var newBuffer = new byte[newSize];
            Array.Copy(this.Buffer, newBuffer, this.Count);
            this.Buffer = newBuffer;
        }
    }
}
