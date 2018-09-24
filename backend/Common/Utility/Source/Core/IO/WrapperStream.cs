// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperStream.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream base class that wraps another stream.
    /// Implementations of this class need to call
    /// <see cref="Open"/> in their constructor to set the underlying stream.
    /// </summary>
    public abstract class WrapperStream : Stream
    {
        private Stream wrapped;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream supports reading; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanRead
        {
            get
            {
                return this.wrapped.CanRead;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream supports seeking; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanSeek
        {
            get
            {
                return this.wrapped.CanSeek;
            }
        }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <returns>
        /// A value that determines whether the current stream can time out.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool CanTimeout
        {
            get
            {
                return this.wrapped.CanTimeout;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanWrite
        {
            get
            {
                return this.wrapped.CanWrite;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override long Length
        {
            get
            {
                return this.wrapped.Length;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                return this.wrapped.Position;
            }

            set
            {
                this.wrapped.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the
        /// stream will attempt to read before timing out.
        /// </summary>
        /// <returns>
        /// A value, in milliseconds, that determines how long the stream will
        /// attempt to read before timing out.
        /// </returns>
        public override int ReadTimeout
        {
            get
            {
                return this.wrapped.ReadTimeout;
            }

            set
            {
                this.wrapped.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the
        /// stream will attempt to write before timing out.
        /// </summary>
        /// <returns>
        /// A value, in milliseconds, that determines how long the stream will
        /// attempt to write before timing out.
        /// </returns>
        public override int WriteTimeout
        {
            get
            {
                return this.wrapped.WriteTimeout;
            }

            set
            {
                this.wrapped.WriteTimeout = value;
            }
        }

        /// <summary>
        /// Closes the current stream and releases any resources
        /// (such as sockets and file handles) associated with the current stream.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Close()
        {
            this.wrapped.Close();
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream
        /// and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
        {
            this.wrapped.Flush();
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
            return this.wrapped.BeginRead(buffer, offset, count, callback, state);
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
            return this.wrapped.EndRead(asyncResult);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <returns>
        /// An IAsyncResult that represents the asynchronous write, which could still be pending.
        /// </returns>
        /// <param name="buffer">The buffer to write data from. </param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing. </param>
        /// <param name="count">The maximum number of bytes to write. </param>
        /// <param name="callback">An optional asynchronous callback, to be called when the write is complete. </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular asynchronous write request from other requests.
        /// </param>
        /// <exception cref="IOException">
        /// Attempted an asynchronous write past the end of the stream, or a disk error occurs.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more of the arguments is invalid.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The current Stream implementation does not support the write operation.
        /// </exception>
        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.wrapped.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request. </param>
        /// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is null. </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="asyncResult"/> didn't originate from <see cref="BeginWrite"/> method on the current stream.
        /// </exception>
        /// <exception cref="IOException">The stream is closed or an internal error has occurred.</exception>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.wrapped.EndWrite(asyncResult);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
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
            return this.wrapped.Seek(offset, origin);
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">
        /// The desired length of the current stream in bytes.
        /// </param>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the
        /// stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            this.wrapped.SetLength(value);
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
            return this.wrapped.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte,
        /// or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an integer, or -1 if at the end of the stream.
        /// </returns>
        /// <exception cref="NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int ReadByte()
        {
            return this.wrapped.ReadByte();
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and
        /// advances the current position within this stream by the number of bytes written.
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
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.wrapped.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">
        /// The byte to write to the stream.
        /// </param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing, or the stream is already closed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void WriteByte(byte value)
        {
            this.wrapped.WriteByte(value);
        }

        /// <summary>
        /// Connects this stream to the underlying <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        protected void Open(Stream stream)
        {
            this.wrapped = stream;
        }
    }
}
