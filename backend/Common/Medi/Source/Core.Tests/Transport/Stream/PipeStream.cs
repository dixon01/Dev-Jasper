// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeStream.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// Stream that pipes writes into another <see cref="PipeStream"/>.
    /// </summary>
    [DebuggerDisplay(@"PipeStream @{GetHashCode()}")]
    public class PipeStream : Stream
    {
        private readonly MessageBuffer readBuffer = new MessageBuffer();

        private readonly object readLock = new object();

        private PipeStream remote;
        private bool isOpen;

        private ReadAsyncResult readRequest;

        private int bytesWritten;

        /// <summary>
        /// Gets or sets after how many bytes the stream should be closed.
        /// This is used for testing to make sure connections can be reopened and transfers recovered.
        /// </summary>
        public int DisconnectAfterWrittenBytes { get; set; }

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
                return this.remote != null;
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
                return false;
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
                return this.remote != null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Connects this pipe to the other pipe and vice versa.
        /// </summary>
        /// <param name="otherEnd">
        /// The other pipe.
        /// </param>
        public void Connect(PipeStream otherEnd)
        {
            this.remote = otherEnd;
            otherEnd.remote = this;

            this.bytesWritten = 0;
            otherEnd.bytesWritten = 0;

            this.isOpen = true;
            otherEnd.isOpen = true;
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles)
        /// associated with the current stream.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Close()
        {
            base.Close();

            if (!this.isOpen)
            {
                return;
            }

            this.isOpen = false;

            var otherEnd = this.remote;
            if (otherEnd != null)
            {
                otherEnd.Dispose();
            }

            ReadAsyncResult request;
            lock (this.readLock)
            {
                request = this.readRequest;
                this.readRequest = null;
            }

            if (request != null)
            {
                ThreadPool.QueueUserWorkItem(
                    s => request.CompleteException(new IOException("Pipe has been closed"), false));
            }
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and
        /// causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
        {
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. </param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the
        /// reference point used to obtain the new position. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes. </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking,
        /// such as if the stream is constructed from a pipe or console output. </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and
        /// advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested
        /// if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between <paramref name="offset"/> and
        /// (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read
        /// from the current source. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing
        /// the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream. </param>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = this.BeginRead(buffer, offset, count, null, null);
            return this.EndRead(result);
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that represents the asynchronous read, which could still be pending.
        /// </returns>
        /// <param name="buffer">The buffer to read the data into. </param>
        /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data
        /// read from the stream. </param>
        /// <param name="count">The maximum number of bytes to read. </param>
        /// <param name="callback">An optional asynchronous callback, to be called when the read is complete. </param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous read request
        /// from other requests. </param>
        public override IAsyncResult BeginRead(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var request = new ReadAsyncResult(buffer, offset, count, callback, state);

            this.CheckOpen();

            lock (this.readLock)
            {
                if (this.readBuffer.Count == 0)
                {
                    this.readRequest = request;
                    return request;
                }

                count = Math.Min(count, this.readBuffer.Count);
                Array.Copy(this.readBuffer.Buffer, 0, buffer, offset, count);
                this.readBuffer.Remove(count);
            }

            ThreadPool.QueueUserWorkItem(s => request.Complete(count, false));
            return request;
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested.
        /// Streams return zero (0) only at the end of the stream, otherwise, they should block until
        /// at least one byte is available.
        /// </returns>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish. </param>
        public override int EndRead(IAsyncResult asyncResult)
        {
            try
            {
                var result = (ReadAsyncResult)asyncResult;
                try
                {
                    result.WaitForCompletionAndVerify();
                }
                finally
                {
                    this.readRequest = null;
                }

                return result.Value;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IOException("Couldn't read from stream", ex);
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and
        /// advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes
        /// from <paramref name="buffer"/> to the current stream. </param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin
        /// copying bytes to the current stream. </param>
        /// <param name="count">The number of bytes to be written to the current stream. </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckOpen();

            if (this.DisconnectAfterWrittenBytes <= 0)
            {
                this.remote.Append(buffer, offset, count);
                Thread.Yield();
            }
            else
            {
                count = Math.Min(count, this.DisconnectAfterWrittenBytes - this.bytesWritten);

                this.remote.Append(buffer, offset, count);
                this.bytesWritten += count;
                if (this.bytesWritten >= this.DisconnectAfterWrittenBytes)
                {
                    // give the reader a chance to catch up until here
                    while (this.remote.readBuffer.Count > 0)
                    {
                        Thread.Sleep(100);
                    }

                    this.Close();
                    throw new IOException("Stream reached disconnect limit");
                }
            }
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
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous write request
        /// from other requests. </param>
        public override IAsyncResult BeginWrite(
            byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<int>(callback, state);
            try
            {
                this.Write(buffer, offset, count);
                result.Complete(count, true);
            }
            catch (Exception ex)
            {
                result.TryCompleteException(new IOException("Couldn't write", ex), true);
            }

            return result;
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request. </param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            try
            {
                var result = (SimpleAsyncResult<int>)asyncResult;
                result.WaitForCompletionAndVerify();
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IOException("Couldn't write to stream", ex);
            }
        }

        private void Append(byte[] buffer, int offset, int count)
        {
            ReadAsyncResult request;
            lock (this.readLock)
            {
                if (this.readRequest == null)
                {
                    this.readBuffer.Append(new MessageBuffer(buffer, offset, count));
                    return;
                }

                request = this.readRequest;
                this.readRequest = null;

                if (request.Buffer.Count < count)
                {
                    this.readBuffer.Append(
                        new MessageBuffer(
                            buffer,
                            offset + request.Buffer.Count,
                            count - request.Buffer.Count));
                    count = request.Buffer.Count;
                }
            }

            Array.Copy(buffer, offset, request.Buffer.Buffer, request.Buffer.Offset, count);

            ThreadPool.QueueUserWorkItem(s => request.Complete(count, false));
        }

        private void CheckOpen()
        {
            if (this.remote == null)
            {
                throw new IOException("Pipe is not connected to remote end");
            }

            if (!this.isOpen)
            {
                throw new IOException("Pipe is closed");
            }
        }

        private class ReadAsyncResult : SimpleAsyncResult<int>
        {
            public ReadAsyncResult(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Buffer = new MessageBuffer(buffer, offset, count);
            }

            public MessageBuffer Buffer { get; private set; }
        }
    }
}
