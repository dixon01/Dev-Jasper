// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamForwarder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamForwarder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Stream
{
    using System;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Medi.Ports.Forwarder;

    /// <summary>
    /// Local port forwarder that transfers data to and from a stream.
    /// </summary>
    internal class StreamForwarder : PortForwarderBase<StreamForwardingConfig>
    {
        private readonly int localStreamId = new Random().Next();

        private ForwardingStream stream;

        /// <summary>
        /// Event that is fired when this forwarder is closing its connection.
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Gets the stream to read from and write to.
        /// </summary>
        public Stream Stream
        {
            get
            {
                if (this.stream == null)
                {
                    throw new NotSupportedException("Can't access stream before calling Connect()");
                }

                return this.stream;
            }
        }

        /// <summary>
        /// Connects to the remote forwarder.
        /// This method should only be called once both forwarders are ready.
        /// </summary>
        public void Connect()
        {
            this.SendConnect(this.localStreamId);
            this.stream = new ForwardingStream(this);
        }

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        /// <returns>
        /// The actual <see cref="StreamForwardingConfig"/> used.
        /// </returns>
        protected override StreamForwardingConfig DoStart()
        {
            this.Config.Forwarder = this;
            return this.Config;
        }

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected override void DoStop()
        {
            this.CloseStream();
        }

        /// <summary>
        /// Handles the connect message for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void HandleConnect(MediAddress source, int streamId)
        {
            throw new NotSupportedException("Can't connect to a " + this.GetType().Name);
        }

        /// <summary>
        /// Handles the reception of data for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void HandleData(MediAddress source, int streamId, byte[] data)
        {
            if (this.localStreamId == streamId && this.stream != null)
            {
                this.stream.AppendReadData(data);
            }
        }

        /// <summary>
        /// Handles the disconnection of a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void HandleDisconnect(MediAddress source, int streamId)
        {
            if (this.localStreamId == streamId)
            {
                this.Stop();
            }
        }

        private void CloseStream()
        {
            var s = this.stream;
            this.stream = null;
            if (s == null)
            {
                return;
            }

            this.Logger.Debug("Closing stream");
            this.RaiseClosing();
            s.DoClose();
        }

        private void RaiseClosing()
        {
            var handler = this.Closing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private class ForwardingStream : Stream
        {
            private readonly StreamForwarder owner;

            private readonly ManualResetEvent waitEvent = new ManualResetEvent(false);

            private readonly MessageBuffer readBuffer = new MessageBuffer();

            private bool isOpen;

            public ForwardingStream(StreamForwarder owner)
            {
                this.owner = owner;
                this.isOpen = true;
            }

            public override bool CanRead
            {
                get
                {
                    return this.isOpen;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return this.isOpen;
                }
            }

            public override long Length
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

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

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (count == 0)
                {
                    return 0;
                }

                if (!this.isOpen)
                {
                    throw new IOException("Stream was closed");
                }

                lock (this.readBuffer)
                {
                    if (this.readBuffer.Count > 0)
                    {
                        return this.ReadFromReadBuffer(buffer, offset, count);
                    }
                }

                this.waitEvent.WaitOne();
                lock (this.readBuffer)
                {
                    return this.ReadFromReadBuffer(buffer, offset, count);
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.owner.SendData(this.owner.localStreamId, buffer, offset, count);
            }

            public override void Close()
            {
                base.Close();
                this.owner.CloseStream();
            }

            public void DoClose()
            {
                this.isOpen = false;
                this.owner.SendDisconnect(this.owner.localStreamId);
                this.waitEvent.Set();
            }

            public override void Flush()
            {
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public void AppendReadData(byte[] data)
            {
                if (!this.isOpen || data.Length == 0)
                {
                    return;
                }

                lock (this.readBuffer)
                {
                    this.readBuffer.Append(new MessageBuffer(data, 0, data.Length));
                }

                this.waitEvent.Set();
            }

            private int ReadFromReadBuffer(byte[] buffer, int offset, int count)
            {
                if (!this.isOpen)
                {
                    return 0;
                }

                count = Math.Min(count, this.readBuffer.Count);
                Array.Copy(this.readBuffer.Buffer, this.readBuffer.Offset, buffer, offset, count);
                this.readBuffer.Remove(count);
                if (this.readBuffer.Count == 0)
                {
                    this.waitEvent.Reset();
                }

                return count;
            }
        }
    }
}