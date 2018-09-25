// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentStream.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContentStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream that reads only a certain amount of bytes from the underlying stream.
    /// </summary>
    public class ContentStream : Stream
    {
        private readonly long contentLength;

        private long readBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentStream"/> class.
        /// </summary>
        /// <param name="baseStream">
        /// The underlying stream.
        /// </param>
        /// <param name="contentLength">
        /// The content length.
        /// </param>
        public ContentStream(Stream baseStream, long contentLength)
        {
            this.BaseStream = baseStream;
            this.contentLength = contentLength;
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        public override long Length
        {
            get
            {
                return this.contentLength;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        public override long Position
        {
            get
            {
                return this.readBytes;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the base stream.
        /// </summary>
        protected Stream BaseStream { get; private set; }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles)
        /// associated with the current stream.
        /// </summary>
        public override void Close()
        {
            var remaining = this.contentLength - this.readBytes;
            if (remaining == 0)
            {
                return;
            }

            // read all remaining bytes until we have read the entire content
            var buffer = new byte[Math.Min(remaining, 4096)];
            while (this.Read(buffer, 0, buffer.Length) > 0)
            {
            }
        }

        /// <summary>
        /// This method throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">
        /// A byte offset relative to the <paramref name="origin"/> parameter.
        /// </param><param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">
        /// The desired length of the current stream in bytes.
        /// </param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the
        /// current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than
        /// the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the
        /// specified byte array with the values between <paramref name="offset"/>
        /// and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin
        /// storing the data read from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = this.contentLength - this.readBytes;
            if (count > remaining)
            {
                count = (int)remaining;
            }

            if (count == 0)
            {
                return 0;
            }

            int read = this.BaseStream.Read(buffer, offset, count);
            this.readBytes += read;
            return read;
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <returns>
        /// An <see cref="IAsyncResult"/> that represents the asynchronous read, which could still be pending.
        /// </returns>
        /// <param name="buffer">
        /// The buffer to read the data into.
        /// </param>
        /// <param name="offset">
        /// The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read.
        /// </param>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the read is complete.
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous read request from other requests.
        /// </param>
        /// <exception cref="IOException">
        /// Attempted an asynchronous read past the end of the stream, or a disk error occurs.</exception>
        /// <exception cref="ArgumentException">One or more of the arguments is invalid. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref="NotSupportedException">
        /// The current Stream implementation does not support the read operation.
        /// </exception>
        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var remaining = this.contentLength - this.readBytes;
            if (count > remaining)
            {
                count = (int)remaining;
            }

            return this.BaseStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the
        /// number of bytes you requested. Streams return zero (0) only at the
        /// end of the stream, otherwise, they should block until at least one byte is available.
        /// </returns>
        /// <param name="asyncResult">
        /// The reference to the pending asynchronous request to finish.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="asyncResult"/> is null. </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="asyncResult"/> didn't originate from a <see cref="BeginRead"/> method on the current stream.
        ///  </exception>
        /// <exception cref="IOException">The stream is closed or an internal error has occurred.</exception>
        public override int EndRead(IAsyncResult asyncResult)
        {
            int read = this.BaseStream.EndRead(asyncResult);
            this.readBytes += read;
            return read;
        }

        /// <summary>
        /// This method throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from
        /// <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin
        /// copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}