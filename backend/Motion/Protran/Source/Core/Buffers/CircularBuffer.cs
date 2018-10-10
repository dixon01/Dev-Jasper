// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Buffers
{
    using System;
    using System.Text;

    /// <summary>
    /// This class represents a data structure organized
    /// as a circular buffer.
    /// </summary>
    public class CircularBuffer
    {
        /// <summary>
        /// Buffer used to store the bytes in
        /// a circular way.
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer"/> class.
        /// </summary>
        /// <param name="size">
        /// The circular buffer's size (expressed in byte).
        /// </param>
        public CircularBuffer(uint size)
        {
            this.buffer = new byte[size];
            this.Clear();
        }

        /// <summary>
        /// Gets the circular buffer.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this.buffer;
            }
        }

        /// <summary>
        /// Gets the actual value of the circular buffer's tail.
        /// Tail is the index of the last byte to which
        /// to consider a telegram.
        /// </summary>
        public int Tail { get; private set; }

        /// <summary>
        /// Gets the actual value of the circular buffer's head.
        /// Head is the index of the first byte from which start
        /// to consider a telegram.
        /// </summary>
        public int Head { get; private set; }

        /// <summary>
        /// Gets the current amount of byte between the head and
        /// the tail, in a circular fashion.
        /// </summary>
        public int CurrentLength
        {
            get
            {
                int dist = (this.Tail >= this.Head)
                                ? this.Tail - this.Head
                                : this.buffer.Length - this.Head + this.Tail;
                return dist;
            }
        }

        /// <summary>
        /// Gets the amount of bytes actually
        /// available in the buffer from the current head
        /// until the real end of the buffer (not in a circular fashion).
        /// </summary>
        public int RemainingLength
        {
            get
            {
                int rest = (this.Tail >= this.Head)
                               ? this.buffer.Length - this.Tail
                               : this.buffer.Length - this.Head;
                return Math.Min(this.buffer.Length - 1, rest);
            }
        }

        /// <summary>
        /// Gets the byte at a specific index of the circular buffer,
        /// in a circular fashion.
        /// </summary>
        /// <param name="index">
        /// The index at which access.
        /// </param>
        /// <returns>The byte at the specified index.</returns>
        public byte this[int index]
        {
            get
            {
                return this.buffer[(this.Head + index) % this.buffer.Length];
            }
        }

        /// <summary>
        /// Clears and resets this buffer. Head and tail aree set back to zero.
        /// </summary>
        public void Clear()
        {
            this.Head = this.Tail = 0;
        }

        /// <summary>
        /// Updates the tail's value depending on a specific
        /// increment of the amount of bytes stored in the circular buffer.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public void UpdateTail(int amount)
        {
            this.Tail = (this.Tail + amount) % this.buffer.Length;
        }

        /// <summary>
        /// Updates the head's value depending on a specific
        /// increment of the amount of bytes stored in the circular buffer.
        /// </summary>
        /// <param name="amount">The amount of bytes to which increment
        /// the head's value circularly.</param>
        public void UpdateHead(int amount)
        {
            this.Head = (this.Head + amount) % this.buffer.Length;
        }

        /// <summary>
        /// Gets the portion of the circular buffer that stays from
        /// it's head and a specific index, circularly.
        /// </summary>
        /// <param name="index">
        /// The index until which get in a circular way (in bytes).
        /// </param>
        /// <returns>
        /// The desired buffer's piece.
        /// </returns>
        public byte[] GetBufferPiece(int index)
        {
            int length = (index >= this.Head)
                             ? index - this.Head
                             : this.buffer.Length - this.Head + index;
            var piece = new byte[length];
            if (this.Head + length < this.buffer.Length)
            {
                Array.Copy(this.buffer, this.Head, piece, 0, length);
            }
            else
            {
                int firstPart = this.buffer.Length - this.Head;
                Array.Copy(this.buffer, this.Head, piece, 0, firstPart);
                Array.Copy(this.buffer, 0, piece, firstPart, length - firstPart);
            }

            return piece;
        }

        /// <summary>
        /// Return the hexadecimal representation of "amount" bytes starting
        /// from the circular buffer's head.
        /// </summary>
        /// <param name="amount">
        /// The amount of bytes to consider.
        /// </param>
        /// <returns>
        /// The hexadecimal string.
        /// </returns>
        public string ToString(int amount)
        {
            var hexStringBuilder = new StringBuilder((amount * 2) + 2);
            hexStringBuilder.Append("0x");
            for (int i = 0; i < amount; i++)
            {
                hexStringBuilder.Append(this.buffer[this.Tail + i].ToString("X2"));
            }

            return hexStringBuilder.ToString();
        }

        /// <summary>
        /// Tells if the current tail's value is bigger than a specific
        /// index, in a circual fashion.
        /// </summary>
        /// <param name="index">The index to be compared with the current
        /// tail's value.</param>
        /// <returns>True if the tail's value is currently bigger than a specific index,
        /// in a circular fashion, else false.</returns>
        public bool IsTailBiggerThan(int index)
        {
            if (this.Tail >= this.Head)
            {
                return this.Tail > index;
            }

            return this.Tail < index;
        }

        /// <summary>
        /// Tells if a specific index is between the actual values
        /// of the head and the tail, in a circular fashion.
        /// </summary>
        /// <param name="index">The index to study.</param>
        /// <returns>True if the index is between the head and the tail,
        /// else false</returns>
        public bool IsIndexBetweenHeadAndTail(int index)
        {
            if (index > this.buffer.Length - 1)
            {
                int rest = index - (this.buffer.Length - 1);
                index = (this.buffer.Length - 1 + rest) % this.buffer.Length;
            }

            if (this.Tail >= this.Head)
            {
                return this.Head <= index && index <= this.Tail;
            }

            return (this.Head <= index && index <= (this.buffer.Length - 1)) ||
                    (0 <= index && index <= this.Tail);
        }
    }
}
