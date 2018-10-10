// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core.Peers.Streams;

    /// <summary>
    /// Message that supports streaming instead of sending
    /// the entire message as one telegram.
    /// </summary>
    internal abstract class StreamMessage : IMessage, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamMessage"/> class.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        protected StreamMessage(StreamHeader header)
        {
            this.Header = header;
        }

        /// <summary>
        /// Gets the stream header.
        /// </summary>
        public StreamHeader Header { get; private set; }

        MediAddress IMessage.Source
        {
            get
            {
                return this.Header.Source;
            }
        }

        MediAddress IMessage.Destination
        {
            get
            {
                return this.Header.Destination;
            }
        }

        /// <summary>
        /// Opens the stream to read from it.
        /// In subclasses it might be possible to 
        /// limit the use of this method per object
        /// to exactly once.
        /// </summary>
        /// <returns>
        /// a stream from which data can be read.
        /// </returns>
        public abstract Stream OpenRead();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}